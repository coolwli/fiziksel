using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.Script.Serialization;

public partial class GetResourceId : Page
{
    protected void btnGetResourceId_Click(object sender, EventArgs e)
    {
        string apiUrl = "https://your-vrops-server/suite-api/api/resources";
        string username = "your-username";
        string password = "your-password";
        string vmName = txtVMName.Text;

        try
        {
            // Create a request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{username}:{password}"));
            request.Headers["Authorization"] = "Basic " + authInfo;

            // Get the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    var resources = new JavaScriptSerializer().Deserialize<dynamic>(responseText);
                    string resourceId = FindResourceId(resources, vmName);

                    if (!string.IsNullOrEmpty(resourceId))
                    {
                        ltlResourceId.Text = $"<p>Resource ID for VM '{vmName}': {resourceId}</p>";
                    }
                    else
                    {
                        ltlResourceId.Text = $"<p>VM '{vmName}' not found.</p>";
                    }
                }
            }
        }
        catch (WebException webEx)
        {
            using (StreamReader reader = new StreamReader(webEx.Response.GetResponseStream()))
            {
                string errorText = reader.ReadToEnd();
                ltlResourceId.Text = $"<pre>Error: {errorText}</pre>";
            }
        }
        catch (Exception ex)
        {
            ltlResourceId.Text = $"<pre>Error: {ex.Message}</pre>";
        }
    }

    private string FindResourceId(dynamic resources, string vmName)
    {
        foreach (var resource in resources["resourceList"])
        {
            if (resource["resourceKey"]["name"] == vmName)
            {
                return resource["identifier"];
            }
        }
        return null;
    }
}
