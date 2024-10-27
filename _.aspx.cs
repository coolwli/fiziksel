using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Web.Script.Serialization;

namespace odmvms
{
    public partial class _default : System.Web.UI.Page
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        
        private string _username;
        private string _password;

        static _default()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            _username = WebConfigurationManager.AppSettings["VropsUsername"];
            _password = WebConfigurationManager.AppSettings["VropsPassword"];

            string dashboardData = await CheckTokenAndFetchDashboardDataAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik", "9646af37-b6cc-4be7-9445-3595dec7ff03");
            form1.InnerHtml = dashboardData ?? "Bir hata olustu daha sonra deneyiniz..";
        }

        private async Task<string> CheckTokenAndFetchDashboardDataAsync(string vropsServer, string tokenType, string dashboardId)
        {
            var tokenInfo = await ReadTokenInfoFromDatabaseAsync(tokenType);

            if (DateTime.Now >= tokenInfo.ExpiryDate)
            {
                string newToken = await AcquireTokenAsync(vropsServer);
                if (newToken != null)
                {
                    await StoreTokenInfoToDatabaseAsync(tokenType, new TokenInfo { Token = newToken, ExpiryDate = DateTime.Now.Add(TokenLifetime) });
                    return await GetDashboardDataAsync(vropsServer, newToken, dashboardId);
                }

                return null;
            }

            // Extend existing token's lifetime and fetch dashboard data
            var extendedTokenInfo = new TokenInfo { Token = tokenInfo.Token, ExpiryDate = DateTime.Now.Add(TokenLifetime) };
            await StoreTokenInfoToDatabaseAsync(tokenType, extendedTokenInfo);
            return await GetDashboardDataAsync(vropsServer, tokenInfo.Token, dashboardId);
        }

        private async Task<string> AcquireTokenAsync(string vropsServer)
        {
            var requestUri = $"{vropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = new { username = _username, password = _password };
            var jsonContent = new JavaScriptSerializer().Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                string tokenXml = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(tokenXml) ? null : ExtractTokenFromXml(tokenXml);
            }

            return null;
        }

        private static string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", "http://webservice.vmware.com/vRealizeOpsMgr/1.0/")).FirstOrDefault();
            return tokenElement?.Value;
        }

        private async Task<string> GetDashboardDataAsync(string vropsServer, string token, string dashboardId)
        {
            var requestUri = $"{vropsServer}/suite-api/api/dashboard/{dashboardId}/data?_no_links=true";
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null; // Or handle the error appropriately
        }

        private async Task<TokenInfo> ReadTokenInfoFromDatabaseAsync(string tokenType)
        {
            var tokenInfo = new TokenInfo();
            var connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT Token, ExpiryDate FROM TokenInfo WHERE TokenType = @TokenType";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TokenType", tokenType);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        tokenInfo.Token = reader["Token"] as string;
                        tokenInfo.ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate"));
                    }
                }
            }

            return tokenInfo;
        }

        private async Task StoreTokenInfoToDatabaseAsync(string tokenType, TokenInfo tokenInfo)
        {
            var connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"
                    IF EXISTS (SELECT 1 FROM TokenInfo WHERE TokenType = @TokenType)
                    BEGIN
                        UPDATE TokenInfo
                        SET Token = @Token, ExpiryDate = @ExpiryDate
                        WHERE TokenType = @TokenType
                    END
                    ELSE
                    BEGIN
                        INSERT INTO TokenInfo (TokenType, Token, ExpiryDate)
                        VALUES (@TokenType, @Token, @ExpiryDate)
                    END";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TokenType", tokenType);
                command.Parameters.AddWithValue("@Token", (object)tokenInfo.Token ?? DBNull.Value);
                command.Parameters.AddWithValue("@ExpiryDate", tokenInfo.ExpiryDate);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            // Logic for hidden button can be implemented here
        }
    }

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
