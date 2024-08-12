using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Xml;
using System.Web.Script.Serialization;
using System.Net;

namespace vminfo
{
    public partial class vmscreen : Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private const int MaxRetryAttempts = 3;
        private const int RetryDelayMilliseconds = 3000;
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

        private readonly string[] _initialMetricsToFilter = { "cpu|usage_average", "mem|usage_average" };
        private string _vRopsServer;
        private string _hostName;
        private string _token;
        private readonly string _username = ConfigurationManager.AppSettings["VropsUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["VropsPassword"];
        private List<string> _metricsToFilter;

        static vmscreen()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            _hostName = Request.QueryString["id"];
            if (string.IsNullOrEmpty(_hostName))
            {
                DisplayHostNameError();
                return;
            }

            if (!IsPostBack)
            {
                ShowHost();
                await EnsureTokenAsync();
            }
        }

        private void DisplayHostNameError()
        {
            form1.InnerHtml = "";
            Response.Write("ID Bulunamadı");
        }

        private async Task EnsureTokenAsync()
        {
            if (Session["Token"] == null || Session["TokenExpiry"] == null || DateTime.Now >= (DateTime)Session["TokenExpiry"])
            {
                try
                {
                    _token = await AcquireTokenWithRetryAsync();
                    Session["Token"] = _token;
                    Session["TokenExpiry"] = DateTime.Now.AddMinutes(300);
                }
                catch (Exception ex)
                {
                    Response.Write($"Token alım hatası: {ex.Message}");
                    return;
                }
            }
            else
            {
                _token = Session["Token"].ToString();
            }

            await FetchVmUsageDataAsync();
        }

        private async Task<string> AcquireTokenWithRetryAsync()
        {
            for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
            {
                try
                {
                    return await AcquireTokenAsync();
                }
                catch (Exception)
                {
                    if (attempt == MaxRetryAttempts - 1) throw;
                    await Task.Delay(RetryDelayMilliseconds);
                }
            }
            throw new Exception("Failed to acquire token.");
        }

        private async Task<string> AcquireTokenAsync()
        {
            var requestUri = $"{_vRopsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = new { username = _username, password = _password };
            var jsonContent = new JavaScriptSerializer().Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Token alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        private string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string vmId = await GetVmIdAsync(_hostName);
                if (!string.IsNullOrEmpty(vmId))
                {
                    await FetchGuestFileSystemMetricsAsync(vmId);
                    var (metricsData, timestamps) = await FetchMetricsAsync(vmId);
                    SendUsageDataToClient(metricsData, timestamps);
                }
                else
                {
                    Response.Write("VM ID bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                Response.Write($"VM verileri alınırken hata oluştu: {ex.Message}");
            }
        }

        private async Task FetchGuestFileSystemMetricsAsync(string vmId)
        {
            string statsUrl = $"{_vRopsServer}/suite-api/api/resources/{vmId}/stats";
            var request = new HttpRequestMessage(HttpMethod.Get, statsUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await HttpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string responseText = await response.Content.ReadAsStringAsync();
                    var xmlDoc = XDocument.Parse(responseText);
                    var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

                    var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName));
                    _metricsToFilter = new List<string>(_initialMetricsToFilter);

                    foreach (var stat in stats)
                    {
                        var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                                      .Element(XName.Get("key", ns.NamespaceName))
                                      .Value;

                        if (key.StartsWith("guestfilesystem") && key.EndsWith("|percentage"))
                        {
                            _metricsToFilter.Add(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Disk metrikleri alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{_vRopsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await HttpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string responseText = await response.Content.ReadAsStringAsync();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseText);

                    var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsManager.AddNamespace("ops", OpsNamespace);

                    var identifierNode = xmlDoc.SelectSingleNode("//ops:resource/@identifier", nsManager);
                    return identifierNode?.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"VM ID alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        private async Task<(Dictionary<string, double[]>, DateTime[])> FetchMetricsAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var fetchTasks = _metricsToFilter.Select(async metric =>
            {
                string metricsUrl = $"{_vRopsServer}/suite-api/api/resources/{vmId}/stats?statKey={metric}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";
                var data = await FetchMetricsDataAsync(metricsUrl);
                var parsedData = ParseMetricsData(data, metric, ref timestamps);
                if (parsedData != null)
                {
                    metricsData[metric] = parsedData;
                }
            });

            await Task.WhenAll(fetchTasks);

            return (metricsData, timestamps.ToArray());
        }

        private async Task<string> FetchMetricsDataAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await HttpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Metriği alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        private double[] ParseMetricsData(string xmlData, string metricKey, ref List<DateTime> timestamps)
        {
            var xmlDoc = XDocument.Parse(xmlData);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

            var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName))
                              .Where(stat => stat.Element(XName.Get("statKey", ns.NamespaceName))
                                                 .Element(XName.Get("key", ns.NamespaceName))
                                                 .Value == metricKey);

            foreach (var stat in stats)
            {
                var timestampElems = stat.Element(XName.Get("timestamps", ns.NamespaceName));
                if (timestampElems != null && !timestamps.Any())
                {
                    timestamps = timestampElems.Value.Split(' ')
                                  .Select(ts => long.TryParse(ts, out long result)
                                  ? DateTimeOffset.FromUnixTimeMilliseconds(result).DateTime
                                  : DateTime.MinValue)
                                  .Where(t => t != DateTime.MinValue)
                                  .ToList();
                }

                var dataElems = stat.Element(XName.Get("data", ns.NamespaceName));
                if (dataElems != null)
                {
                    return dataElems.Value.Split(' ')
                           .Select(d => double.TryParse(d, out double value) ? value : 0.0)
                           .ToArray();
                }
            }

            return Array.Empty<double>();
        }

        private void SendUsageDataToClient(Dictionary<string, double[]> metricsData, DateTime[] timestamps)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("let dates = [" + string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-ddTHH:mm:ss}\"")) + "];");

            var processedDiskMetrics = new HashSet<string>();

            foreach (var metric in metricsData)
            {
                if (metric.Key.StartsWith("guestfilesystem"))
                {
                    string key = metric.Key.Split(':')[1].Split('|')[0];

                    if (!processedDiskMetrics.Contains(key))
                    {
                        string diskDataArray = string.Join(",", metric.Value.Select(v => v.ToString("F2")));
                        scriptBuilder.AppendLine($"diskDataMap['{key}'] = [{diskDataArray}];");
                        processedDiskMetrics.Add(key);
                    }
                }
                else
                {
                    string key = metric.Key.Substring(0, 3);

                    string dataArray = string.Join(",", metric.Value.Select(v => v.ToString("F2")));
                    scriptBuilder.AppendLine($"let {key}Datas = [{dataArray}];");
                }
            }
            scriptBuilder.AppendLine("fetchDisk();");

            ClientScript.RegisterStartupScript(GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }
    }
}
