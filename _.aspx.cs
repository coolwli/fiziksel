using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Xml;

namespace ajaxExample
{
    public partial class _default : System.Web.UI.Page
    {
        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string _token;
        private List<string> _desiredMetrics =  new List<string>
            {
                "cpu|usage_average",
                "mem|usage_average"
            };

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            Button1_Click(sender, e);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Use proper validation in production
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            await AcquireTokenAsync();
        }

        private async Task AcquireTokenAsync()
        {
            var apiUrl = $"{VropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = $"{{ \"username\": \"{ConfigurationManager.AppSettings["VropsUsername"]}\", \"password\": \"{ConfigurationManager.AppSettings["VropsPassword"]}\" }}";

            try
            {
                var tokenXml = await PostApiDataAsync(apiUrl, requestBody);
                _token = ExtractTokenFromXml(tokenXml);
                if (!string.IsNullOrEmpty(_token))
                {
                    await FetchVmUsageDataAsync();
                }
                else
                {
                    DisplayMessage("Token bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error acquiring token: {ex.Message}");
            }
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                var vmId = await GetVmIdAsync("tekscr1");
                if (!string.IsNullOrEmpty(vmId))
                {
                    var metricsData = await GetMetricsDataAsync(vmId);
                    SendUsageDataToClient(metricsData);
                }
                else
                {
                    DisplayMessage("VM ID bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error fetching VM data: {ex.Message}");
            }
        }

        private async Task<string> PostApiDataAsync(string url, string requestBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = Encoding.UTF8.GetByteCount(requestBody);

            using (var dataStream = await request.GetRequestStreamAsync())
            {
                var byteArray = Encoding.UTF8.GetBytes(requestBody);
                await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            }

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            var getIdUrl = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";
            var request = (HttpWebRequest)WebRequest.Create(getIdUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                var responseText = await reader.ReadToEndAsync();
                var xdoc = XDocument.Parse(responseText);
                var identifierNode = xdoc.Descendants(XName.Get("resource", OpsNamespace))
                                          .Select(e => e.Attribute("identifier"))
                                          .FirstOrDefault();
                return identifierNode?.Value;
            }
        }

        private async Task<Dictionary<string, Tuple<double[], DateTime[]>>> GetMetricsDataAsync(string vmId)
        {
            var startTime = DateTime.UtcNow.AddDays(-30);
            var endTime = DateTime.UtcNow;
            var allMetricsKeys = _desiredMetrics.Concat(new[] { "guestfilesystem:*\\percentage" }).Distinct();

            var metricsUrl = BuildMetricsUrl(vmId, "*", startTime, endTime);
            var metricData = await GetMetricDataAsync(metricsUrl);

            return ParseMetricsData(metricData, allMetricsKeys);
        }

        private string BuildMetricsUrl(string vmId, string statKey, DateTime startTime, DateTime endTime)
        {
            long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

            return $"{VropsServer}/suite-api/api/resources/{vmId}/stats?statKey={statKey}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";
        }

        private async Task<string> GetMetricDataAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private Dictionary<string, Tuple<double[], DateTime[]>> ParseMetricsData(string xmlData, IEnumerable<string> requiredKeys)
        {
            var metricsData = new Dictionary<string, Tuple<double[], DateTime[]>>();
            var xdoc = XDocument.Parse(xmlData);

            var metrics = xdoc.Descendants(XName.Get("metric", OpsNamespace))
                              .Where(m => requiredKeys.Contains(m.Attribute("statKey")?.Value))
                              .ToList();

            if (!metrics.Any())
            {
                DisplayMessage("Gerekli metrikler bulunamadı.");
                return metricsData;
            }

            var timestamps = metrics.First().Element(XName.Get("timestamps", OpsNamespace))?.Value.Split(' ')
                                    .Select(long.Parse)
                                    .ToArray();

            foreach (var metric in metrics)
            {
                var key = metric.Attribute("statKey")?.Value;
                if (key != null)
                {
                    var data = metric.Element(XName.Get("data", OpsNamespace))?.Value.Split(' ')
                                  .Select(double.Parse)
                                  .ToArray();
                    var dateTimes = timestamps?.Select(t => DateTimeOffset.FromUnixTimeMilliseconds(t).UtcDateTime).ToArray();

                    metricsData[key] = new Tuple<double[], DateTime[]>(data, dateTimes);
                }
            }

            return metricsData;
        }

        private string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;
        }

        private void SendUsageDataToClient(Dictionary<string, Tuple<double[], DateTime[]>> metricsData)
        {
            if (metricsData.Count == 0)
            {
                DisplayMessage("Veri bulunamadı.");
                return;
            }

            var allDates = metricsData.Values.SelectMany(v => v.Item2).Distinct();
            var allDatesArray = string.Join(",", allDates.Select(d => $"\"{d:yyyy-MM-ddTHH:mm:ss}\""));

            var scripts = metricsData.Select(metric =>
            {
                var statKey = metric.Key;
                var data = metric.Value.Item1;
                var timestamps = metric.Value.Item2;

                var dataArray = string.Join(",", data.Select(d => d.ToString("F2")));
                var dataWithDates = timestamps.Select(t => new { Date = t, Value = data[Array.IndexOf(timestamps, t)] });

                var scriptData = string.Join(",", dataWithDates.Select(d => $"{{date: \"{d.Date:yyyy-MM-ddTHH:mm:ss}\", value: {d.Value:F2}}}"));

                return $@"
                    let {statKey}Data = [{scriptData}];
                    // Process metric data
                ";
            }).ToList();

            var script = $@"
                const allDates = [{allDatesArray}];
                {string.Join("\n", scripts)}
                // Fetch or process all data
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", script, true);
        }

        private void DisplayMessage(string message)
        {
            Label.InnerText = message;
        }
    }
}
