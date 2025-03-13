using System;
using System.Net;
using System.Xml;
using System.Text;
using System.Linq;
using System.Data;
using System.Net.Http;
using System.Xml.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace vmpedia
{
    public partial class vmscreen : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string vRopsServer;
        private readonly string[] _initialMetricsToFilter = { "cpu|usage_average", "mem|usage_average" };
    
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private string hostName;
        private string uuid;
        private string vcenterName;
        private string _token;
        private List<string> _metricsToFilter;


        static vmscreen()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }
        protected async void Page_Load(object sender, EventArgs e)
        {
            hostName = Request.QueryString["id"];
            vcenterName = Request.QueryString["vc"];
            if (string.IsNullOrEmpty(hostName))
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

        private void ShowHost()
        {
            string query = "SELECT * FROM vminfoVMs WHERE VMName = @name AND vCenter = @vc";
            using (var connection = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True"))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", hostName);
                    command.Parameters.AddWithValue("@vc", vcenterName);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateDetails(reader);
                        }
                        else
                        {
                            DisplayHostNameError();
                        }
                    }
                }
            }
        }

        private void PopulateDetails(SqlDataReader reader)
        {
            vcenter.InnerText = reader["vCenter"].ToString();
            host.InnerText = reader["VMHost"].ToString();
            cluster.InnerText = reader["VMCluster"].ToString();
            datacenter.InnerText = reader["VMDataCenter"].ToString();
            power.InnerText = reader["VMPowerState"].ToString();
            notes.InnerText = reader["VMNotes"].ToString();
            numcpu.InnerText = reader["VMNumCPU"].ToString();
            totalmemory.InnerText = reader["VMMemoryCapacity"].ToString();
            usedDisk.InnerHtml = $"Used Disk: <strong>{reader["VMUsedStorage"]}GB</strong>";
            freeDisk.InnerHtml = $"Free Disk: <strong>{(Convert.ToDouble(reader["VMTotalStorage"]) - Convert.ToDouble(reader["VMUsedStorage"])).ToString("F2")}GB</strong>";
            totalDisk.InnerText = $"Total Disk {reader["VMTotalStorage"]} GB";
            double usedPercentageNum = Convert.ToDouble(reader["VMUsedStorage"]) / Convert.ToDouble(reader["VMTotalStorage"]) * 100;
            usedPercentage.Style["width"] = $"{usedPercentageNum}%";
            usedBar.InnerText = $"{usedPercentageNum.ToString("F2")}%";
            lasttime.InnerText = reader["LastWriteTime"].ToString();
            hostmodel.InnerText = reader["VMHostModel"].ToString();
            os.InnerText = reader["VMGuestOS"].ToString();
            createdDate.InnerText = reader["VMCreatedDate"].ToString();
            uuid = reader["VMEntityInstanceUUID"].ToString();
            PopulateTable(networkTable, reader["VMNetworkAdapterName"].ToString(), reader["VMNetworkName"].ToString(), reader["VMNetworkAdapterMacAddress"].ToString(), reader["VMNetworkCardType"].ToString());
            PopulateTable(disksTable, reader["VMDiskName"].ToString(), reader["VMDiskTotalSize"].ToString(), reader["VMDiskStorageFormat"].ToString(), reader["VMDiskDataStore"].ToString());
            PopulateTable(tagsTable, reader["VMTags"].ToString());
        }

        private void PopulateTable(HtmlTable table, params string[] columns)
        {
            var columnData = columns.Select(c => c.Split('~')).ToArray();
            int rowCount = columnData[0].Length;

            for (int i = 0; i < rowCount; i++)
            {
                if (columnData[0][i] != "")
                {
                    var row = new HtmlTableRow();
                    for (int j = 0; j < columnData.Length; j++)
                    {
                        row.Cells.Add(new HtmlTableCell { InnerText = columnData[j][i] });
                    }
                    table.Rows.Add(row);
                }
            }
        }

        private async Task EnsureTokenAsync()
        {
            vRopsServer = (vcenterName == "apgaraavcs801" || vcenterName == "apgartksvcs801" || vcenterName == "ptekvcsd01") ? "https://apgaravrops801.fw.garanti.com.tr" : "https://ptekvrops01.fw.garanti.com.tr";

            TokenManager tokenManager = new TokenManager(_httpClient);
            _token = await tokenManager.GetTokenAsync(vcenterName);
            if (string.IsNullOrEmpty(_token)) {
                Response.Write("Şu anda Performance & Disk verilerini görüntüleyemiyoruz lütfen daha sonra deneyiniz");
                return;
            }
            await FetchVmUsageDataAsync();

        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string vmId = await GetVmIdAsync(hostName);
                if (!string.IsNullOrEmpty(vmId))
                {
                    await FetchGuestFileSystemMetricsAsync(vmId);
                    var metricsData = await FetchMetricsAsync(vmId);
                    SendUsageDataToClient(metricsData.Item1, metricsData.Item2);
                }
                else
                {
                    Response.Write("VM ID not found.");
                }
            }
            catch
            {
                Response.Write("Bu VM özelinde performans verilerine bakılamamaktadır.");
            }
        }

        private async Task FetchGuestFileSystemMetricsAsync(string vmId)
        {
            string statsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats";
            var request = new HttpRequestMessage(HttpMethod.Get, statsUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            using (var response = await _httpClient.SendAsync(request))
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

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{vRopsServer}/suite-api/api/adapterkinds/VMWARE/resourcekinds/virtualmachine/resources?identifiers[VMEntityInstanceUUID]={uuid}";

            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

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

        private async Task<Tuple<Dictionary<string, double[]>, DateTime[]>> FetchMetricsAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.Now.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var metricsData = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            var fetchTasks = _metricsToFilter.Select(async metric =>
            {
                string metricsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?statKey={metric}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=10&intervalType=MINUTES&rollUpType=AVG";
                var data = await FetchMetricsDataAsync(metricsUrl);
                var parsedData = ParseMetricsData(data, metric, ref timestamps);
                if (parsedData != null)
                {
                    metricsData[metric] = parsedData;
                }
            });

            await Task.WhenAll(fetchTasks);

            return new Tuple<Dictionary<string, double[]>, DateTime[]>(metricsData, timestamps.ToArray());
        }

        private async Task<string> FetchMetricsDataAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
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
                                  ? DateTimeOffset.FromUnixTimeMilliseconds(result + 10800000).DateTime
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

            return null;
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

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", scriptBuilder.ToString(), true);
        }

        private void DisplayHostNameError()
        {
            form1.InnerHtml = "";
            Response.Write("VM Bulunamadı");
        }
    }

}
