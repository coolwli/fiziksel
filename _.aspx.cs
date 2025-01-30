using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace vNetwork
{
    public partial class _default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderPath = @"C:\Temp\Winwin\Windows VM Networks";

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
                        if (!string.IsNullOrEmpty(line))
                        {

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
                csv.AppendLine("Sunucu Adı;IP Adresi;Subnet Bilgisi;Mac Adresi;İşletim Sistemi;vCenter");

                foreach (var row in tableData)
                {
                    csv.AppendLine($"{row[0]};{row[1]}...");

                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=administrators.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }
    }
}
