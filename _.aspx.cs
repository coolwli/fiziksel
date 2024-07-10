using System;
using System.IO;
using System.Net;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string serverUrl = "https://vrops-server-address";
            string dashboardId = "your-dashboard-id";
            string username = "your-username";
            string password = "your-password";

            try
            {
                string dashboardData = GetDashboardData(serverUrl, dashboardId, username, password);
                Label1.Text = dashboardData;
            }
            catch (Exception ex)
            {
                Label1.Text = "Error: " + ex.Message;
            }
        }
    }

    private string GetDashboardData(string serverUrl, string dashboardId, string username, string password)
    {
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        var requestUrl = $"{serverUrl}/suite-api/api/dashboards/{dashboardId}";

        var request = (HttpWebRequest)WebRequest.Create(requestUrl);
        request.Method = "GET";
        request.Headers["Authorization"] = $"Basic {authToken}";
        request.Accept = "application/json";

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}
