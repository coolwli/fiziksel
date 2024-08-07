using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml.Linq;

namespace blank_page
{
    public partial class _default : System.Web.UI.Page
    {
        private static readonly HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private const string Token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::22d233c2-bc52-4917-959f-d50b4b0782f4";
        private static readonly string[] MetricsToFilter = { "cpu|usage_average", "mem|usage_average" };

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            Button1_Click(sender, e).ConfigureAwait(false);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Always accept certificates
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            await FetchVmUsageDataAsync();
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string vmId = await GetVmIdAsync("tekscr1");
                if (!string.IsNullOrEmpty(vmId))
                {
                    var metricsData = await GetAllMetricsDataAsync(vmId);
                    var (parsedData, timestamps) = ParseMetricsData(metricsData);
                    SendUsageDataToClient(parsedData, timestamps);
                }
                else
                {
                    DisplayMessage("VM ID not found.");
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error fetching VM data: {ex.Message}");
            }
        }

        private async Task<string> GetAllMetricsDataAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-30)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string statsUrl = $"{VropsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            using var request = new HttpRequestMessage(HttpMethod.Get, statsUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("vRealizeOpsToken", Token);

            try
            {
                var response = await Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Network error while fetching metrics data.", e);
            }
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            using var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("vRealizeOpsToken", Token);

            try
            {
                var response = await Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                var xmlDoc = XDocument.Parse(responseText);
                var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

                var identifierNode = xmlDoc.Descendants(XName.Get("resource", ns.NamespaceName))
                                            .Attributes("identifier")
                                            .FirstOrDefault();
                return identifierNode?.Value;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Network error while fetching VM ID.", e);
            }
        }

        private (Dictionary<string, double[]>, DateTime[]) ParseMetricsData(string xmlData)
        {
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var xmlDoc = XDocument.Parse(xmlData);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

            // Extract timestamps
            var timestampElems = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName))
                                        .Elements(XName.Get("timestamps", ns.NamespaceName))
                                        .FirstOrDefault();
            if (timestampElems != null)
            {
                timestamps = timestampElems.Value.Split(' ')
                                 .Select(ts => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ts)).DateTime)
                                 .ToList();
            }

            // Extract metrics data
            var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName));

            foreach (var stat in stats)
            {
                var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                              .Element(XName.Get("key", ns.NamespaceName))
                              .Value;

                if (key.StartsWith("guestfilesystem") && key.EndsWith("|percentage") || MetricsToFilter.Contains(key))
                {
                    var dataElems = stat.Element(XName.Get("data", ns.NamespaceName));
                    if (dataElems != null)
                    {
                        var dataValues = dataElems.Value.Split(' ')
                                        .Select(d => double.TryParse(d, out double value) ? value : 0.0)
                                        .ToArray();

                        metricsData[key] = dataValues;
                    }
                }
            }

            return (metricsData, timestamps.ToArray());
        }

        private void SendUsageDataToClient(Dictionary<string, double[]> metricsData, DateTime[] timestamps)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("let dates = [" + string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-ddTHH:mm:ss}\"")) + "];");

            foreach (var metric in metricsData)
            {
                string key = metric.Key.Replace("|", "").Replace(@"\", "_").Replace(":", "");
                string dataArray = string.Join(",", metric.Value.Select(v => v.ToString("F2")));
                scriptBuilder.AppendLine($"let {key}Data = [{dataArray}];");
            }

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }

        private void DisplayMessage(string message)
        {
            Label.InnerText = message;
        }
    }
}
