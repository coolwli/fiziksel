using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public partial class YourPage : System.Web.UI.Page
{
    protected async void Page_Load(object sender, EventArgs e)
    {
        string vropsUrl = "https://<vrops-server>/suite-api/api/resources";
        string username = "your-username";
        string password = "your-password";
        string vmName = "name-of-your-vm";

        try
        {
            string resourceId = await GetResourceIdAsync(vropsUrl, username, password, vmName);
            Response.Write($"Resource ID: {resourceId}");
        }
        catch (Exception ex)
        {
            Response.Write($"Error: {ex.Message}");
        }
    }

    private async Task<string> GetResourceIdAsync(string url, string username, string password, string vmName)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Basic authentication
            var byteArray = new System.Text.ASCIIEncoding().GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            // Parse JSON response manually
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("resources", out JsonElement resources))
                {
                    foreach (JsonElement resource in resources.EnumerateArray())
                    {
                        if (resource.TryGetProperty("name", out JsonElement nameElement) &&
                            nameElement.GetString() == vmName)
                        {
                            if (resource.TryGetProperty("resourceId", out JsonElement resourceIdElement))
                            {
                                return resourceIdElement.GetString();
                            }
                        }
                    }
                }
            }

            return null; // VM not found
        }
    }
}
