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

                // En son oluşturulmuş CSV dosyasını alıyoruz
                string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                List<string[]> rows = new List<string[]>();

                // Dosyayı daha hızlı bir şekilde okuyalım
                using (StreamReader reader = new StreamReader(latestFile))
                {
                    string line;
                    
                    // İlk satırı (başlık satırını) atlıyoruz
                    reader.ReadLine();

                    // Diğer satırlarda işlem yapıyoruz
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            // Split işlemi yerine doğrudan kolonları temizleyelim
                            string[] columns = line.Split(',')
                                .Select(col => col.Replace("\"", "").Trim())
                                .ToArray();
                            rows.Add(columns);
                        }
                    }
                }

                // JSON verisine dönüştürme
                JavaScriptSerializer serializer = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue
                };
                string json = serializer.Serialize(rows);

                // Veriyi JavaScript ile başlat
                string script = $"<script>data = {json}; initializeTable();</script>";
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
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer
                    {
                        MaxJsonLength = int.MaxValue
                    };

                    var tableData = js.Deserialize<List<string[]>>(jsonData);

                    if (tableData.Count == 0)
                    {
                        Response.Write("No data available.");
                        return;
                    }

                    // CSV'yi hazırlarken StringBuilder kullanımı daha verimli
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("Sunucu Adı;IP Adresi;Subnet Bilgisi;Mac Adresi;İşletim Sistemi;vCenter");

                    foreach (var row in tableData)
                    {
                        csv.AppendLine($"{row[0]};{row[1]};{row[2]};{row[3]};{row[4]};{row[5]}");
                    }

                    // CSV dosyasını kullanıcıya gönder
                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", "attachment;filename=administrators.csv");
                    Response.Write(csv.ToString());
                    Response.End();
                }
                catch (Exception ex)
                {
                    Response.Write("Hata: " + ex.Message);
                }
            }
        }
    }
}
