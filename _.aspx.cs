private async Task<string> GetVmId(string vmName)
{
    string getIdUrl = $"{VropsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getIdUrl);
    request.Method = "GET";
    request.Headers["Authorization"] = $"vRealizeOpsToken {token}";

    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
    {
        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            string responseText = await reader.ReadToEndAsync();
            XDocument xdoc = XDocument.Parse(responseText);
            XNamespace opsNamespace = "http://webservice.vmware.com/vRealizeOpsMgr/1.0/";
            XElement resourceElement = xdoc.Descendants(opsNamespace + "resource").FirstOrDefault();
            if (resourceElement != null)
            {
                string identifier = resourceElement.Attribute("identifier")?.Value;
                return identifier;
            }
            return null;
        }
    }
}
