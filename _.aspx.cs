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
        private string _token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::22d233c2-bc52-4917-959f-d50b4b0782f4";
        private readonly string[] _metricsToFilter = { "cpu|usage_average", "mem|usage_average" }; // Filtrelemek istediğiniz metrikler


        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            Button1_Click(sender, e);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
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
        private async Task<string> GetAllMetricsDataAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-30)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            string statsUrl = $"{VropsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            var request = (HttpWebRequest)WebRequest.Create(statsUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

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
        private Tuple<Dictionary<string, double[]>, DateTime[]> ParseMetricsData(string xmlData)
        {
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var xmlDoc = XDocument.Parse(xmlData);
            var ns = xmlDoc.Root.GetNamespaceOfPrefix("ops");

            // Extract timestamps once
            var timestampElems = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName))
                                        .Elements(XName.Get("timestamps", ns.NamespaceName))
                                        .FirstOrDefault();
            if (timestampElems != null)
            {
                var timestampsStr = timestampElems.Value.Split(' ');
                foreach (var timestamp in timestampsStr)
                {
                    if (long.TryParse(timestamp, out long ts))
                    {
                        timestamps.Add(DateTimeOffset.FromUnixTimeMilliseconds(ts).DateTime);
                    }
                }
            }

            var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName));

            foreach (var stat in stats)
            {
                var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                              .Element(XName.Get("key", ns.NamespaceName))
                              .Value;
                if (_metricsToFilter.Contains(key))
                {
                    var dataElems = stat.Element(XName.Get("data", ns.NamespaceName));
                    if (dataElems != null)
                    {
                        var dataStr = dataElems.Value.Split(' ');
                        var dataValues = dataStr.Select(d => double.TryParse(d, out double value) ? value : 0.0).ToArray();

                        metricsData[key] = dataValues;
                        Response.Write(metricsData[key].Length);
                    }

                }
            }

            return new Tuple<Dictionary<string, double[]>, DateTime[]>(metricsData, timestamps.ToArray());
        }

        private void SendUsageDataToClient(Dictionary<string, double[]> metricsData, DateTime[] timestamps)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("let dates = [" + string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-ddTHH:mm:ss}\"")) + "];");
            string parameters = "";
            foreach (var metric in metricsData)
            {

                string key = metric.Key.Replace("|", "_");
                parameters +=key+"Data ,";
                string dataArray = string.Join(",", metric.Value.Select(v => v.ToString("F2")));
                scriptBuilder.AppendLine($"let {key}Data = [{dataArray}];");
            }
            parameters.Substring(0, parameters.Length - 2);
            scriptBuilder.AppendLine("fetchData("+parameters+");");

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }

        private void DisplayMessage(string message)
        {
            Label.InnerText = message;
        }
    }
}
