using System;
using System.Net;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace vmpedia
{
    public partial class vmscreen : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string vRopsServer;
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private string hostID;
        private string uuid;
        private string vCenter;
        private string _token;
        private List<string> _metricsToFilter = new List<string> { "cpu|usage_average", "mem|usage_average" };
        private Dictionary<string, string> propertyValues = new Dictionary<string, string>();

        static vmscreen()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            hostID = Request.QueryString["id"];
            vCenter = Request.QueryString["vcenter"];

            if (string.IsNullOrEmpty(hostID))
            {
                DisplayHostNameError("Eksik URL.", null);
                return;
            }

            if (!IsPostBack)
            {
                await EnsureTokenAsync();

                if (string.IsNullOrEmpty(_token)) {
                    DisplayHostNameError("token alınamadı.", null);
                    return;
                }

                string vmID = await GetVmIDAsync(hostID);

                if (string.IsNullOrEmpty(vmID)) {
                    DisplayHostNameError("vm id bulunamadı", null);
                    return;
                }

                await FetchVMDataAsync(vmID);
                await FetchVMMetricsAsync(vmID);
            }
        }

        private async Task EnsureTokenAsync()
        {
            vRopsServer = (vCenter == "apgaraavcs801" || vCenter == "apgartksvcs801" || vCenter == "ptekvcsd01")
                ? "https://apgaravrops801.fw.garanti.com.tr"
                : "https://ptekvrops01.fw.garanti.com.tr";

            TokenManager tokenManager = new TokenManager(_httpClient);
            try
            {
                _token = await tokenManager.GetTokenAsync(vCenter);
            }
            catch (Exception ex)
            {
                DisplayHostNameError("An error occurred while fetching token. Please try again later.", ex);
            }
        }

        private async Task<string> GetVmIDAsync(string vmName)
        {
            string getIdUrl = $"{vRopsServer}/suite-api/api/adapterkinds/VMWARE/resourcekinds/virtualmachine/resources?identifiers[VMEntityInstanceUUID]={hostID}";

            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await _httpClient.SendAsync(request))
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
                DisplayHostNameError("Failed to retrieve VM ID.", ex);
            }

            return null;
        }

        private async Task FetchVMDataAsync(string vmID)
        {
            try
            {
                var url = $"{vRopsServer}/suite-api/api/resources/{vmID}/properties";
                using var response = await ExecuteRequestAsync(url);
                var doc = XDocument.Parse(await response.Content.ReadAsStringAsync());

                var properties = new List<string>
                {
                    "summary|guest|ipAddress",
                    "summary|runtime|powerState",
                    "summary|parentVcenter",
                    "summary|parentCluster",
                    "summary|parentHost",
                    "summary|parentDatacenter",
                    "summary|guest|fullName",
                    "summary|parentFolder",
                    "config|hardware|numCpu",
                    "config|createDate",
                    "config|memoryAllocation|shares",
                    "config|hardware|diskSpace",
                    "summary|guest|toolsVersion",
                    "config|hardware|numSockets"
                };

                properties.AddRange(doc.Descendants(XName.Get("property", OpsNamespace))
                    .Where(p => p.Attribute("name")?.Value.StartsWith("virtualDisk:") == true &&
                                (p.Attribute("name")?.Value.EndsWith("|provisioning_type") == true ||
                                p.Attribute("name")?.Value.EndsWith("|configuredGB") == true ||
                                p.Attribute("name")?.Value.EndsWith("|label") == true))
                    .Select(p => p.Attribute("name")?.Value));

                properties.AddRange(doc.Descendants(XName.Get("property", OpsNamespace))
                    .Where(p => p.Attribute("name")?.Value.StartsWith("summary|customTag:") == true &&
                                p.Attribute("name")?.Value.EndsWith("|customTagValue") == true)
                    .Select(p => p.Attribute("name")?.Value));

                foreach (var property in properties)
                {
                    var value = doc.Descendants(XName.Get("property", OpsNamespace))
                        .FirstOrDefault(p => p.Attribute("name")?.Value == property)?
                        .Value;

                    propertyValues[property] = value;
                }
                Response.Write("<h2>VM Properties</h2>");
                Response.Write("<ul>");
                foreach (var property in propertyValues)
                {
                    Response.Write($"<li>{property.Key}: {property.Value}</li>");
                }
                Response.Write("</ul>");
            }
            catch (Exception ex)
            {
                HandleError("Failed to load VM data", ex);
            }
        }

        private async Task FetchVMMetricsAsync(string vmID)
        {
            try
            {
                await DiscoverDiskKeysAsync(vmID);
                var metricsData = await FetchMetricsAsync(vmID);
                SendUsageDataToClient(metricsData.Item1, metricsData.Item2);
            }
            catch (Exception ex)
            {
                DisplayHostNameError("An error occurred while fetching VM metrics.", ex);
            }
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "vRealizeOpsToken", _token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                HandleError("Failed to execute request", ex);
                return null;
            }
        }

        private async Task DiscoverDiskKeysAsync(string vmID)
        {
            string statsUrl = $"{vRopsServer}/suite-api/api/resources/{vmID}/stats/latest";
            var request = new HttpRequestMessage(HttpMethod.Get, statsUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string responseText = await response.Content.ReadAsStringAsync();

                    var xmlDoc = XDocument.Parse(responseText);
                    var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

                    var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName));

                    foreach (var stat in stats)
                    {
                        var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                                      .Element(XName.Get("key", ns.NamespaceName))
                                      .Value;

                        if (key.StartsWith("guestfilesystem") && key.EndsWith("|percentage"))
                        {
                            _metricsToFilter.Add(key);
                        }
                        if (key== "guestfilesystem|usage_total"){
                            var usageTotal = stat.Element(XName.Get("data", ns.NamespaceName))
                                                .Elements(XName.Get("datum", ns.NamespaceName))
                                                .FirstOrDefault()?
                                                .Element(XName.Get("value", ns.NamespaceName))?.Value;
                            propertyValues["guestfilesystem|usage_total"] = usageTotal;
                        }
                        if (key == "guestfilesystem|capacity_total"){
                            var capacityTotal = stat.Element(XName.Get("data", ns.NamespaceName))
                                                .Elements(XName.Get("datum", ns.NamespaceName))
                                                .FirstOrDefault()?
                                                .Element(XName.Get("value", ns.NamespaceName))?.Value;
                            propertyValues["guestfilesystem|capacity_total"] = capacityTotal;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                DisplayHostNameError("Failed to fetch metrics keys.", ex);
            }
        }

        private async Task<Tuple<Dictionary<string, double[]>, DateTime[]>> FetchMetricsAsync(string vmID)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.Now.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            try
            {
                var fetchTasks = _metricsToFilter.Select(async metric =>
                {
                    string metricsUrl = $"{vRopsServer}/suite-api/api/resources/{vmID}/stats?statKey={metric}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=10&intervalType=MINUTES&rollUpType=AVG";
                    var data = await FetchMetricsDataAsync(metricsUrl);
                    var parsedData = ParseMetricsData(data, metric, ref timestamps);
                    if (parsedData != null)
                    {
                        metricsData[metric] = parsedData;
                    }
                });

                await Task.WhenAll(fetchTasks);
            }
            catch (Exception ex)
            {
                DisplayHostNameError("An error occurred while fetching metrics data.", ex);
            }

            return new Tuple<Dictionary<string, double[]>, DateTime[]>(metricsData, timestamps.ToArray());
        }

        private async Task<string> FetchMetricsDataAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayHostNameError("Error fetching metrics data from the server.", ex);
            }

            return null;
        }

        private double[] ParseMetricsData(string data, string metric, ref List<DateTime> timestamps)
        {
            var xmlDoc = XDocument.Parse(data);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

            var statElements = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName))
                                    .Where(e => e.Element(XName.Get("statKey", ns.NamespaceName))
                                                .Element(XName.Get("key", ns.NamespaceName)).Value == metric);

            var values = new List<double>();

            foreach (var statElement in statElements)
            {
                var dataPoints = statElement.Element(XName.Get("data", ns.NamespaceName))
                                            .Elements(XName.Get("datum", ns.NamespaceName));

                foreach (var dataPoint in dataPoints)
                {
                    var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(
                        long.Parse(dataPoint.Element(XName.Get("timestamp", ns.NamespaceName)).Value)).DateTime;

                    if (!timestamps.Contains(timestamp))
                    {
                        timestamps.Add(timestamp);
                    }

                    values.Add(double.Parse(dataPoint.Element(XName.Get("value", ns.NamespaceName)).Value));
                }
            }

            return values.ToArray();
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
                    string dataArray = string.join(",", metric.Value.Select(v => v.ToString("F2")));
                    scriptBuilder.AppendLine($"let {key}Datas = [{dataArray}];");
                }
            }

            scriptBuilder.AppendLine("fetchDisk();");
            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }

        private void DisplayHostNameError(string errorMessage, Exception ex)
        {
            form1.InnerHtml = "";
            string fullErrorMessage = string.IsNullOrEmpty(ex?.Message) ? errorMessage : $"{errorMessage} Details: {ex.Message}";
            Response.Write(fullErrorMessage);
        }

        private void HandleError(string errorMessage, Exception ex)
        {
            // Log the error (not implemented here)
            DisplayHostNameError(errorMessage, ex);
        }
    }
}
