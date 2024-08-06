using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public partial class DiskStats : Page
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DiskStats()
    {
        _httpClientFactory = Startup.ServiceProvider.GetRequiredService<IHttpClientFactory>();
    }

    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string apiUrl = "https://api.example.com/diskstats";
            
            try
            {
                var diskStats = await GetDiskStatsAsync(apiUrl);
                var processedStats = ProcessDiskStats(diskStats);

                // Use a control to display the results
                DisplayDiskStats(processedStats);
            }
            catch (Exception ex)
            {
                // Handle error
                DisplayError(ex.Message);
            }
        }
    }

    private async Task<Dictionary<string, float>> GetDiskStatsAsync(string apiUrl)
    {
        var client = _httpClientFactory.CreateClient();
        
        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var diskStats = JsonConvert.DeserializeObject<Dictionary<string, float>>(responseBody);
            return diskStats;
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP request-specific errors
            throw new ApplicationException("An error occurred while fetching disk stats.", ex);
        }
        catch (JsonException ex)
        {
            // Handle JSON parsing errors
            throw new ApplicationException("An error occurred while processing disk stats.", ex);
        }
    }

    private Dictionary<string, float> ProcessDiskStats(Dictionary<string, float> diskStats)
    {
        var filteredStats = new Dictionary<string, float>();

        foreach (var stat in diskStats)
        {
            if (stat.Key.StartsWith("guestfilesystem:"))
            {
                filteredStats[stat.Key] = stat.Value;
            }
        }

        return filteredStats;
    }

    private void DisplayDiskStats(Dictionary<string, float> processedStats)
    {
        foreach (var stat in processedStats)
        {
            // Instead of Response.Write, consider using a control like GridView, Repeater, etc.
            // Example using a Label control:
            var statLabel = new Label
            {
                Text = $"Disk: {stat.Key}, Usage: {stat.Value}%<br>"
            };
            Controls.Add(statLabel);
        }
    }

    private void DisplayError(string errorMessage)
    {
        // Display error message, for example using a Label control
        var errorLabel = new Label
        {
            Text = $"Hata: {errorMessage}<br>",
            ForeColor = System.Drawing.Color.Red
        };
        Controls.Add(errorLabel);
    }
}
