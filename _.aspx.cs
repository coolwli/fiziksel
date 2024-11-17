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

        private void LoadConfigFiles()
        {
            string rootPath = @"C:\inetpub\wwwroot"; // wwwroot dizinini burada belirtiyoruz
            if (Directory.Exists(rootPath))
            {
                var configFiles = FindConfigFiles(rootPath);
                ddlConfigFiles.Items.Clear();

                if (configFiles.Count > 0)
                {
                    foreach (var configFile in configFiles)
                    {
                        // web.config dosyasının tam yolunu alıyoruz
                        string topLevelDir = GetTopLevelDirectory(configFile, rootPath);

                        // dropdown'a, wwwroot altındaki en üst klasörü ekliyoruz
                        ddlConfigFiles.Items.Add(new ListItem(topLevelDir, configFile));
                    }

                    ddlConfigFiles.SelectedIndex = 0;
                    ddlConfigFiles_SelectedIndexChanged(null, null);
                }
                else
                {
                    DisplayError("No web.config files found.");
                }
            }
            else
            {
                DisplayError("Root path does not exist.");
            }
        }

        // web.config dosyalarını tüm dizinlerde arıyoruz
        private List<string> FindConfigFiles(string rootPath)
        {
            List<string> configFiles = new List<string>();
            try
            {
                // web.config dosyasını arama
                string[] files = Directory.GetFiles(rootPath, "web.config", SearchOption.AllDirectories);
                configFiles.AddRange(files);
            }
            catch (Exception ex)
            {
                DisplayError("Config dosyaları yüklenirken bir hata oluştu: " + ex.Message);
            }
            return configFiles;
        }

        // web.config dosyasının bulunduğu dizinden, wwwroot altında ilk dizini alır
        private string GetTopLevelDirectory(string configFilePath, string rootPath)
        {
            // config dosyasının bulunduğu dizini alıyoruz
            string relativePath = configFilePath.Replace(rootPath + @"\", "");

            // wwwroot altındaki ilk dizini almak için, ilk dizini ayıklıyoruz
            string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);

            // İlk dizini döndürüyoruz
            return pathParts.Length > 0 ? pathParts[0] : string.Empty;
        }

        protected void ddlConfigFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;
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

        private List<User> GetAuthorizedUsersFromConfig(string configFile)
        {
            List<User> users = new List<User>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);

                XmlNodeList authorizationNodes = doc.GetElementsByTagName("authorization");
                foreach (XmlNode authorizationNode in authorizationNodes)
                {
                    foreach (XmlNode childNode in authorizationNode.ChildNodes)
                    {
                        if (childNode.Name == "allow" || childNode.Name == "deny")
                        {
                            string username = childNode.Attributes["users"]?.Value;
                            if (!string.IsNullOrEmpty(username))
                            {
                                users.Add(new User
                                {
                                    Username = username,
                                    Action = childNode.Name
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError("Hata: " + ex.Message);
            }
            return users;
        }

        private void AddUserToConfig(string configFile, string username, string action)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(configFile);

                    XmlNode authorizationNode = GetOrCreateAuthorizationNode(doc);
                    XmlNode newUserNode = doc.CreateElement(action);
                    XmlAttribute usersAttr = doc.CreateAttribute("users");
                    usersAttr.Value = username;
                    newUserNode.Attributes.Append(usersAttr);
                    authorizationNode.AppendChild(newUserNode);

                    doc.Save(configFile);
                }
                else
                {
                    throw new FileNotFoundException("Config dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Kullanıcıyı eklerken hata oluştu: " + ex.Message);
            }
        }

        private XmlNode GetOrCreateAuthorizationNode(XmlDocument doc)
        {
            XmlNode systemWebNode = doc.SelectSingleNode("//configuration/system.web");
            if (systemWebNode == null)
            {
                systemWebNode = doc.CreateElement("system.web");
                doc.SelectSingleNode("//configuration").AppendChild(systemWebNode);
            }

            XmlNode authorizationNode = systemWebNode.SelectSingleNode("authorization");
            if (authorizationNode == null)
            {
                authorizationNode = doc.CreateElement("authorization");
                systemWebNode.AppendChild(authorizationNode);
            }

            return authorizationNode;
        }

        private void RemoveUserFromConfig(string configFile, string username)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(configFile);

                    XmlNodeList authorizationNodes = doc.GetElementsByTagName("authorization");
                    foreach (XmlNode authorizationNode in authorizationNodes)
                    {
                        foreach (XmlNode childNode in authorizationNode.ChildNodes)
                        {
                            if ((childNode.Name == "allow" || childNode.Name == "deny") &&
                                childNode.Attributes["users"]?.Value == username)
                            {
                                authorizationNode.RemoveChild(childNode); // Kullanıcıyı kaldır
                                doc.Save(configFile);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("Config dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Kullanıcıyı kaldırırken hata oluştu: " + ex.Message);
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;

            string usernameToAdd = txtUsername.Text.Trim();
            string action = ddlAction.SelectedValue;

            if (string.IsNullOrEmpty(usernameToAdd))
            {
                DisplayError("Kullanıcı adı boş olamaz.");
                return;
            }

            if (string.IsNullOrEmpty(selectedConfigFile))
            {
                DisplayError("Config dosyası seçilmemiş.");
                return;
            }

            try
            {
                AddUserToConfig(selectedConfigFile, usernameToAdd, action);
                ddlConfigFiles_SelectedIndexChanged(sender, e);
                txtUsername.Text = string.Empty;
            }
            catch (Exception ex)
            {
                DisplayError("Hata: " + ex.Message);
            }
        }

        protected void gvAuthorizedUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveUser")
            {
                selectedConfigFile = ddlConfigFiles.SelectedValue;

                int index = Convert.ToInt32(e.CommandArgument);
                string usernameToRemove = gvAuthorizedUsers.Rows[index].Cells[0].Text;

                try
                {
                    RemoveUserFromConfig(selectedConfigFile, usernameToRemove);
                    ddlConfigFiles_SelectedIndexChanged(sender, e);
                }
                catch (Exception ex)
                {
                    DisplayError("Hata: " + ex.Message);
                }
            }
        }

        private void DisplayError(string message)
        {
            errorMessage.InnerText = message;
            errorMessage.Visible = true;
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Action { get; set; }
    }
}
