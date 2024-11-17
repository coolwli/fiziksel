using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Xml;

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

        // Config dosyasının üst dizin ismini alır
        private string GetTopLevelDirectory(string configFilePath, string rootPath)
        {
            string relativePath = configFilePath.Replace(rootPath + @"\", "");
            string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);
            return pathParts.Length > 0 ? pathParts[0] : string.Empty;
        }

        // Konfigürasyon dosyalarını arar
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

        // Config dosyasındaki yetkili kullanıcıları çeker
        private List<string> GetAuthorizedUsersFromConfig(string configFile)
        {
            List<string> users = new List<string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);

                XmlNodeList authNodes = doc.GetElementsByTagName("authorization");
                foreach (XmlNode authNode in authNodes)
                {
                    foreach (XmlNode childNode in authNode.ChildNodes)
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

        // Kullanıcıyı config dosyasından silme
        [System.Web.Services.WebMethod]
        public static void RemoveUserFromConfig(string configFile, string username)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);

                XmlNodeList authNodes = doc.GetElementsByTagName("authorization");
                foreach (XmlNode authNode in authNodes)
                {
                    foreach (XmlNode childNode in authNode.ChildNodes)
                    {
                        if (childNode.Name == "add" && childNode.Attributes["roles"]?.Value == username)
                        {
                            authNode.RemoveChild(childNode); // Kullanıcıyı kaldır
                            break;
                        }
                    }
                }

                doc.Save(configFile); // Değişiklikleri kaydet
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcıyı silerken hata oluştu: {ex.Message}");
            }
        }

        // Config dosyasını seçtiğinde yetkili kullanıcıları yükler
        protected void ddlConfigFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedConfigFile = ddlConfigFiles.SelectedValue;
            if (File.Exists(selectedConfigFile))
            {
                RefreshAuthorizedUsers();
            }
            else
            {
                DisplayError("Config dosyası bulunamadı");
            }
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

        // Seçilen config dosyasındaki yetkili kullanıcıları günceller
        private void RefreshAuthorizedUsers()
        {
            if (File.Exists(selectedConfigFile))
            {
                List<string> authorizedUsers = GetAuthorizedUsersFromConfig(selectedConfigFile);
                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                var json = serializer.Serialize(authorizedUsers);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "addUserScript",
                    $"addUsersToTable({json});", true);
            }
            else
            {
                DisplayError("Seçilen config dosyası bulunamadı.");
            }
        }

        // Hata mesajı görüntüler
        private void DisplayError(string message)
        {
            errorMessage.InnerText = message;
            errorMessage.Visible = true;
        }
    }
}
