using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace blank_page
{
    public partial class _default : System.Web.UI.Page
    {
        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string _token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::aed863ae-72b9-452d-86d1-cf9536f4fd9c";
        private readonly string[] _metricsToFilter = { "cpu|usage_average", "mem|usage_average", "disk|usage_average" }; // Filtrelemek istediğiniz metrikler

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            Button1_Click(sender, e);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Gerçek ortamda sertifika doğrulamasını yapmalısınız.
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            await AcquireTokenAsync();
        }

        private async Task AcquireTokenAsync()
        {
            // Token işlemi yapıldıktan sonra veri çekme işlemi yapılır
            await FetchVmUsageDataAsync();
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string vmId = await GetVmIdAsync("tekscr1");
                if (!string.IsNullOrEmpty(vmId))
                {
                    string metricsData = await GetAllMetricsDataAsync(vmId);
                    var (metrics, timestamps) = ParseMetricsData(metricsData);
                    SendUsageDataToClient(metrics, timestamps);
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
            DateTime startTime = DateTime.UtcNow.AddDays(-30);
            DateTime endTime = DateTime.UtcNow;

            string statsUrl = BuildAllMetricsUrl(vmId, startTime, endTime);

            return await PostApiDataAsync(statsUrl, string.Empty); // POST yerine GET de olabilir
        }

        private string BuildAllMetricsUrl(string vmId, DateTime startTime, DateTime endTime)
        {
            long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

            return $"{VropsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";
        }

        private async Task<string> PostApiDataAsync(string url, string requestBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json"; // Sadece XML kullanacaksanız bu satırı "application/xml" olarak değiştirebilirsiniz.

            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = byteArray.Length;

            using (var dataStream = await request.GetRequestStreamAsync())
            {
                await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            }

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            var request = (HttpWebRequest)WebRequest.Create(getIdUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseText = await reader.ReadToEndAsync();

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseText);

                    var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsManager.AddNamespace("ops", OpsNamespace);

                    var identifierNode = xmlDoc.SelectSingleNode("//ops:resource/@identifier", nsManager);
                    return identifierNode?.Value;
                }
            }
        }

        private (Dictionary<string, double[]>, DateTime[]) ParseMetricsData(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var statsElements = xdoc.Descendants(XName.Get("stat", OpsNamespace)).ToList();

            var timestamps = statsElements.FirstOrDefault()?.Element(XName.Get("timestamps", OpsNamespace))?.Value.Split(' ').Select(long.Parse).ToArray();
            var dataDictionary = new Dictionary<string, double[]>();

            foreach (var stat in statsElements)
            {
                var statKey = stat.Element(XName.Get("statKey", OpsNamespace))?.Value;

                if (_metricsToFilter.Contains(statKey))
                {
                    var data = stat.Element(XName.Get("data", OpsNamespace))?.Value.Split(' ').Select(double.Parse).ToArray();
                    if (statKey != null && data != null)
                    {
                        dataDictionary[statKey] = data;
                    }
                }
            }

            var dateTimes = timestamps?.Select(t => DateTimeOffset.FromUnixTimeMilliseconds(t).UtcDateTime).ToArray();
            return (dataDictionary, dateTimes);
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
