using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Net.Http;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace vmpedia
{
    public partial class _default : System.Web.UI.Page
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private string _username;
        private string _password;
        private string _token;

        static _default()
        {
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            _username = WebConfigurationManager.AppSettings["VropsUsername"];
            _password = WebConfigurationManager.AppSettings["VropsPassword"];

            try
            {
                var jsonData = new StringBuilder();

                var urls = new List<string>
                {
                    "https://ptekvrops01.fw.garanti.com.tr/suite-api/internal/views/51f11b22-4019-45db-b5a5-512b40b0f130/data/export?resourceId=00330e14-5263-4728-8273-a135ae4d22fa&pageSize=1000&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true"//,
                    //"https://apgarvrops201.fw.garanti.com.tr/suite-api/internal/views/51f11b22-4019-45db-b5a5-512b40b0f130/data/export?resourceId=00330e14-5263-4728-8273-a135ae4d22fa&pageSize=1000&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true"
                };

                var tasks = new List<Task<string>>();

                foreach (var url in urls)
                {
                    tasks.Add(CheckTokenAndFetchDataAsync(url)); 
                }

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    jsonData.Append(result);
                }

                ParseDashboardData(jsonData.ToString());
            }
            catch (Exception ex)
            {
                form1.InnerHtml = "An error occurred while loading the page: " + ex.ToString();
            }
        }

        private async Task<string> CheckTokenAndFetchDataAsync(string url)
        {
            var tokenManager = new TokenManager(_httpClient);

            try
            {
                string tokenType = GetTokenTypeFromUrl(url);
                _token = await tokenManager.GetTokenAsync(tokenType);

                var tasks = new List<Task<string>>();
                for (int page = 0; page <= 2; page++)
                {
                    tasks.Add(GetDataAsync(url + $"&page={page}"));
                }

                var pageResults = await Task.WhenAll(tasks);
                var jsonDataBuilder = new StringBuilder();
                foreach (var pageData in pageResults)
                {
                    if (!string.IsNullOrEmpty(pageData))
                    {
                        jsonDataBuilder.Append(pageData);
                    }
                }

                return jsonDataBuilder.ToString();
            }
            catch (Exception ex)
            {
                form1.InnerHtml = "An error occurred: " + ex.ToString();
                return string.Empty;
            }
        }

        private string GetTokenTypeFromUrl(string url)
        {
            if (url.Contains("ptekvrops01"))
            {
                return "pendik"; 
            }
            else
            {
                return "ankara"; 
            }
        }

        private async Task<string> GetDataAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();

                    }
                    return string.Empty;
                }
            }
            catch (HttpRequestException)
            {
                return string.Empty;
            }
        }

        private void ParseDashboardData(string jsonData)
        {
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            dynamic data = js.Deserialize<dynamic>(jsonData);
            var tableRows = new List<Dictionary<string, object>>();

            var excludedKeys = new HashSet<string> { "grandTotal", "groupUUID", "objUUID", "summary" };

            foreach (var view in data)
            {
                foreach (var elements in view.Value)
                {
                    foreach (var rows in elements["rows"])
                    {
                        var tableRow = new Dictionary<string, object>();
                        foreach (var row in rows)
                        {
                            foreach (var kv in row.Value)
                            {
                                if (excludedKeys.Contains(kv.Key.ToString())) continue;

                                if (kv.Value == null)
                                {
                                    tableRow[kv.Key] = "";
                                }
                                else if (kv.Key == "12")
                                {
                                    if (long.TryParse(kv.Value.ToString(), out long unixTimestamp))
                                    {
                                        tableRow[kv.Key] = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime.ToString("dd/MM/yyyy");
                                    }
                                }
                                else
                                {
                                    tableRow[kv.Key] = kv.Value.ToString();
                                }
                            }
                        }
                        tableRows.Add(tableRow);
                    }
                }
            }

            var json = js.Serialize(tableRows);
            var script = $"<script>baseData = {json}; initializeTable(); </script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (string.IsNullOrEmpty(jsonData))
            {
                Response.Write("No data available.");
                return;
            }

            try
            {
                var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                var tableData = js.Deserialize<List<Dictionary<string, object>>>(jsonData);

                if (tableData == null || tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                var columnNames = new List<string> { "Name", "Power State", "IPv4", "OS", "Cluster", "VCenter", "Datastore Cluster", "Include Snapshot", "Snapshot Size" };

                var csv = new StringBuilder();
                csv.AppendLine(string.Join(";", columnNames));

                foreach (var row in tableData)
                {
                    var values = new List<string>
                    {
                        row["1"]?.ToString(),
                        row["2"]?.ToString(),
                        row["3"]?.ToString(),
                        row["4"]?.ToString(),
                        row["5"]?.ToString(),
                        row["6"]?.ToString(),
                        row["7"]?.ToString(),
                        row["8"]?.ToString(),
                        row["9"]?.ToString(),
                        row["10"]?.ToString(),
                        row["11"]?.ToString(),
                        row["12"]?.ToString(),
                        row["13"]?.ToString()
                    };

                    csv.AppendLine(string.Join(";", values));
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment;filename=Replicated_VMs.csv");
                Response.Write(csv.ToString());
            }
            catch (Exception ex)
            {
                Response.Write($"An error occurred: {ex.ToString()}");
            }
        }
    }
}
