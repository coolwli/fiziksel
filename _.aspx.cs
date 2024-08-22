using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;

namespace windows_users
{
    public partial class _default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderPath = @"F:\Ali\windows users";

            try
            {
                // CSV dosyalarını al
                var csvFiles = Directory.GetFiles(folderPath, "*.csv");

                if (csvFiles.Length == 0)
                {
                    Response.Write("CSV dosyası bulunamadı.");
                    return;
                }

                // En son oluşturulmuş dosyayı seç
                string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                List<string[]> rows = new List<string[]>();

                // CSV dosyasını oku
                using (StreamReader reader = new StreamReader(latestFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        if (columns.Length > 0)
                        {
                            string[] cell = columns[0].Split('+');
                            rows.Add(cell);
                        }
                    }
                }

                // Sonuçları ekrana yazdır
                if (rows.Count > 0)
                {
                    StringBuilder output = new StringBuilder();
                    output.AppendLine($"Toplam satır sayısı: {rows.Count}");
                    output.AppendLine("Veri içeriği:");
                    
                    foreach (var row in rows)
                    {
                        output.AppendLine(string.Join(" | ", row));
                    }

                    Response.Write(output.ToString());
                }
                else
                {
                    Response.Write("CSV dosyası boş.");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
        }
    }
}
