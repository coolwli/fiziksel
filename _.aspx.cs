
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

namespace vminfo
{
    public partial class vmscreen : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private SqlConnection con;
        private string hostName;
        private string vRopsServer;
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

            string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";
            con = new SqlConnection(connectionString);

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
            //_token = "f267bd72-f77d-43ee-809c-c081d9a62dbe::3f0521a0-45e5-433f-ad0f-0afe4c65861c";
            if (Session["Token"] == null || Session["TokenExpiry"] == null || DateTime.UtcNow >= (DateTime)Session["TokenExpiry"])
            //if (_token=="")
            {
                Response.Write("yok");
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
                    Session["TokenExpiry"] = DateTime.UtcNow.AddMinutes(30); 
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
            con.Open();
            using (var command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@name", hostName);
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
            con.Close();
        }

        private void PopulateDetails(SqlDataReader reader)
        {
            vcenter.InnerText = reader["vCenter"].ToString();
            //vRopsServer = reader["vCenter"].ToString() == "ptekvcs01"?  "https://ptekvrops01.fw.garanti.com.tr": "https://ptekvrops01.fw.garanti.com.tr";
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
                    var usageData = await GetUsageDataAsync(vmId);
                    SendUsageDataToClient(usageData.Item1, usageData.Item2, usageData.Item3);
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

        private async Task<Tuple<double[], double[], DateTime[]>> GetUsageDataAsync(string vmId)
        {
            DateTime startTime = DateTime.UtcNow.AddDays(-365);
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
            return $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?statKey={statKey}&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";
        }

        private async Task<Tuple<double[], DateTime[]>> GetMetricsDataAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = $"vRealizeOpsToken {_token}";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseXml = await reader.ReadToEndAsync();
                return ParseUsageData(responseXml);
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
            string cpuUsageArray = string.Join(",", cpuUsage.Select(u => u.ToString("F2")));
            string memoryUsageArray = string.Join(",", memoryUsage.Select(u => u.ToString("F2")));
            string dateArray = string.Join(",", timestamps.Select(t => $"\"{t:yyyy-MM-ddTHH:mm:ss}\""));
            string script = $@"
                let dates = [{dateArray}];
                let cpuDatas = [{cpuUsageArray}];
                let memoryDatas = [{memoryUsageArray}];
                fetchData();";

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", script, true);
        }

    }
}
