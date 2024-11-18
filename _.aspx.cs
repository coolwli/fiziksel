using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;

namespace authconfiger
{
    public partial class _default : System.Web.UI.Page
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
            string rootPath = @"C:\inetpub\wwwroot";  // Web uygulamanızın kök dizini
            if (Directory.Exists(rootPath))
            {
                var configFiles = GetConfigFiles(rootPath);  // Config dosyalarını al
                PopulateConfigFileDropdown(configFiles);  // Dropdown'ı doldur
            }
            else
            {
                DisplayError("Kök dizin bulunamadı.");
            }
        }

        private List<string> GetConfigFiles(string rootPath)
        {
            var configFiles = new List<string>();
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

        private void PopulateConfigFileDropdown(List<string> configFiles)
        {
            ddlConfigFiles.Items.Clear();
            if (configFiles.Count > 0)
            {
                foreach (var configFile in configFiles)
                {
                    string topLevelDir = GetTopLevelDirectory(configFile);
                    ddlConfigFiles.Items.Add(new ListItem(topLevelDir, configFile));
                }
                ddlConfigFiles.SelectedIndex = 0;
                ddlConfigFiles_SelectedIndexChanged(null, null);  
            }
            else
            {
                DisplayError("Hiçbir config dosyası bulunamadı.");
            }
        }

        private string GetTopLevelDirectory(string configFilePath)
        {
            string relativePath = configFilePath.Substring(@"C:\inetpub\wwwroot".Length).TrimStart(Path.DirectorySeparatorChar);
            string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);
            return pathParts[0];
        }

        private List<User> GetAuthorizedUsersFromConfig()
        {
            var users = new List<User>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(selectedConfigFile);

                XmlNodeList addNodes = doc.SelectNodes("//system.webServer/security/authorization/add");
                foreach (XmlNode addNode in addNodes)
                {
                    string roles = addNode.Attributes["roles"]?.Value;
                    if (!string.IsNullOrEmpty(roles))
                    {
                        users.Add(new User { UserName = roles });
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Config dosyası okunurken hata oluştu: {ex.Message}");
            }

            return users;
        }

        private void RefreshAuthorizedUsers()
        {
            if (File.Exists(selectedConfigFile))
            {
                var authorizedUsers = GetAuthorizedUsersFromConfig();
                if (authorizedUsers.Count > 0)
                {
                    gvAuthorizedUsers.DataSource = authorizedUsers;
                    gvAuthorizedUsers.DataBind();
                }
                else
                {
                    gvAuthorizedUsers.DataSource = null;
                    gvAuthorizedUsers.DataBind();
                }
            }
            else
            {
                DisplayError("Seçilen config dosyası bulunamadı.");
            }
        }

        protected void ddlConfigFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;
            RefreshAuthorizedUsers();
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;

            string newUser = txtUserName.Text.Trim();
            if (!string.IsNullOrEmpty(newUser))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(selectedConfigFile);

                    XmlNode authorizationNode = doc.SelectSingleNode("//system.webServer/security/authorization");
                    if (authorizationNode != null)
                    {
                        XmlElement newUserElement = doc.CreateElement("add");
                        newUserElement.SetAttribute("roles", newUser);
                        newUserElement.SetAttribute("accessType", "Allow");
                        authorizationNode.AppendChild(newUserElement);
                    }

                    doc.Save(selectedConfigFile);
                    RefreshAuthorizedUsers();
                    txtUserName.Text = "";  // Textbox'ı temizle
                }
                catch (Exception ex)
                {
                    DisplayError($"Kullanıcı eklenirken hata oluştu: {ex.Message}");
                }
            }
            else
            {
                DisplayError("Geçersiz kullanıcı adı.");
            }
        }

        protected void gvAuthorizedUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                string userNameToRemove = e.CommandArgument.ToString();
                RemoveUserFromConfig(userNameToRemove);
            }
        }

        private void RemoveUserFromConfig(string userName)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(selectedConfigFile);

                XmlNodeList addNodes = doc.SelectNodes("//system.webServer/security/authorization/add");
                foreach (XmlNode addNode in addNodes)
                {
                    string roles = addNode.Attributes["roles"]?.Value;
                    if (roles == userName)
                    {
                        addNode.ParentNode.RemoveChild(addNode);
                        break;
                    }
                }

                doc.Save(selectedConfigFile);
                RefreshAuthorizedUsers();
            }
            catch (Exception ex)
            {
                DisplayError($"Kullanıcı kaldırılırken hata oluştu: {ex.Message}");
            }
        }

        private void DisplayError(string message)
        {
            errorMessage.InnerText = message;
            errorMessage.Style["display"] = "block";
        }
    }
}
public class User
{
    public string UserName { get; set; }
}
