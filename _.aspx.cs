using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Linq;
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
        private string _token = "";
        private string _username;
        private string _password;

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            _username = ConfigurationManager.AppSettings["VropsUsername"];
            _password = ConfigurationManager.AppSettings["VropsPassword"];
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            await AcquireTokenAsync();
        }

        private async Task AcquireTokenAsync()
        {
            string apiUrl = $"{VropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            string requestBody = $"{{ \"username\": \"{_username}\", \"password\": \"{_password}\" }}";

            try
            {
                string tokenXml = await PostApiDataAsync(apiUrl, requestBody);
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
                string vmId = await GetVmIdAsync("tekscr1");
                if (!string.IsNullOrEmpty(vmId))
                {
                    var usageData = await GetUsageDataAsync(vmId);
                    SendUsageDataToClient(usageData.Item1, usageData.Item2, usageData.Item3);
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

        private async Task<string> PostApiDataAsync(string url, string requestBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = byteArray.Length;

            using (var dataStream = await request.GetRequestStreamAsync())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
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

        private async Task<Tuple<double[], double[], DateTime[]>> GetUsageDataAsync(string vmId)
        {
            DateTime startTime = DateTime.UtcNow.AddDays(-30);
            DateTime endTime = DateTime.UtcNow;

            string cpuMetricsUrl = BuildMetricsUrl(vmId, "cpu|usage_average", startTime, endTime);
            string memoryMetricsUrl = BuildMetricsUrl(vmId, "mem|usage_average", startTime, endTime);

            var cpuUsageData = await GetMetricsDataAsync(cpuMetricsUrl);
            var memoryUsageData = await GetMetricsDataAsync(memoryMetricsUrl);

            return new Tuple<double[], double[], DateTime[]>(cpuUsageData.Item1, memoryUsageData.Item1, cpuUsageData.Item2);
        }

        private string BuildMetricsUrl(string vmId, string statKey, DateTime startTime, DateTime endTime)
        {
            long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

            return $"{VropsServer}/suite-api/api/resources/{vmId}/stats?statKey={statKey}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=3&intervalType=HOURS&rollUpType=AVG";
        }

        private async Task<Tuple<double[], DateTime[]>> GetMetricsDataAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseXml = await reader.ReadToEndAsync();
                    return ParseUsageData(responseXml);
                }
            }
        }

        private Tuple<double[], DateTime[]> ParseUsageData(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var stats = xdoc.Descendants(XName.Get("stat", OpsNamespace)).FirstOrDefault();

            if (stats == null)
            {
                return new Tuple<double[], DateTime[]>(Array.Empty<double>(), Array.Empty<DateTime>());
            }

            var timestamps = stats.Element(XName.Get("timestamps", OpsNamespace)).Value.Split(' ').Select(long.Parse).ToArray();
            var data = stats.Element(XName.Get("data", OpsNamespace)).Value.Split(' ').Select(double.Parse).ToArray();

            var dateTimes = timestamps.Select(t => DateTimeOffset.FromUnixTimeMilliseconds(t).UtcDateTime).ToArray();

            return new Tuple<double[], DateTime[]>(data, dateTimes);
        }

        private string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;
        }

        private void SendUsageDataToClient(double[] cpuUsage, double[] memoryUsage, DateTime[] timestamps)
        {
            string cpuUsageArray = string.Join(",", cpuUsage.Select(u => u.ToString()).ToArray());
            string memoryUsageArray = string.Join(",", memoryUsage.Select(u => u.ToString()).ToArray());
            string dateArray = string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-dd HH:mm:ss}\"").ToArray());

            string script = $@"
                var date = [{dateArray}];
                var cpuUsage = [{cpuUsageArray}];
                var memoryUsage = [{memoryUsageArray}];
                console.log(date, cpuUsage, memoryUsage);";

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", script, true);
        }

        private void DisplayMessage(string message)
        {
            Label.InnerText = message;
        }
    }
}
