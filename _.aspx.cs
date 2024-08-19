using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace vminfo
{
    public class TokenManager
    {
        private const string TokenFileName = "tokens.txt"; // Tek bir metin dosyası adı
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
            }
            return await CheckTokenAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
        }

        private async Task<string> CheckTokenAsync(string vropsServer, string tokenType)
        {
            string tokenFilePath = GetFilePath();
            (string token, DateTime expiryDate) = ReadTokenAndExpiryFromFile(tokenFilePath, tokenType);

            if (DateTime.Now >= expiryDate)
            {
                string newToken = await AcquireTokenAsync(vropsServer);

                if (newToken != null)
                {
                    StoreTokenAndExpiry(tokenFilePath, tokenType, newToken, DateTime.Now.Add(TokenLifetime));
                    return newToken;
                }

                return null;
            }

            return token;
        }

        private async Task<string> AcquireTokenAsync(string vropsServer)
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
                string tokenXml = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(tokenXml) ? null : ExtractTokenFromXml(tokenXml);
            }

            return null;
        }

        private static string ExtractTokenFromXml(string xmlData)
        {
            var xdoc = XDocument.Parse(xmlData);
            var tokenElement = xdoc.Descendants(XName.Get("token", "http://webservice.vmware.com/vRealizeOpsMgr/1.0/")).FirstOrDefault();
            return tokenElement?.Value;
        }

        private (string token, DateTime expiryDate) ReadTokenAndExpiryFromFile(string filePath, string tokenType)
        {
            if (!File.Exists(filePath))
            {
                return (null, DateTime.MinValue);
            }

            var lines = File.ReadAllLines(filePath);
            var tokenLine = lines.FirstOrDefault(line => line.StartsWith($"{tokenType}:Token="));
            var expiryLine = lines.FirstOrDefault(line => line.StartsWith($"{tokenType}:Expiry="));

            string token = tokenLine?.Substring(tokenLine.IndexOf('=') + 1);
            DateTime expiryDate = DateTime.TryParse(expiryLine?.Substring(expiryLine.IndexOf('=') + 1), out DateTime result) ? result : DateTime.MinValue;

            return (token, expiryDate);
        }

        private void StoreTokenAndExpiry(string filePath, string tokenType, string token, DateTime expiryDate)
        {
            var lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

            var tokenLine = $"{tokenType}:Token={token}";
            var expiryLine = $"{tokenType}:Expiry={expiryDate:o}";

            lines.RemoveAll(line => line.StartsWith($"{tokenType}:Token=") || line.StartsWith($"{tokenType}:Expiry="));
            lines.Add(tokenLine);
            lines.Add(expiryLine);

            File.WriteAllLines(filePath, lines);
        }

        private string GetFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TokenFileName);
        }
    }
}
