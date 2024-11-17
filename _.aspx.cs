using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace webconfigs
{
    public partial class config : System.Web.UI.Page
    {
        private string selectedConfigFile = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadConfigFiles();
            }
        }

        // Konfigürasyon dosyalarını yükler
        private void LoadConfigFiles()
        {
            string rootPath = @"C:\inetpub\wwwroot"; // wwwroot dizini
            if (Directory.Exists(rootPath))
            {
                var configFiles = FindConfigFiles(rootPath);
                ddlConfigFiles.Items.Clear();

                if (configFiles.Count > 0)
                {
                    foreach (var configFile in configFiles)
                    {
                        string topLevelDir = GetTopLevelDirectory(configFile, rootPath);
                        ddlConfigFiles.Items.Add(new ListItem(topLevelDir, configFile));
                    }

                    ddlConfigFiles.SelectedIndex = 0;
                    ddlConfigFiles_SelectedIndexChanged(null, null); // İlk dosya için otomatik yükleme
                }
                else
                {
                    DisplayError("Web.config dosyası bulunamadı.");
                }
            }
            else
            {
                DisplayError("Kök dizin mevcut değil.");
            }
        }

        // Web.config dosyalarını arar
        private List<string> FindConfigFiles(string rootPath)
        {
            List<string> configFiles = new List<string>();
            try
            {
                string[] files = Directory.GetFiles(rootPath, "web.config", SearchOption.AllDirectories);
                configFiles.AddRange(files);
            }
            catch (Exception ex)
            {
                DisplayError($"Config dosyaları yüklenirken hata oluştu: {ex.Message}");
            }
            return configFiles;
        }

        // Config dosyasının üst dizin ismini alır
        private string GetTopLevelDirectory(string configFilePath, string rootPath)
        {
            string relativePath = configFilePath.Replace(rootPath + @"\", "");
            string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);
            return pathParts.Length > 0 ? pathParts[0] : string.Empty;
        }

        // Config dosyasındaki yetkili kullanıcıları çeker
        private List<string> GetAuthorizedUsersFromConfig(string configFile)
        {
            List<string> users = new List<string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);

                XmlNodeList systemWebServerNodes = doc.GetElementsByTagName("system.webServer");
                foreach (XmlNode systemWebServerNode in systemWebServerNodes)
                {
                    foreach (XmlNode childNode in systemWebServerNode.ChildNodes)
                    {
                        if (childNode.Name == "add")
                        {
                            string username = childNode.Attributes["roles"]?.Value;
                            if (!string.IsNullOrEmpty(username))
                            {
                                users.Add(username);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Hata: {ex.Message}");
            }
            return users;
        }

        // Seçilen config dosyasına yeni kullanıcı ekler
        private void AddUserToConfig(string configFile, string username)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(configFile);

                    XmlNode systemWebServerNode = GetOrCreateSystemWebServerNode(doc);
                    XmlNode addNode = doc.CreateElement("add");

                    // Kullanıcı adı ile roles ekleniyor
                    XmlAttribute rolesAttr = doc.CreateAttribute("roles");
                    rolesAttr.Value = username;
                    addNode.Attributes.Append(rolesAttr);

                    // Sabit olarak accessType ve verbs ekleniyor
                    XmlAttribute accessTypeAttr = doc.CreateAttribute("accessType");
                    accessTypeAttr.Value = "Allow"; // Sabit olarak "Allow" olacak
                    addNode.Attributes.Append(accessTypeAttr);

                    XmlAttribute verbsAttr = doc.CreateAttribute("verbs");
                    verbsAttr.Value = "GET, POST"; // Sabit verbs değeri
                    addNode.Attributes.Append(verbsAttr);

                    systemWebServerNode.AppendChild(addNode);

                    doc.Save(configFile);
                }
                else
                {
                    throw new FileNotFoundException("Config dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcıyı eklerken hata oluştu: {ex.Message}");
            }
        }

        // system.webServer node'unu alır veya oluşturur
        private XmlNode GetOrCreateSystemWebServerNode(XmlDocument doc)
        {
            XmlNode configurationNode = doc.SelectSingleNode("//configuration");
            XmlNode systemWebServerNode = configurationNode.SelectSingleNode("system.webServer");

            if (systemWebServerNode == null)
            {
                systemWebServerNode = doc.CreateElement("system.webServer");
                configurationNode.AppendChild(systemWebServerNode);
            }

            return systemWebServerNode;
        }

        // Hata mesajını görüntüler
        private void DisplayError(string message)
        {
            errorMessage.InnerText = message;
            errorMessage.Visible = true;
        }

        // Kullanıcı ekleme butonuna tıklandığında
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    AddUserToConfig(selectedConfigFile, username);
                    RefreshAuthorizedUsers(); // Kullanıcı listesine yenilemeyi uygular
                    txtUsername.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    DisplayError($"Hata: {ex.Message}");
                }
            }
            else
            {
                DisplayError("Kullanıcı adı boş olamaz.");
            }
        }

        // Config dosyasındaki yetkili kullanıcıları günceller
        private void RefreshAuthorizedUsers()
        {
            if (File.Exists(selectedConfigFile))
            {
                var authorizedUsers = GetAuthorizedUsersFromConfig(selectedConfigFile);
                gvAuthorizedUsers.DataSource = authorizedUsers;
                gvAuthorizedUsers.DataBind();
            }
            else
            {
                DisplayError("Seçilen config dosyası bulunamadı.");
            }
        }

        // Kullanıcıyı kaldırma işlemi
        protected void gvAuthorizedUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveUser")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string username = gvAuthorizedUsers.DataKeys[index].Value.ToString();
                RemoveUserFromConfig(selectedConfigFile, username);
                RefreshAuthorizedUsers(); // Kullanıcıyı kaldırdıktan sonra listeyi yeniler
            }
        }

        // Config dosyasından kullanıcıyı kaldırır
        private void RemoveUserFromConfig(string configFile, string username)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(configFile);

                    XmlNode systemWebServerNode = doc.SelectSingleNode("//configuration/system.webServer");
                    foreach (XmlNode addNode in systemWebServerNode.ChildNodes)
                    {
                        if (addNode.Attributes["roles"]?.Value == username)
                        {
                            systemWebServerNode.RemoveChild(addNode);
                        }
                    }

                    doc.Save(configFile);
                }
                else
                {
                    throw new FileNotFoundException("Config dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Kullanıcıyı kaldırırken hata oluştu: {ex.Message}");
            }
        }
    }
}
