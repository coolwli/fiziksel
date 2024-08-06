using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace vminfo
{
    public partial class vmscreen : System.Web.UI.Page
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private readonly string vRopsServer = "https://ptekvrops01.fw.garanti.com.tr";
        private readonly string _username = ConfigurationManager.AppSettings["VropsUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["VropsPassword"];
        private string hostName;
        private string _token;
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        protected async void Page_Load(object sender, EventArgs e)
        {
            hostName = Request.QueryString["id"];
            if (string.IsNullOrEmpty(hostName))
            {
                DisplayHostNameError();
                return;
            }

            // Sertifika doğrulama işlemini güncelleyin
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            if (!IsPostBack)
            {
                ShowHost();
                await EnsureTokenAsync();
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Sertifika doğrulama işlemini burada düzgün yapılandırın
            return sslPolicyErrors == SslPolicyErrors.None;
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
            var requestBody = new { username = _username, password = _password };

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
                // Log the exception and display a user-friendly message
                Response.Write($"Error acquiring token: {ex.Message}");
            }
        }

        private void ShowHost()
        {
            string query = "SELECT * FROM vminfoVMs WHERE VMName = @name";
            using (var connection = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True"))
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
            double usedStorage = Convert.ToDouble(reader["VMUsedStorage"]);
            double totalStorage = Convert.ToDouble(reader["VMTotalStorage"]);
            usedDisk.InnerHtml = $"Used Disk: <strong>{usedStorage}GB </strong>";
            freeDisk.InnerHtml = $"Free Disk: <strong>{(totalStorage - usedStorage):F2}GB </strong>";
            totalDisk.InnerText = $"Total Disk {totalStorage} GB";
            usedPercentage.Style["width"] = $"{(usedStorage / totalStorage * 100):F2}%";
            usedBar.InnerText = $"{(usedStorage / totalStorage * 100):F2}%";
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

            for (int i = 0; i < rowCount; i++)
            {
                var row = new HtmlTableRow();
                foreach (var dataColumn in columnData)
                {
                    row.Cells.Add(new HtmlTableCell { InnerText = dataColumn[i] });
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
                    // Define metric patterns
                    string[] patterns = { "guestfilesystem:", "cpu|usage_average", "mem|usage_average" };
                    var (usageData, timestamps) = await GetUsageDataAsync(vmId, patterns);
                    SendUsageDataToClient(usageData, timestamps);
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

        private async Task<(Dictionary<string, double[]>, DateTime[])> GetUsageDataAsync(string vmId, params string[] patterns)
        {
            long startTimeMillis = new DateTimeOffset(DateTime.UtcNow.AddDays(-365)).ToUnixTimeMilliseconds();
            long endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string metricsUrl = $"{vRopsServer}/suite-api/api/resources/{vmId}/stats?begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=5&intervalType=MINUTES&rollUpType=AVG";

            return await GetMetricsDataAsync(metricsUrl, patterns);
        }

        private async Task<(Dictionary<string, double[]>, DateTime[])> GetMetricsDataAsync(string url, params string[] patterns)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

                var response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseXml = await response.Content.ReadAsStringAsync();
                return ParseUsageData(responseXml, patterns);
            }
            catch (Exception ex)
            {
                // Log the exception and throw it to be handled upstream
                throw new InvalidOperationException("Error fetching metrics data", ex);
            }
        }

        private (Dictionary<string, double[]>, DateTime[]) ParseUsageData(string xmlData, params string[] patterns)
        {
            var xdoc = XDocument.Parse(xmlData);
            var statsElements = xdoc.Descendants(XName.Get("stat", OpsNamespace));

            var dataDictionary = new Dictionary<string, double[]>();
            var timestamps = new List<DateTime>();

            foreach (var stat in statsElements)
            {
                var statKey = stat.Attribute("key")?.Value;
                if (statKey == null || !patterns.Any(pattern => statKey.Contains(pattern)))
                    continue;

                if (statKey.Contains("guestfilesystem:") && statKey.EndsWith(":percentage"))
                {
                    var dataXml = stat.Element(XName.Get("data", OpsNamespace))?.Value
                        .Split(' ')
                        .Select(double.Parse)
                        .ToArray();

                    if (dataXml != null)
                    {
                        dataDictionary[statKey] = dataXml;
                    }
                }

                // Handle timestamps if available
                var timeStampsXml = stat.Element(XName.Get("timestamps", OpsNamespace))?.Value
                    .Split(' ')
                    .Select(t => DateTime.Parse(t))
                    .ToArray();

                if (timeStampsXml != null)
                {
                    timestamps.AddRange(timeStampsXml);
                }
            }

            return (dataDictionary, timestamps.Distinct().ToArray()); // Distinct to remove duplicates
        }

        private async Task<string> PostApiDataAsync(string url, object requestBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{vRopsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";
            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();

            var xmlDoc = XDocument.Parse(responseText);
            var nsManager = new XmlNamespaceManager(xmlDoc.CreateReader().NameTable);
            nsManager.AddNamespace("ops", OpsNamespace);
            var identifierNode = xmlDoc.XPathSelectElement("//ops:resource/@identifier", nsManager);
            return identifierNode?.Value;
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

            if (timestamps.Length > 0)
            {
                scripts.Add($"let dates = [{dateArray}];");
            }

            scripts.Add("fetchData();");

            string script = string.Join("\n", scripts);

            ClientScript.RegisterStartupScript(this.GetType(), "usageDataScript", script, true);
        }
    }
}
