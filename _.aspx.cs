using System;
using System.Net;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using System.Net.Http;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace cpuMonitor
{

    public partial class _default : System.Web.UI.Page
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
            _username = WebConfigurationManager.AppSettings["VropsUsername"];
            _password = WebConfigurationManager.AppSettings["VropsPassword"];

            var data = await CheckTokenAndFetchDataAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
            if (data == null)
            {
                form1.InnerHtml = "Bir hata olustu, daha sonra deneyiniz..";
            }

        }


        private async Task<string> CheckTokenAndFetchDataAsync(string vropsServer, string tokenType)
        {
            var tokenInfo = await ReadTokenInfoAsync(tokenType);

            if (tokenInfo.ExpiryDate <= DateTime.Now)
            {
                var newToken = await AcquireTokenAsync(vropsServer);
                if (newToken != null)
                {
                    await StoreTokenInfoToDatabaseAsync(tokenType, new TokenInfo { Token = newToken, ExpiryDate = DateTime.Now.Add(TokenLifetime) });
                    return await GetDataAsync(vropsServer, newToken);
                }
                return null;
            }

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
            var hostsUrl  = $"{vropsServer}/suite-api/internal/views/037e5f68-2dbd-4628-81fc-e359f01dee86/data/export?resourceId=98141705-743b-4083-87cf-4f8f6cedcaa3&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true";
            var vmsUrl = $"{vropsServer}/suite-api/internal/views/fe1d2eb3-c8c6-4f90-8e8d-fcb0f475c203/data/export?pageSize=10000&resourceId=98141705-743b-4083-87cf-4f8f6cedcaa3&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true";

            var vmrequest = new HttpRequestMessage(HttpMethod.Get, vmsUrl);
            vmrequest.Headers.Add("Authorization", $"vRealizeOpsToken {token}");

            var vmRows = new List<Dictionary<string, object>>();
            var hostRows = new List<Dictionary<string, object>>();

            using (var response = await _httpClient.SendAsync(vmrequest))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var js = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                    dynamic data = js.Deserialize<dynamic>(jsonData);

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
                                    tableRow["host"] = row.Value["1"];
                                    tableRow["cluster"] = row.Value["2"];
                                    decimal.TryParse(row.Value["3"]?.ToString(), out decimal cpuValue);

                                    tableRow["cpu"] = Math.Round(cpuValue,2);
                                }
                                vmRows.Add(tableRow);
                            }
                        }
                    }

                }
            }


            var hostrequest = new HttpRequestMessage(HttpMethod.Get, hostsUrl);
            hostrequest.Headers.Add("Authorization", $"vRealizeOpsToken {token}"); 
            using (var response = await _httpClient.SendAsync(hostrequest))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
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
                                    tableRow["cpu"] = row.Value["1"];

                                }
                                hostRows.Add(tableRow);
                            }
                        }
                    }

                }
            }

            
            var hostMapping = new Dictionary<string, string>();
            foreach (var hostRow in hostRows)
            {
                string hostFirstColumn = hostRow["name"].ToString();
                string hostSecondColumn = hostRow["cpu"].ToString();
                hostMapping[hostFirstColumn] = hostSecondColumn;
            }

            foreach (var vmRow in vmRows)
            {
                string vmSecondColumn = vmRow["host"].ToString();

                if (hostMapping.ContainsKey(vmSecondColumn))
                {
                    decimal.TryParse(vmRow["cpu"]?.ToString(), out decimal cpuValue);
                    decimal.TryParse(hostMapping[vmSecondColumn]?.ToString(), out decimal hostValue);
                    vmRow["host_cpu"] = Math.Round(hostValue, 2);

                    vmRow["prc"] = Math.Round(cpuValue / hostValue*100, 2);

                }
                else
                {
                    vmRow["host_cpu"] = null; 
                }
            }

            var jss = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };

            var json = jss.Serialize(vmRows);
            var script = $"<script>baseData = {json}; initializeTable(); </script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);

            return "1";
        }

        private string ParseViewData(string jsonData)
        {
            
            var script = $"<script>data = {jsonData}; initializeTable(); </script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);

            return "1";
        }

        private async Task<TokenInfo> ReadTokenInfoAsync(string tokenType)
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

            if (string.IsNullOrEmpty(jsonData))
            {
                Response.Write("No data available.");
                return;
            }

            try
            {
                var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                var tableData = js.Deserialize<List<Dictionary<string, object>>>(jsonData);

                if (tableData == null || tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                var columnNames = new List<string> { "Name", "Host", "Cluster", "CPU", "Host CPU", "Percentage" };

                var csv = new StringBuilder();
                csv.AppendLine(string.Join(",", columnNames));

                foreach (var row in tableData)
                {
                    var values = new List<string>
                    {
                        row["name"]?.ToString(),
                        row["host"]?.ToString(),
                        row["cluster"]?.ToString(),
                        row["cpu"]?.ToString(),
                        row["host_cpu"]?.ToString(),
                        row["prc"]?.ToString()
                    };

                    csv.AppendLine(string.Join(",", values));
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment;filename=vmpedia.csv");
                Response.Write(csv.ToString());
            }
            catch (Exception ex)
            {
                Response.Write($"An error occurred: {ex.ToString()}");
            }
        }

    }
}

public class TokenInfo
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}
