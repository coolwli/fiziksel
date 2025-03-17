using System;
using System.Net;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using System.Net.Http;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Collections.Generic;
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

            await CheckTokenAndFetchDataAsync("https://ptekvrops01.fw.garanti.com.tr/suite-api/internal/views/51f11b22-4019-45db-b5a5-512b40b0f130/data/export?resourceId=00330e14-5263-4728-8273-a135ae4d22fa&pageSize=9000&traversalSpec=vSphere Hosts and Clusters-VMWARE-vSphere World&_ack=true", "ptekvcs01");
       
        }

        private async Task CheckTokenAndFetchDataAsync(string url, string tokenType)
        {
            var tokenManager = new TokenManager(_httpClient);
            try
            {
                _token = await tokenManager.GetTokenAsync(tokenType);
                string jsonData = await GetDataAsync(url);
                ParseDashboardData(jsonData);

            }
            catch(Exception ex){
                form1.InnerHtml = ex.ToString();
            }
        }

        private async Task<string> GetDataAsync(string url)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            return null;
        }

        private void ParseDashboardData(string jsonData)
        {
            var js = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            dynamic data = js.Deserialize<dynamic>(jsonData);
            var tableRows = new List<Dictionary<string, object>>();

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
                                if (new string[] { "grandTotal", "groupUUID", "objUUID", "summary" }.Any(s=> kv.Key.ToString().Contains(s))) continue;
                                if(kv.Key=="12")
                                {
                                    tableRow[kv.Key] = string.IsNullOrWhiteSpace(kv.Value?.ToString()) ? "" : DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(kv.Value.ToString())).DateTime.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    tableRow[kv.Key] = string.IsNullOrWhiteSpace(kv.Value?.ToString()) ? "" : kv.Value;
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

            if (!string.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                var tableData = js.Deserialize<List<Dictionary<string, object>>>(jsonData);

                if (tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Name;Power State;IPv4;OS;Cluster;VCenter;Datastore Cluster;Include Snapshot;Snapshot Size");

                foreach (var row in tableData)
                {
                    csv.AppendLine($"{row["name"]};{row["ps"]};{row["ip"]};{row["os"]};{row["cl"]};{row["vc"]};{row["ds"]};{row["sb"]};{row["ss"]}");
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=Replicated VMs.csv");
                Response.Write(csv.ToString());
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }



    }
}
