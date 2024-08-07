using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml.Linq;

namespace blank_page
{
    public partial class _default : Page
    {
        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private const string Token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::22d233c2-bc52-4917-959f-d50b4b0782f4";
        private static readonly string[] MetricsToFilter = { "cpu|usage_average", "mem|usage_average" };
        
        private static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(VropsServer),
            DefaultRequestHeaders = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("vRealizeOpsToken", Token) }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            if (IsPostBack) return;
            Button1_Click(sender, e);
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
                if (string.IsNullOrEmpty(vmId))
                {
                    DisplayMessage("VM ID not found.");
                    return;
                }

                var metricsDataTask = GetAllMetricsDataAsync(vmId);
                var parsedData = ParseMetricsData(await metricsDataTask);
                SendUsageDataToClient(parsedData.Item1, parsedData.Item2);
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error fetching VM data: {ex.Message}");
            }
        }

        private async Task<string> GetAllMetricsDataAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-7)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string statsUrl = $"/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            var response = await HttpClient.GetAsync(statsUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            var response = await HttpClient.GetAsync(getIdUrl);
            response.EnsureSuccessStatusCode();
            var xmlData = await response.Content.ReadAsStringAsync();

            var xmlDoc = XDocument.Parse(xmlData);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");
            var identifierNode = xmlDoc.Root.Element(XName.Get("resource", ns.NamespaceName))
                                            ?.Attribute(XName.Get("identifier", ns.NamespaceName))
                                            ?.Value;
            return identifierNode;
        }

        private Tuple<Dictionary<string, double[]>, DateTime[]> ParseMetricsData(string xmlData)
        {
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var xmlDoc = XDocument.Parse(xmlData);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

            var timestampElems = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName))
                                        .Elements(XName.Get("timestamps", ns.NamespaceName))
                                        .FirstOrDefault();
            if (timestampElems != null)
            {
                timestamps = timestampElems.Value.Split(' ')
                    .Select(ts => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ts)).DateTime)
                    .ToList();
            }

            foreach (var stat in xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName)))
            {
                var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                              .Element(XName.Get("key", ns.NamespaceName))
                              .Value;

                if (key.StartsWith("guestfilesystem") && key.EndsWith("|percentage") ||
                    MetricsToFilter.Contains(key))
                {
                    var dataElems = stat.Element(XName.Get("data", ns.NamespaceName));
                    if (dataElems != null)
                    {
                        var dataValues = dataElems.Value.Split(' ')
                            .Select(d => double.TryParse(d, out var value) ? value : 0.0)
                            .ToArray();

                        metricsData[key] = dataValues;
                    }
                }
            }

            return new Tuple<Dictionary<string, double[]>, DateTime[]>(metricsData, timestamps.ToArray());
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
