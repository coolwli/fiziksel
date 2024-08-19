using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Configuration;

namespace vminfo
{
    public class TokenManager
    {
        private const string OpsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(3);

        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;

        public TokenManager(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _username = WebConfigurationManager.AppSettings["VropsUsername"]
                        ?? throw new ArgumentNullException(nameof(_username));
            _password = WebConfigurationManager.AppSettings["VropsPassword"]
                        ?? throw new ArgumentNullException(nameof(_password));
        }

        public async Task<string> GetTokenAsync(string vcenter)
        {
            if (vcenter == "apgaraavcs801" || vcenter == "apgartksvcs801" || vcenter == "ptekvcsd01")
            {
                return null;
                //https://apgaravrops801.fw.garanti.com.tr
            }
            return await CheckTokenAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
        }

        private async Task<string> CheckTokenAsync(string vropsServer, string tokenType)
        {
            string tokenKey = $"{tokenType}Token";
            string expiryKey = $"{tokenType}TokenExpiry";

            DateTime tokenExpiry = GetTokenExpiry(expiryKey);

            if (DateTime.Now >= tokenExpiry)
            {
                string token = await AcquireTokenAsync(vropsServer, tokenType);          

                if (token != null)
                {
                    StoreToken(token, tokenKey);
                    StoreTokenExpiry(DateTime.Now.Add(TokenLifetime), expiryKey);
                }
                return "token";
            }
            else
            {
                return WebConfigurationManager.AppSettings[tokenKey];
            }
        }

        private async Task<string> AcquireTokenAsync(string vropsServer, string tokenType)
        {
            var requestUri = $"{vropsServer}/suite-api/api/auth/token/acquire?_no_links=true";
            var requestBody = new
            {
                username = _username,
                password = _password
            };
            var jsonContent = new JavaScriptSerializer().Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                string tokenXml= await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(tokenXml))
                {
                    return "null";
                }
                return ExtractTokenFromXml(tokenXml);
            }

            return "401";
        }

        private static string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", OpsNamespace)).FirstOrDefault();
            return tokenElement?.Value;

        }

        private DateTime GetTokenExpiry(string expiryKey)
        {
            string expiryStr = WebConfigurationManager.AppSettings[expiryKey];
            if (DateTime.TryParse(expiryStr, out DateTime expiryDate))
            {
                return expiryDate;
            }
            return DateTime.MinValue;
        }

        private void StoreToken(string token, string tokenKey)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = config.AppSettings;
            if (appSettings.Settings[tokenKey] != null)
            {
                appSettings.Settings[tokenKey].Value = token;
            }
            else
            {
                appSettings.Settings.Add(tokenKey, token);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void StoreTokenExpiry(DateTime expiryDate, string expiryKey)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = config.AppSettings;
            if (appSettings.Settings[expiryKey] != null)
            {
                appSettings.Settings[expiryKey].Value = expiryDate.ToString("o");
            }
            else
            {
                appSettings.Settings.Add(expiryKey, expiryDate.ToString("o"));
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}


TokenManager tokenManager = new TokenManager(_httpClient);
_token = await tokenManager.GetTokenAsync(vcenter.InnerText);
