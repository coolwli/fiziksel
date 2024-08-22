using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;

namespace windows_users
{
    public partial class _default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderPath = @"F:\Ali\windows users";

            try
            {
                var csvFiles = Directory.GetFiles(folderPath, "*.csv");

                if (csvFiles.Length == 0)
                {
                    Response.Write("CSV dosyası bulunamadı.");
                    return;
                }

                string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                List<string[]> rows = new List<string[]>();

                using (StreamReader reader = new StreamReader(latestFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        if (columns.Length > 0)
                        {
                            string[] cell = columns[0].Split('+').Select(c=>c.Trim()).ToArray();
                            rows.Add(cell);
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
                js.MaxJsonLength = int.MaxValue; // Maksimum JSON uzunluğunu ayarla
                var tableData = js.Deserialize<List<Dictionary<string, string>>>(jsonData);

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Host,User");

                foreach (var row in tableData)
                {
                    csv.AppendLine($"{row["a"]},{row["vCenter"]}");
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=vmsData.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }
    }
}
