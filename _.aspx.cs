 XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(xmlData);

    // Identifier değerini alıyoruz
    XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
    nsManager.AddNamespace("ops", "http://webservice.vmware.com/vRealizeOpsMgr/1.0/");

    XmlNode identifierNode = xmlDoc.SelectSingleNode("//ops:resource/@identifier", nsManager);
    if (identifierNode != null)
    {
        string identifier = identifierNode.Value;
        // identifier değeriyle yapılacak işlemler
        Response.Write("Identifier: " + identifier);
    }
