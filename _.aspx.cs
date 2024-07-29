using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

public partial class Default : Page
{
    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string token = await GetVROpsTokenAsync("https://vrops-server-url", "kullanici_adi", "parola");
            if (!string.IsNullOrEmpty(token))
            {
                await GetVMCPUUsageAsync("https://vrops-server-url", token, "vm_name");
            }
        }
    }

    private async Task<string> GetVROpsTokenAsync(string vropsServer, string username, string password)
    {
        string tokenUrl = $"{vropsServer}/suite-api/api/auth/token/acquire";
        string base64AuthInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64AuthInfo);
            HttpResponseMessage response = await client.PostAsync(tokenUrl, null);

            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = await response.Content.ReadAsAsync<dynamic>();
                return jsonResponse.token;
            }
        }

        return null;
    }

    private async Task GetVMCPUUsageAsync(string vropsServer, string token, string vmName)
    {
        string vmId = await GetVMIdAsync(vropsServer, token, vmName);
        if (string.IsNullOrEmpty(vmId)) return;

        DateTime startTime = DateTime.Now.AddDays(-30);
        DateTime endTime = DateTime.Now;

        string metricsUrl = $"{vropsServer}/suite-api/api/resources/{vmId}/statistics?statKey=cpu|usage_average&begin={startTime:yyyy-MM-ddTHH:mm:ss.fffZ}&end={endTime:yyyy-MM-ddTHH:mm:ss.fffZ}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("vRealizeOpsToken", token);
            HttpResponseMessage response = await client.GetAsync(metricsUrl);

            if (response.IsSuccessStatusCode)
            {
                string cpuUsage = await response.Content.ReadAsStringAsync();
                // CPU kullanım verilerini işleme veya görüntüleme
                Response.Write($"CPU Usage: {cpuUsage}");
            }
        }
    }

    private async Task<string> GetVMIdAsync(string vropsServer, string token, string vmName)
    {
        string resourceUrl = $"{vropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("vRealizeOpsToken", token);
            HttpResponseMessage response = await client.GetAsync(resourceUrl);

            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = await response.Content.ReadAsAsync<dynamic>();
                return jsonResponse.resourceList.resource[0].identifier;
            }
        }

        return null;
    }
}
