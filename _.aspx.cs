using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;

namespace windows_users
{
    public partial class local : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderPath = @"C:\temp\windows users\local users";

            try
            {
                var txtFiles = Directory.GetFiles(folderPath, "*.txt");

                if (txtFiles.Length == 0)
                {
                    Response.Write("TXT dosyası bulunamadı.");
                    return;
                }

                string latestFile = txtFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                List<string[]> rows = new List<string[]>();

                using (StreamReader reader = new StreamReader(latestFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Satırı '+' ile ayrıştır
                        var entries = line.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(e => e.Trim())
                                          .ToArray();

                        if (entries.Length >= 2)
                        {
                            // İlk iki öğeyi al
                            string serverName = entries[0].Split(':')[1].Trim().Replace("'", "");
                            string userName = entries[1].Split(':')[1].Trim().Replace("'", "");

                            rows.Add(new[] { serverName, userName });
                        }
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                string json = serializer.Serialize(rows);
                string script = $"<script> data = {json};initializeTable();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                var tableData = js.Deserialize<List<string[]>>(jsonData);

                if (tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Host,User");

                foreach (var row in tableData)
                {
                    csv.AppendLine($"{row[0]},{row[1]}");
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=local_users.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }
    }
}


Severity	Code	Description	Project	File	Line	Suppression State
Error	CS0136	A local or parameter named 'e' cannot be declared in this scope because that name is used in an enclosing local scope to define a local or parameter	windows users	C:\inetpub\wwwroot\windows users\windows users\local.aspx.cs	38	Active

