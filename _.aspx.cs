using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI;
using System.Configuration;
using System.Net;

namespace ajaxExample
{
    public partial class _default : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private readonly string vRopsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private readonly string _username = ConfigurationManager.AppSettings["VropsUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["VropsPassword"];
        private string _token;
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders =
            {
                Accept = { new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json") }
            }
        };

        static _default()
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    await EnsureTokenAsync();
                }
                catch (Exception ex)
                {
                    LogError("Error during Page Load", ex);
                }
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        private async Task EnsureTokenAsync()
        {
            if (Session["Token"] == null || Session["TokenExpiry"] == null || DateTime.UtcNow >= (DateTime)Session["TokenExpiry"])
            {
                await AcquireTokenAsync();
            }
            else
            {
                _token = Session["Token"].ToString();
                await FetchVmUsageDataAsync();
            }
        }

        private async Task AcquireTokenAsync()
        {
            string apiUrl = $"{vRopsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = new { username = _username, password = _password };

            try
            {
                string tokenJson = await PostApiDataAsync(apiUrl, requestBody);
                _token = ExtractTokenFromJson(tokenJson);

                if (string.IsNullOrEmpty(_token))
                {
                    Response.Write("Error: Token is empty");
                }
                else
                {
                    Session["Token"] = _token;
                    Session["TokenExpiry"] = DateTime.UtcNow.AddMinutes(300);
                    await FetchVmUsageDataAsync();
                }
            }
            catch (Exception ex)
            {
                LogError("Error acquiring token", ex);
                Response.Write("Error acquiring token: " + ex.Message);
            }
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string hostName = "your-host-name"; // Ensure this is set correctly
                string vmId = await GetVmIdAsync(hostName);
                if (!string.IsNullOrEmpty(vmId))
                {
                    string[] patterns = { "guestfilesystem:", "cpu|usage_average", "mem|usage_average" };
                    var (usageData, timestamps) = await GetUsageDataAsync(vmId, patterns);
                    SendUsageDataToClient(usageData, timestamps);
                }
                else
                {
                    Response.Write("VM ID not found.");
                }
            }
            catch (Exception ex)
            {
                LogError("Error fetching VM data", ex);
                Response.Write("Error fetching VM data: " + ex.Message);
            }
        }

        private async Task<(Dictionary<string, double[]>, DateTime[])> GetUsageDataAsync(string vmId, params string[] patterns)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string metricsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            return await GetMetricsDataAsync(metricsUrl, patterns);
        }

        private async Task<(Dictionary<string, double[]>, DateTime[])> GetMetricsDataAsync(string url, params string[] patterns)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url)
                {
                    Headers =
                    {
                        Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("vRealizeOpsToken", _token)
                    }
                };

                var response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                return ParseUsageData(responseJson, patterns);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error fetching metrics data", ex);
            }
        }

        private (Dictionary<string, double[]>, DateTime[]) ParseUsageData(string jsonData, params string[] patterns)
        {
            var dataDictionary = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                var root = doc.RootElement;

                foreach (var stat in root.GetProperty("stats").EnumerateArray())
                {
                    string statKey = stat.GetProperty("key").GetString();
                    if (statKey == null || !patterns.Any(pattern => statKey.Contains(pattern)))
                        continue;

                    if (statKey.Contains("guestfilesystem:") && statKey.EndsWith(":percentage"))
                    {
                        var dataArray = stat.GetProperty("data").EnumerateArray()
                            .Select(d => d.GetDouble())
                            .ToArray();

                        if (dataArray != null)
                        {
                            dataDictionary[statKey] = dataArray;
                        }
                    }

                    var timeStampsArray = stat.GetProperty("timestamps").EnumerateArray()
                        .Select(t => DateTime.Parse(t.GetString()))
                        .ToArray();

                    if (timeStampsArray != null)
                    {
                        timestamps.AddRange(timeStampsArray);
                    }
                }
            }

            return (dataDictionary, timestamps.Distinct().ToArray());
        }

        private async Task<string> PostApiDataAsync(string url, object requestBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{vRopsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";
            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl)
            {
                Headers =
                {
                    Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("vRealizeOpsToken", _token)
                }
            };

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(responseText))
            {
                var root = doc.RootElement;
                var identifierNode = root.GetProperty("resource").GetProperty("identifier").GetString();
                return identifierNode;
            }
        }

        private string ExtractTokenFromJson(string jsonData)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                var root = doc.RootElement;
                return root.GetProperty("token").GetString();
            }
        }

        private void SendUsageDataToClient(Dictionary<string, double[]> metricsData, DateTime[] timestamps)
        {
            var dataToSend = new
            {
                Metrics = metricsData,
                Timestamps = timestamps
            };

            var jsonResponse = JsonSerializer.Serialize(dataToSend);
            Response.ContentType = "application/json";
            Response.Write(jsonResponse);
        }

        private void LogError(string message, Exception ex)
        {
            // Replace with your preferred logging method
            // For example, using a logging library or custom logging logic
            System.Diagnostics.Debug.WriteLine($"{message}: {ex.Message}");
        }
    }
}
