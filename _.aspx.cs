using System;
using System.Collections.Generic;
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
        private const string TokenFileName = "tokens.txt"; 
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
                return await CheckTokenAsync("https://apgaravrops801.fw.garanti.com.tr", "ankara"); ;
            }
            return await CheckTokenAsync("https://ptekvrops01.fw.garanti.com.tr", "pendik");
        }

        private async Task<string> CheckTokenAsync(string vropsServer, string tokenType)
        {
            string tokenFilePath = GetFilePath();
            var tokenInfo = ReadTokenInfoFromFile(tokenFilePath, tokenType);

            if (DateTime.Now >= tokenInfo.ExpiryDate)
            {
                string newToken = await AcquireTokenAsync(vropsServer);

                if (newToken != null)
                {
                    var newTokenInfo = new TokenInfo
                    {
                        Token = newToken,
                        ExpiryDate = DateTime.Now.Add(TokenLifetime)
                    };
                    StoreTokenInfo(tokenFilePath, tokenType, newTokenInfo);
                    return newToken;
                }

                return null;
            }
            tokenInfo = ReadTokenInfoFromFile(tokenFilePath, tokenType);
            return tokenInfo.Token;
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

        private TokenInfo ReadTokenInfoFromFile(string filePath, string tokenType)
        {
            if (!File.Exists(filePath))
            {
                return new TokenInfo { Token = null, ExpiryDate = DateTime.MinValue };
            }

            var lines = File.ReadAllLines(filePath);
            var tokenLine = lines.FirstOrDefault(line => line.StartsWith($"{tokenType}:Token="));
            var expiryLine = lines.FirstOrDefault(line => line.StartsWith($"{tokenType}:Expiry="));

            string token = tokenLine?.Substring(tokenLine.IndexOf('=') + 1);
            DateTime expiryDate = DateTime.TryParse(expiryLine?.Substring(expiryLine.IndexOf('=') + 1), out DateTime result) ? result : DateTime.MinValue;

            return new TokenInfo { Token = token, ExpiryDate = expiryDate };
        }

        private void StoreTokenInfo(string filePath, string tokenType, TokenInfo tokenInfo)
        {
            var lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

            var tokenLine = $"{tokenType}:Token={tokenInfo.Token}";
            var expiryLine = $"{tokenType}:Expiry={tokenInfo.ExpiryDate:o}";

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

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

 'The process cannot access the file 'C:\inetpub\wwwroot\vminfo\vminfo\tokens.txt' because it is being used by another process.'
