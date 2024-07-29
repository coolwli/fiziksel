using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.WebSockets;
using System.Web.UI;
using System.Xml.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;

namespace blank_page
{

    public partial class _default : System.Web.UI.Page
    {
        string vropsServer = "https://ptekvrops01.fw.garanti.com.tr";
        string token ="";
        protected void Page_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(validateServerCertificate);
        }
        private static bool validateServerCertificate(Object sender,X509Certificate certificate,X509Chain chain,SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            if (token=="")
            {
                string apiUrl = "https://ptekvrops01.fw.garanti.com.tr/suite-api/api/auth/token/acquire?_no_links=true";
                string requestBody = "{ \"username\": \"admin\", \"password\": \"VMware123!\" }"; 

                try
                {
                    string tokenXml = await getApiToken(apiUrl, requestBody);
                    XDocument xdoc = XDocument.Parse(tokenXml);

                    XNamespace opsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";

                    XElement tokenElement = xdoc.Descendants(opsNamespace + "token").FirstOrDefault();

                    if (tokenElement != null)
                    {
                        token = tokenElement.Value;
                        Label.InnerText="Token: " + token; 
                    }
                    else
                    {
                        Label.InnerText="Token bulunamadı.";
                    }
                    Button1_Click(sender, e);


                }
                catch (Exception ex)
                {
                    Label.InnerText = "Error1: " + ex.Message;
                }
            }
            else 
            {
                try
                {
                    string result = await getVMID("tekscr1", token);
                    label2.InnerText = result;
                }
                catch (Exception ex)
                {
                    label2.InnerText = "Error2: " + ex.Message;

                }
            }
            
        }

        private async Task<string> getApiToken(string url, string requestBody)
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

        private async Task<string> getVMID(string vmName, string token)
        {
            
            string getId = $"https://ptekvrops01.fw.garanti.com.tr/suite-api/api/resources?resourceKind?=VirtualMachine&name=tekscr1";


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getId);
            request.Method = "GET";
            request.Headers["Authorization"] = "vRealizeOpsToken " + token;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task<string> getUsageDatas(string vmId, string token)
        {
            DateTime startTime = DateTime.UtcNow.AddDays(-30);
            DateTime endTime = DateTime.UtcNow;

            DateTimeOffset startOffset = new DateTimeOffset(startTime);
            long startTimeMillis = startOffset.ToUnixTimeMilliseconds();

            DateTimeOffset endOffset = new DateTimeOffset(endTime);
            long endTimeMillis = endOffset.ToUnixTimeMilliseconds();

            string metricsUrl = $"{vropsServer}/suite-api/api/resources/{vmId}/stats?statKey=cpu|usage_average&begin={startTimeMillis}&end={endTimeMillis}&intervalQuantifier=3&intervalType=HOURS&rollUpType=AVG";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metricsUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = "vRealizeOpsToken " + token;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

    }
}
