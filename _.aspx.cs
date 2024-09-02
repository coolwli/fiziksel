        private async Task<string> GetVmIdAsync(string vmName)
        {
            string getIdUrl = $"{vRopsServer}/suite-api/api/resources?resourceKind=VirtualMachine&name={vmName}";

            var request = new HttpRequestMessage(HttpMethod.Get, getIdUrl);
            request.Headers.Add("Authorization", $"vRealizeOpsToken {_token}");

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseText);

                var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsManager.AddNamespace("ops", OpsNamespace);

                var identifierNode = xmlDoc.SelectSingleNode("//ops:resource/@identifier", nsManager);
                return identifierNode?.Value;
            }
        }
