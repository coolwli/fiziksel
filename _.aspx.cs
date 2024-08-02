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

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
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
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString))
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
                    SendUsageDataToClient(usageData.Item1, usageData.Item2);
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

        private async Task<Tuple<double[], double[]>> GetUsageDataAsync(string vmId)
        {
            string getStatsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats";
            var request = (HttpWebRequest)WebRequest.Create(getStatsUrl);
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

                var cpuValues = xmlDoc.SelectNodes("//ops:stat[@name='cpu|usage_average']/ops:data/ops:datum", nsManager)
                    .Cast<XmlNode>()
                    .Select(node => Convert.ToDouble(node.InnerText))
                    .ToArray();

                var memoryValues = xmlDoc.SelectNodes("//ops:stat[@name='mem|usage_average']/ops:data/ops:datum", nsManager)
                    .Cast<XmlNode>()
                    .Select(node => Convert.ToDouble(node.InnerText))
                    .ToArray();

                return new Tuple<double[], double[]>(cpuValues, memoryValues);
            }
        }

        private void SendUsageDataToClient(double[] cpuValues, double[] memoryValues)
        {
            var usageData = new
            {
                CpuValues = cpuValues,
                MemoryValues = memoryValues
            };
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(usageData);
            ScriptManager.RegisterStartupScript(this, GetType(), "sendData", $"window.chartData = {jsonData};", true);
        }

        private string ExtractTokenFromXml(string tokenXml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(tokenXml);
            var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ops", OpsNamespace);
            var tokenNode = xmlDoc.SelectSingleNode("//ops:token", nsManager);
            return tokenNode?.InnerText;
        }
    }
}
