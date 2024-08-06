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
        private string vRopsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private string hostName;
        public string _token;
        private readonly string _username = ConfigurationManager.AppSettings["VropsUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["VropsPassword"];

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
            if (Session["Token"] == null || Session["TokenExpiry"] == null || DateTime.UtcNow >= (DateTime)Session["TokenExpiry"])
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
                    // Belirli metrik desenlerini belirleyin
                    string[] patterns = { "guestfilesystem:", "cpu|usage_average", "mem|usage_average" };

                    var usageData = await GetUsageDataAsync(vmId, patterns);
                    SendUsageDataToClient(usageData, new DateTime[0]); // Eğer timestamps gerekli değilse boş bir dizi gönderebilirsiniz
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

        private async Task<Dictionary<string, double[]>> GetUsageDataAsync(string vmId, params string[] patterns)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            // Tek URL ile tüm statları çekme
            string metricsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            var metricsData = await GetMetricsDataAsync(metricsUrl, patterns);
            return metricsData;
        }

        private async Task<Dictionary<string, double[]>> GetMetricsDataAsync(string url, params string[] patterns)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseXml = await reader.ReadToEndAsync();
                return ParseUsageData(responseXml, patterns);
            }
        }

        private Dictionary<string, double[]> ParseUsageData(string xmlData, params string[] patterns)
        {
            var xdoc = XDocument.Parse(xmlData);
            var statsElements = xdoc.Descendants(XName.Get("stat", OpsNamespace));

            var dataDictionary = new Dictionary<string, double[]>();

            foreach (var stat in statsElements)
            {
                var statKey = stat.Attribute("key")?.Value;
                if (statKey == null || !patterns.Any(pattern => statKey.Contains(pattern)))
                    continue;

                var dataXml = stat.Element(XName.Get("data", OpsNamespace))?.Value.Split(' ').Select(double.Parse).ToArray();
                
                if (dataXml != null)
                {
                    dataDictionary[statKey] = dataXml;
                }
            }

            return dataDictionary;
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

        private void SendUsageDataToClient(Dictionary<string, double[]> metricsData, DateTime[] timestamps)
        {
            var dataStrings = metricsData.ToDictionary(
                kvp => kvp.Key,
                kvp => string.Join(",", kvp.Value.Select(v => v.ToString("F2")))
            );
            var dateArray = string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-ddTHH:mm:ss}\""));

            var scripts = dataStrings.Select(kvp => 
                $"let {kvp.Key}Datas = [{kvp.Value}];"
            ).ToList();
            
            scripts.Add($"let dates = [{dateArray}];");
            scripts.Add("fetchData();");

            string script = string.Join("\n", scripts);

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", script, true);
        }
    }
}
