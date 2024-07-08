using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // NuGet package for JSON handling

namespace YourNamespace
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void btnFetchData_Click(object sender, EventArgs e)
        {
            string vropsServer = "https://<vrops-server>";
            string endpoint = "/suite-api/api/resources";
            string username = "your-username";
            string password = "your-password";

            string base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(vropsServer);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    litResult.Text = json.ToString();
                }
                else
                {
                    litResult.Text = $"Error: {response.StatusCode}";
                }
            }
        }
    }
}
