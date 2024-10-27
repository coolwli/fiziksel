9646af37-b6cc-4be7-9445-3595dec7ff03

using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Xml.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;

namespace odmvms
{
    public partial class _default : System.Web.UI.Page
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);

        private static  HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
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

            string token = await CheckTokenAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
            if (token == null)
            {
                form1.InnerHtml = "Bir hata olustu daha sonra deneyiniz..";
            }
            else
            {

            }
        }


        private async Task<string> CheckTokenAsync(string vropsServer, string tokenType)
        {
            var tokenInfo = await ReadTokenInfoFromDatabaseAsync(tokenType);

            if (DateTime.Now >= tokenInfo.ExpiryDate)
            {
                string newToken = await AcquireTokenAsync(vropsServer);

                if (newToken != null)
                {

                    var newTokenInfo = new TokenInfo
                    {
                        Token = newToken,
                        ExpiryDate = DateTime.Now.Add(TokenLifetime)
                    };
                    await StoreTokenInfoToDatabaseAsync(tokenType, newTokenInfo);
                    return newToken;
                }

                return null;
            }


            // Extend the existing token's lifetime
            var extendedTokenInfo = new TokenInfo
            {
                Token = tokenInfo.Token,
                ExpiryDate = DateTime.Now.Add(TokenLifetime)
            };
            await StoreTokenInfoToDatabaseAsync(tokenType, extendedTokenInfo);

            return tokenInfo.Token;
        }

        private async Task<string> AcquireTokenAsync(string vropsServer)
        {
            var requestUri = $"{vropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = new
            {
                username = _username,
                password = _password
            };
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

        private async Task<TokenInfo> ReadTokenInfoFromDatabaseAsync(string tokenType)
        {
            var tokenInfo = new TokenInfo { Token = null, ExpiryDate = DateTime.MinValue };
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
        }


    }

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
