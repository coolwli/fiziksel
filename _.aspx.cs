using System;
using System.Net;
using System.Xml;
using System.Text;
using System.Linq;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Net.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Collections.Generic;

namespace vminfo
{
    public partial class vmscreen : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string vRopsServer= "https://ptekvrops01.fw.garanti.com.tr";
        private string hostName;
        public string _token;
        private readonly string _username = ConfigurationManager.AppSettings["VropsUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["VropsPassword"];
        private readonly string[] _metricsToFilter = { "cpu|usage_average", "mem|usage_average" };


        protected async void Page_Load(object sender, EventArgs e)
        {
            hostName = Request.QueryString["id"];
            if (string.IsNullOrEmpty(hostName))
            {
                DisplayHostNameError();
                return;
            }
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;


            if (!IsPostBack)
            {
                ShowHost();
                await EnsureTokenAsync();
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; 
        }

        private void DisplayHostNameError()
        {
            form1.InnerHtml = "";
            Response.Write("ID Bulunamadı");
        }

        private async Task EnsureTokenAsync()
        {
            Session["Token"] = "f267bd72-f77d-43ee-809c-c081d9a62dbe::a4ccbe4a-e9b6-4e01-92dd-0930cb99e2ac";
            //if (Session["Token"] == null || Session["TokenExpiry"] == null || DateTime.UtcNow >= (DateTime)Session["TokenExpiry"])
            if (Session["Token"] == null)
            {
                await AcquireTokenAsync();
            }
            else
            {
                _token = Session["Token"].ToString();
                await FetchVmUsageDataAsync();


            }
        }

        private async Task AcquireTokenAsync()
        {
            string apiUrl = $"{vRopsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            string requestBody = $"{{ \"username\": \"{_username}\", \"password\": \"{_password}\" }}";

            try
            {
                string tokenXml = await PostApiDataAsync(apiUrl, requestBody);
                _token = ExtractTokenFromXml(tokenXml);
                if (string.IsNullOrEmpty(_token))
                {
                    Response.Write("Error: Token is empty");
                }
                else
                {
                    Session["Token"] = _token;
                    Session["TokenExpiry"] = DateTime.UtcNow.AddMinutes(300); 
                    await FetchVmUsageDataAsync();
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error acquiring token: {ex.Message}");
            }
        }

        private void ShowHost()
        {
            string query = "SELECT * FROM vminfoVMs WHERE VMName = @name";
            using (var connection = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True"))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", hostName);
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
            usedDisk.InnerHtml = "Used Disk: <strong>" + reader["VMUsedStorage"].ToString() + "GB </strong >";
            freeDisk.InnerHtml = "Free Disk: <strong>" + (Convert.ToDouble(reader["VMTotalStorage"]) - Convert.ToDouble(reader["VMUsedStorage"])).ToString("F2") + "GB </strong >";
            totalDisk.InnerText = "Total Disk " + reader["VMTotalStorage"].ToString() + " GB";
            usedPercentage.Style["width"] = ((Convert.ToDouble(reader["VMUsedStorage"])) / (Convert.ToDouble(reader["VMTotalStorage"])) * 100).ToString() + "%";
            usedBar.InnerText = ((Convert.ToDouble(reader["VMUsedStorage"])) / (Convert.ToDouble(reader["VMTotalStorage"])) * 100).ToString("F2") + "%";
            lasttime.InnerText = reader["LastWriteTime"].ToString();
            hostmodel.InnerText = reader["VMHostModel"].ToString();
            os.InnerText = reader["VMGuestOS"].ToString();
            createdDate.InnerText = reader["VMCreatedDate"].ToString();

            PopulateTable(eventsTable, reader["VMEventDates"].ToString(), reader["VMEventMessages"].ToString());
            PopulateTable(networkTable, reader["VMNetworkAdapterName"].ToString(), reader["VMNetworkName"].ToString(), reader["VMNetworkAdapterMacAddress"].ToString(), reader["VMNetworkCardType"].ToString());
            PopulateTable(disksTable, reader["VMDiskName"].ToString(), reader["VMDiskTotalSize"].ToString(), reader["VMDiskStorageFormat"].ToString(), reader["VMDiskDataStore"].ToString());
        }

        private void PopulateTable(HtmlTable table, params string[] columns)
        {
            var columnData = columns.Select(c => c.Split('~')).ToArray();
            int rowCount = columnData[0].Length;

            for (int i = 0; i < rowCount - 1; i++)
            {
                var row = new HtmlTableRow();
                for (int j = 0; j < columnData.Length; j++)
                {
                    row.Cells.Add(new HtmlTableCell { InnerText = columnData[j][i] });
                }
                table.Rows.Add(row);
            }
        }

        private async Task FetchVmUsageDataAsync()
        {
            try
            {
                string vmId = await GetVmIdAsync(hostName);
                if (!string.IsNullOrEmpty(vmId))
                {
                    string metricsData = await GetAllMetricsDataAsync(vmId);
                    var parsedData = ParseMetricsData(metricsData);
                    SendUsageDataToClient(parsedData.Item1, parsedData.Item2);
                }
                else
                {
                    Response.Write("VM ID not found.");
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error fetching VM data: {ex.Message}");
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
            string getIdUrl = $"{vRopsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";
            var request = (HttpWebRequest)WebRequest.Create(getIdUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
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

        private string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;
        }
        private async Task<string> GetAllMetricsDataAsync(string vmId)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-30)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string statsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            var request = (HttpWebRequest)WebRequest.Create(statsUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
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
                              .Select(ts => long.TryParse(ts, out long result)
                              ? DateTimeOffset.FromUnixTimeMilliseconds(result).DateTime
                              : DateTime.MinValue)
                              .Where(t => t != DateTime.MinValue)
                              .ToList();
            }

            var stats = xmlDoc.Descendants(XName.Get("stat", ns.NamespaceName));

            foreach (var stat in stats)
            {
                var key = stat.Element(XName.Get("statKey", ns.NamespaceName))
                              .Element(XName.Get("key", ns.NamespaceName))
                              .Value;

                if (key.StartsWith("guestfilesystem") && key.EndsWith("|percentage") || _metricsToFilter.Contains(key))
                {
                    var dataElems = stat.Element(XName.Get("data", ns.NamespaceName));
                    if (dataElems != null)
                    {
                        var dataValues = dataElems.Value.Split(' ')
                                       .Select(d => double.TryParse(d, out double value) ? value : 0.0)
                                       .ToArray();

                        int minLength = Math.Min(timestamps.Count, dataValues.Length);
                        if (timestamps.Count > dataValues.Length)
                        {
                            // Truncate timestamps to match dataValues length
                            timestamps = timestamps.Skip(timestamps.Count - dataValues.Length).ToList();
                        }
                        else if (dataValues.Length > timestamps.Count)
                        {
                            // Truncate dataValues to match timestamps length
                            dataValues = dataValues.Skip(dataValues.Length - timestamps.Count).ToArray();
                        }

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

    }
}
