using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YourNamespace
{
    public partial class Default : System.Web.UI.Page
    {
        private const string VROPS_URL = "https://vrops.example.com/suite-api/api/auth/token";
        private const string USERNAME = "your_username";
        private const string PASSWORD = "your_password";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void GetTokenButton_Click(object sender, EventArgs e)
        {
            try
            {
                string token = await GetTokenAsync();
                TokenLabel.Text = "Token: " + token;
            }
            catch (Exception ex)
            {
                TokenLabel.Text = "Error: " + ex.Message;
            }
        }

        private async Task<string> GetTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", USERNAME),
                    new KeyValuePair<string, string>("password", PASSWORD),
                    new KeyValuePair<string, string>("grant_type", "password")
                });

                var response = await client.PostAsync(VROPS_URL, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent; // JSON response
                }
                else
                {
                    throw new HttpRequestException($"Token retrieval failed. Status Code: {response.StatusCode}");
                }
            }
        }
    }
}
