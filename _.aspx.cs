using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Vmpedia
{
    public class TokenManager
    {
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
            var vropsServer = GetVropsServer(vcenter);
            var tokenType = GetTokenType(vcenter);

            return await CheckTokenAsync(vropsServer, tokenType);
        }

        private string GetVropsServer(string vcenter)
        {
            return vcenter switch
            {
                "apgaraavcs801" or "apgartksvcs801" or "ptekvcsd01" => "https://apgaravrops801.fw.garanti.com.tr",
                _ => "https://ptekvrops01.fw.garanti.com.tr",
            };
        }

        private string GetTokenType(string vcenter)
        {
            return vcenter switch
            {
                "apgaraavcs801" or "apgartksvcs801" => "ankara",
                _ => "pendik",
            };
        }

        private async Task<string> CheckTokenAsync(string vropsServer, string tokenType)
        {
            var tokenInfo = await ReadTokenInfoFromDatabaseAsync(tokenType);

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
                    await StoreTokenInfoToDatabaseAsync(tokenType, newTokenInfo);
                    return newToken;
                }

                return null;
            }

            // Extend the existing token's lifetime
            var extendedTokenInfo = new TokenInfo
            {
                Token = tokenInfo.Token,
                ExpiryDate = DateTime.Now.Add(TokenLifetime)
            };
            await StoreTokenInfoToDatabaseAsync(tokenType, extendedTokenInfo);

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

        private async Task<TokenInfo> ReadTokenInfoFromDatabaseAsync(string tokenType)
        {
            var connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

            using var connection = new SqlConnection(connectionString);
            var query = "SELECT Token, ExpiryDate FROM TokenInfo WHERE TokenType = @TokenType";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TokenType", tokenType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TokenInfo
                {
                    Token = reader["Token"] as string,
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate"))
                };
            }

            return new TokenInfo { Token = null, ExpiryDate = DateTime.MinValue };
        }

        private async Task StoreTokenInfoToDatabaseAsync(string tokenType, TokenInfo tokenInfo)
        {
            var connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

            using var connection = new SqlConnection(connectionString);
            var query = @"
                IF EXISTS (SELECT 1 FROM TokenInfo WHERE TokenType = @TokenType)
                BEGIN
                    UPDATE TokenInfo
                    SET Token = @Token, ExpiryDate = @ExpiryDate
                    WHERE TokenType = @TokenType
                END
                ELSE
                BEGIN
                    INSERT INTO TokenInfo (TokenType, Token, ExpiryDate)
                    VALUES (@TokenType, @Token, @ExpiryDate)
                END";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TokenType", tokenType);
            command.Parameters.AddWithValue("@Token", (object)tokenInfo.Token ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", tokenInfo.ExpiryDate);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
