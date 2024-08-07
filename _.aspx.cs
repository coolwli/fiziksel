using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace blank_page
{
    public partial class _default : System.Web.UI.Page
    {
        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private readonly string _token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::22d233c2-bc52-4917-959f-d50b4b0782f4";
        private readonly string[] _metricsToFilter = { "cpu|usage_average", "mem|usage_average", "disk|usage_average" };

        private static readonly HttpClient HttpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            Button1_Click(sender, e).GetAwaiter().GetResult(); // Ensure async call completes
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // For demo purposes, be cautious with this in production
        }

        protected async Task Button1_Click(object sender, EventArgs e)
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
                    var parsedData = ParseMetricsData(metricsData);
                    SendUsageDataToClient(parsedData.Item1, parsedData.Item2);
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

        private async Task<string> GetVmIdAsync(string vmName)
        {
            var url = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            return await SendRequestAsync(url);
        }

        private async Task<string> GetAllMetricsDataAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-30)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var url = $"{VropsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            return await SendRequestAsync(url);
        }

        private async Task<string> SendRequestAsync(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private Tuple<Dictionary<string, double[]>, DateTime[]> ParseMetricsData(string xmlData)
        {
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var xmlDoc = XDocument.Parse(xmlData);
            var ns = XNamespace.Get(OpsNamespace);

            // Parse metrics data
            var metricElements = xmlDoc.Descendants(ns + "metric");
            foreach (var metricElement in metricElements)
            {
                var metricName = (string)metricElement.Attribute("name");
                if (_metricsToFilter.Contains(metricName))
                {
                    var values = metricElement.Elements(ns + "value")
                                              .Select(v => (double)v)
                                              .ToArray();
                    metricsData[metricName] = values;
                }
            }

            // Parse timestamps
            var timeElements = xmlDoc.Descendants(ns + "timestamp");
            foreach (var timeElement in timeElements)
            {
                if (DateTime.TryParse((string)timeElement, out DateTime parsedDateTime))
                {
                    timestamps.Add(parsedDateTime);
                }
                else
                {
                    Console.WriteLine($"Unable to parse timestamp: {(string)timeElement}");
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
                string key = metric.Key.Replace("|", "_");
                string dataArray = string.Join(",", metric.Value.Select(v => v.ToString("F2")));
                scriptBuilder.AppendLine($"let {key}Data = [{dataArray}];");
            }

            scriptBuilder.AppendLine("fetchData();");

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }

        private void DisplayMessage(string message)
        {
            Label.InnerText = message;
        }
    }
}
