using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        private const string VropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string token = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(token))
            {
                await AcquireToken();
            }
            else
            {
                await FetchVmUsageData();
            }
        }

        private async Task AcquireToken()
        {
            string apiUrl = $"{VropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            string requestBody = "{ \"username\": \"admin\", \"password\": \"VMware123!\" }";

            try
            {
                string tokenXml = await GetApiToken(apiUrl, requestBody);
                XDocument xdoc = XDocument.Parse(tokenXml);
                XElement tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();

                if (tokenElement != null)
                {
                    token = tokenElement.Value;
                    Label.InnerText = $"Token: {token}";
                }
                else
                {
                    Label.InnerText = "Token bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                Label.InnerText = $"Error acquiring token: {ex.Message}";
            }
        }

        private async Task FetchVmUsageData()
        {
            try
            {
                string vmId = await GetVmId("tekscr1");
                if (!string.IsNullOrEmpty(vmId))
                {
                    string usageData = await GetUsageData(vmId);
                    label2.InnerText = usageData;
                }
                else
                {
                    label2.InnerText = "VM ID not found.";
                }
            }
            catch (Exception ex)
            {
                label2.InnerText = $"Error fetching VM data: {ex.Message}";
            }
        }

        private async Task<string> GetApiToken(string url, string requestBody)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = await request.GetRequestStreamAsync())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task<string> GetVmId(string vmName)
        {
            string getIdUrl = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getIdUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {token}";

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseText = await reader.ReadToEndAsync();
                    XDocument xdoc = XDocument.Parse(responseText);
                    XElement resourceElement = xdoc.Descendants(XName.Get("resource", OpsNamespace)).FirstOrDefault();
                    return resourceElement?.Element(XName.Get("identifier", OpsNamespace))?.Value;
                }
            }
        }

        private async Task<string> GetUsageData(string vmId)
        {
            DateTime startTime = DateTime.UtcNow.AddDays(-30);
            DateTime endTime = DateTime.UtcNow;

            long startTimeMillis = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

            string metricsUrl = $"{VropsServer}/suite-api/api/resources/{vmId}/stats?statKey=cpu|usage_average&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=3&intervalType=HOURS&rollUpType=AVG";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metricsUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {token}";

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseXml = await reader.ReadToEndAsync();
                    return ParseUsageData(responseXml);
                }
            }
        }

        private string ParseUsageData(string xmlData)
        {
            XDocument xdoc = XDocument.Parse(xmlData);
            XElement stats = xdoc.Descendants(XName.Get("stat", OpsNamespace)).FirstOrDefault();

            if (stats == null)
            {
                return "No stats data found.";
            }

            long[] timestamps = stats.Element(XName.Get("timestamps", OpsNamespace)).Value.Split(' ').Select(long.Parse).ToArray();
            double[] data = stats.Element(XName.Get("data", OpsNamespace)).Value.Split(' ').Select(double.Parse).ToArray();

            var result = new List<string>();
            for (int i = 0; i < timestamps.Length; i++)
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamps[i]).UtcDateTime;
                result.Add($"{dateTime:yyyy-MM-dd HH:mm:ss}: {data[i]}%");
            }

            return string.Join("<br/>", result);
        }
    }
}
