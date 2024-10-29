using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace odmvms
{
    public partial class _default : Page
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string _username;
        private string _password;

        static _default()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            // Retrieve credentials from configuration
            _username = WebConfigurationManager.AppSettings["VropsUsername"];
            _password = WebConfigurationManager.AppSettings["VropsPassword"];

            // Attempt to fetch dashboard data
            var dashboardData = await CheckTokenAndFetchDashboardDataAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
            if (dashboardData == null)
            {
                form1.InnerHtml = "Bir hata olustu, daha sonra deneyiniz..";
            }
            
        }

        private async Task<string> CheckTokenAndFetchDashboardDataAsync(string vropsServer, string tokenType)
        {
            var tokenInfo = await ReadTokenInfoFromDatabaseAsync(tokenType);

            if (tokenInfo.ExpiryDate <= DateTime.Now)
            {
                // Token expired or doesn't exist, acquire a new one
                var newToken = await AcquireTokenAsync(vropsServer);
                if (newToken != null)
                {
                    await StoreTokenInfoToDatabaseAsync(tokenType, new TokenInfo { Token = newToken, ExpiryDate = DateTime.Now.Add(TokenLifetime) });
                    return await GetDataAsync(vropsServer, newToken);
                }
                return null;
            }

            // Token is still valid; extend its lifetime and fetch data
            var extendedTokenInfo = new TokenInfo { Token = tokenInfo.Token, ExpiryDate = DateTime.Now.Add(TokenLifetime) };
            await StoreTokenInfoToDatabaseAsync(tokenType, extendedTokenInfo);
            return await GetDataAsync(vropsServer, tokenInfo.Token);
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
                var tokenXml = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(tokenXml) ? null : ExtractTokenFromXml(tokenXml);
            }
            return null;
        }

        private static string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;
        }

        private async Task<string> GetDataAsync(string vropsServer, string token)
        {
            var getIdUrl = $"{vropsServer}/suite-api/internal/views/e5bb44f3-f7d8-45c5-8819-dfc6e7672463/data/export?resourceId=00330e14-5263-4728-8273-a135ae4d22fa&pageSize=10000&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true";

            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {token}");

            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return ParseDashboardData(jsonData);
                }
            }
            return null;
        }

        private string ParseDashboardData(string jsonData)
        {
            var js = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            dynamic data = js.Deserialize<dynamic>(jsonData);
            var tableRows = new List<Dictionary<string, object>>();

            foreach (var view in data)
            {
                foreach (var elements in view.Value)
                {
                    foreach (var rows in elements["rows"])
                    {
                        var tableRow = new Dictionary<string, object>();
                        foreach (var row in rows)
                        {
                            tableRow["name"] = row.Value["objId"];
                            tableRow["ps"] = row.Value["1"];
                            tableRow["ip"] = row.Value["2"];
                            tableRow["os"] = row.Value["3"];
                            tableRow["cl"] = row.Value["4"];
                            tableRow["vc"] = row.Value["5"];
                        }
                        tableRows.Add(tableRow);
                    }
                }
            }
            var json = js.Serialize(tableRows);
            var script = $"<script>data = {json}; initializeTable(); </script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);

            return "1";
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
                        UPDATE TokenInfo SET Token = @Token, ExpiryDate = @ExpiryDate WHERE TokenType = @TokenType
                    END
                    ELSE
                    BEGIN
                        INSERT INTO TokenInfo (TokenType, Token, ExpiryDate) VALUES (@TokenType, @Token, @ExpiryDate)
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
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                var tableData = js.Deserialize<List<string[]>>(jsonData);

                if (tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Name;Power State;IPv4;OS;Cluster;VCenter");

                foreach (var row in tableData)
                {
                    csv.AppendLine($"{row[0]};{row[1]};{row[2]};{row[3]};{row[4]};{row[5]}");

                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=Replicated VMs.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }
    }

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
