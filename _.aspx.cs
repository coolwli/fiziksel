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
        #region Constants and Fields

        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private const string ErrorMessage = "An error occurred. Please try again later.";

        private readonly string _username;
        private readonly string _password;
        private readonly string _connectionString;


        private readonly Dictionary<string, List<string>> vropsServerViews = new Dictionary<string, List<string>>
        {
            {
                "https://ptekvrops01.fw.garanti.com.tr",
                new List<string>
                {
                    "/suite-api/internal/views/fe1d2eb3-c8c6-4f90-8e8d-fcb0f475c203/data/export?resourceId=00330e14-5263-4728-8273-a135ae4d22fa&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true&pageSize=10000",
                    "/suite-api/internal/views/037e5f68-2dbd-4628-81fc-e359f01dee86/data/export?resourceId=98141705-743b-4083-87cf-4f8f6cedcaa3&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true"
                }
            },
            {
                "https://apgaravrops801.fw.garanti.com.tr",
                new List<string>
                {
                    "/suite-api/internal/views/fe1d2eb3-c8c6-4f90-8e8d-fcb0f475c203/data/export?resourceId=951a9b2c-696e-49b0-8c02-038297022f32&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true&pageSize=10000",
                    "/suite-api/internal/views/037e5f68-2dbd-4628-81fc-e359f01dee86/data/export?resourceId=951a9b2c-696e-49b0-8c02-038297022f32&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true"
                }
            }
        };
        #endregion

        #region Constructor and Initialization

        public _default()
        {
            _username = WebConfigurationManager.AppSettings["VropsUsername"];
            _password = WebConfigurationManager.AppSettings["VropsPassword"];
            _connectionString = WebConfigurationManager.ConnectionStrings["CloudUnited"]?.ConnectionString
                ?? @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";
        }

        static _default()
        {
            // Only disable certificate validation in development
#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
#endif
        }

        #endregion

        #region Page Events

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                {
                    DisplayError("Configuration error: Username or password not found.");
                    return;
                }

                var allData = new List<Dictionary<string, object>>();

                foreach (var serverEntry in vropsServerViews)
                {
                    var vropsServer = serverEntry.Key;
                    var tokenType = vropsServer.Contains("ptekvrops01") ? "pendik" : "ankara";

                    var serverData = await CheckTokenAndFetchDataAsync(vropsServer, tokenType);
                    if (serverData != null)
                    {
                        allData.AddRange(serverData);
                    }
                }

                if (allData == null)
                {
                    DisplayError(ErrorMessage);
                }
                RegisterClientScript(allData);

            }
            catch (Exception ex)
            {
                LogError(ex);
                DisplayError(ErrorMessage);
            }
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToCsv();
            }
            catch (Exception ex)
            {
                LogError(ex);
                Response.Write($"Export failed: {ErrorMessage}");
            }
        }

        #endregion

        #region Token Management

        private async Task<List<Dictionary<string, object>>> CheckTokenAndFetchDataAsync(string vropsServer, string tokenType)
        {
            try
            {
                var tokenInfo = await ReadTokenInfoAsync(tokenType);

                if (tokenInfo.ExpiryDate <= DateTime.Now)
                {
                    var newToken = await AcquireTokenAsync(vropsServer);
                    if (newToken != null)
                    {
                        var newTokenInfo = new TokenInfo
                        {
                            Token = newToken,
                            ExpiryDate = DateTime.Now.Add(TokenLifetime)
                        };
                        await StoreTokenInfoToDatabaseAsync(tokenType, newTokenInfo);
                        return await GetDataAsync(vropsServer, newToken);
                    }
                    return null;
                }

                // Extend token lifetime
                var extendedTokenInfo = new TokenInfo
                {
                    Token = tokenInfo.Token,
                    ExpiryDate = DateTime.Now.Add(TokenLifetime)
                };
                await StoreTokenInfoToDatabaseAsync(tokenType, extendedTokenInfo);
                return await GetDataAsync(vropsServer, tokenInfo.Token);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        private async Task<string> AcquireTokenAsync(string vropsServer)
        {
            try
            {
                var requestUri = $"{vropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
                var requestBody = new { username = _username, password = _password };
                var jsonContent = new JavaScriptSerializer().Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var response = await HttpClient.PostAsync(requestUri, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var tokenXml = await response.Content.ReadAsStringAsync();
                        return string.IsNullOrWhiteSpace(tokenXml) ? null : ExtractTokenFromXml(tokenXml);
                    }
                    LogError($"Token acquisition failed: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        private static string ExtractTokenFromXml(string xmlData)
        {
            try
            {
                var xdoc = XDocument.Parse(xmlData);
                var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
                return tokenElement?.Value;
            }
            catch (Exception ex)
            {
                // Log XML parsing error
                System.Diagnostics.Debug.WriteLine($"XML parsing error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Data Retrieval

        private async Task<List<Dictionary<string, object>>> GetDataAsync(string vropsServer, string token)
        {
            try
            {
                var vmData = await FetchVmDataAsync(vropsServer, token);
                var hostData = await FetchHostDataAsync(vropsServer, token);

                if (vmData == null || hostData == null)
                {
                    return null;
                }

                return EnrichVmDataWithHostInfo(vmData, hostData);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        private async Task<List<VmInfo>> FetchVmDataAsync(string vropsServer, string token)
        {
            var viewUrl = vropsServerViews[vropsServer][0];
            var vmsUrl = vropsServer+viewUrl;

            using (var request = new HttpRequestMessage(HttpMethod.Get, vmsUrl))
            {
                request.Headers.Add("Authorization", $"vRealizeOpsToken {token}");

                using (var response = await HttpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"VM data fetch failed: {response.StatusCode}");
                        return null;
                    }

                    var jsonData = await response.Content.ReadAsStringAsync();
                    return ParseVmData(jsonData);
                }
            }
        }

        private async Task<Dictionary<string, decimal>> FetchHostDataAsync(string vropsServer, string token)
        {
            var viewUrl = vropsServerViews[vropsServer][1];
            var hostsUrl = vropsServer + viewUrl;

            using (var request = new HttpRequestMessage(HttpMethod.Get, hostsUrl))
            {
                request.Headers.Add("Authorization", $"vRealizeOpsToken {token}");

                using (var response = await HttpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"Host data fetch failed: {response.StatusCode}");
                        return null;
                    }

                    var jsonData = await response.Content.ReadAsStringAsync();
                    return ParseHostData(jsonData);
                }
            }
        }

        #endregion

        #region Data Parsing

        private List<VmInfo> ParseVmData(string jsonData)
        {
            try
            {
                var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                dynamic data = js.Deserialize<dynamic>(jsonData);
                var vmList = new List<VmInfo>();

                foreach (var view in data)
                {
                    foreach (var elements in view.Value)
                    {
                        
                        foreach (var rows in elements["rows"])
                        {
                            
                            foreach (var row in rows)
                            {
                                var vm = new VmInfo
                                {
                                    Name = row.Value["objId"]?.ToString(),
                                    Host = row.Value["1"]?.ToString(),
                                    Cluster = row.Value["2"]?.ToString(),
                                    vCenter = row.Value["3"]?.ToString(),
                                    Cpu = ParseDecimal(row.Value["4"]?.ToString())
                                };
                                vmList.Add(vm);
                            }
                        }
                    }
                }

                return vmList;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new List<VmInfo>();
            }
        }

        private Dictionary<string, decimal> ParseHostData(string jsonData)
        {
            try
            {
                var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                dynamic data = js.Deserialize<dynamic>(jsonData);
                var hostMapping = new Dictionary<string, decimal>();

                foreach (var view in data)
                {
                    foreach (var elements in view.Value)
                    {
                        foreach (var rows in elements["rows"])
                        {
                            foreach (var row in rows)
                            {
                                var hostName = row.Value["objId"]?.ToString();
                                var hostCpu = ParseDecimal(row.Value["1"]?.ToString());

                                if (!string.IsNullOrEmpty(hostName))
                                {
                                    hostMapping[hostName] = hostCpu;
                                }
                            }
                        }
                    }
                }

                return hostMapping;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new Dictionary<string, decimal>();
            }
        }

        private List<Dictionary<string, object>> EnrichVmDataWithHostInfo(List<VmInfo> vmData, Dictionary<string, decimal> hostData)
        {
            var enrichedData = new List<Dictionary<string, object>>();

            foreach (var vm in vmData)
            {
                var vmDict = new Dictionary<string, object>
                {
                    ["name"] = vm.Name ?? string.Empty,
                    ["host"] = vm.Host ?? string.Empty,
                    ["cluster"] = vm.Cluster ?? string.Empty,
                    ["vCenter"] = vm.vCenter ?? string.Empty,
                    ["cpu"] = vm.Cpu
                };

                if (!string.IsNullOrEmpty(vm.Host) && hostData.ContainsKey(vm.Host))
                {
                    var hostCpu = hostData[vm.Host];
                    vmDict["host_cpu"] = hostCpu;
                    vmDict["prc"] = hostCpu > 0 ? Math.Round((vm.Cpu / hostCpu) * 100, 2) : 0;
                }
                else
                {
                    vmDict["host_cpu"] = 1;
                    vmDict["prc"] = 1;
                }

                enrichedData.Add(vmDict);
            }

            return enrichedData;
        }

        #endregion

        #region Database Operations

        private async Task<TokenInfo> ReadTokenInfoAsync(string tokenType)
        {
            var tokenInfo = new TokenInfo();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string query = "SELECT Token, ExpiryDate FROM TokenInfo WHERE TokenType = @TokenType";
                    using (var command = new SqlCommand(query, connection))
                    {
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
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return tokenInfo;
        }

        private async Task StoreTokenInfoToDatabaseAsync(string tokenType, TokenInfo tokenInfo)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string query = @"
                        MERGE TokenInfo AS target
                        USING (SELECT @TokenType AS TokenType) AS source
                        ON target.TokenType = source.TokenType
                        WHEN MATCHED THEN
                            UPDATE SET Token = @Token, ExpiryDate = @ExpiryDate
                        WHEN NOT MATCHED THEN
                            INSERT (TokenType, Token, ExpiryDate) 
                            VALUES (@TokenType, @Token, @ExpiryDate);";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TokenType", tokenType);
                        command.Parameters.AddWithValue("@Token", (object)tokenInfo.Token ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ExpiryDate", tokenInfo.ExpiryDate);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        #endregion

        #region CSV Export

        private void ExportToCsv()
        {
            var jsonData = hiddenField.Value;

            if (string.IsNullOrEmpty(jsonData))
            {
                Response.Write("No data available for export.");
                return;
            }

            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var tableData = js.Deserialize<List<Dictionary<string, object>>>(jsonData);

            if (tableData == null || !tableData.Any())
            {
                Response.Write("No data available for export.");
                return;
            }

            var csv = GenerateCsv(tableData);

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment;filename=cpu_monitor_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Response.Write(csv);
            Response.End();
        }

        private string GenerateCsv(List<Dictionary<string, object>> data)
        {
            var columnNames = new[] { "Name", "Host", "Cluster", "CPU", "Host CPU", "Percentage" };
            var columnKeys = new[] { "name", "host", "cluster", "cpu", "host_cpu", "prc" };

            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", columnNames));

            foreach (var row in data)
            {
                var values = columnKeys.Select(key =>
                    EscapeCsvValue(row.ContainsKey(key) ? row[key]?.ToString() : string.Empty));
                csv.AppendLine(string.Join(",", values));
            }

            return csv.ToString();
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        #endregion

        #region Helper Methods

        private void RegisterClientScript(List<Dictionary<string, object>> data)
        {
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var json = js.Serialize(data);
            var script = $"<script>baseData = {json}; initializeTable(); </script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);
        }

        private void DisplayError(string message)
        {
            if (form1 != null)
            {
                form1.InnerHtml = message;
            }
        }

        private decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? Math.Round(result, 2) : 0;
        }

        private void LogError(Exception ex)
        {
            // Implement proper logging here (e.g., NLog, Serilog, or System.Diagnostics)
            Response.Write($"Error: {ex}");
            // In production, log to file or database
        }

        private void LogError(string message)
        {
            Response.Write($"Error: {message}");
        }

        #endregion
    }

    #region Data Models

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class VmInfo
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Cluster { get; set; }
        public string vCenter { get; set; }
        public decimal Cpu { get; set; }
    }

    #endregion
}
