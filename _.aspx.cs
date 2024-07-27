using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace blank_page
{

    public partial class _default : System.Web.UI.Page
    {
        string token = "";
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
            if (token == "")
            {
                string apiUrl = "https://ptekvrops01.fw.garanti.com.tr/suite-api/api/auth/token/acquire?_no_links=true";
                string requestBody = "{ \"username\": \"admin\", \"password\": \"VMware123!\" }"; 

                try
                {
                    string tokenXml = await getApiToken(apiUrl, requestBody);
                    Button1_Click(sender, e);
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

                }
                catch (Exception ex)
                {
                    Label.InnerText = "Error1: " + ex.Message;
                }
            }
            else 
            {
                string vmname = "testvm";
                string apiUrl = "";

                try
                {
                    string result = await getApiResponse(apiUrl, token);
                    Label.InnerText =result;
                }
                catch (Exception ex)
                {
                    Label.InnerText = "Error2: " + ex.Message;

                }
            }
            
        }

        private async Task<string> getVMID(string vmName, string token)
        {
            // vROps API'sine VM bilgilerini almak için istek yapacağımız URL
            string apiUrl = $"https://ptekvrops01.fw.garanti.com.tr/suite-api/api/resources?resourceName={vmName}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + token);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();                    
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
    }
}
