using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.Script.Serialization;
using Microsoft.Office.Interop.Excel;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sharedFolderPath = @"\\tekscr1\f$\Serhat\RvTools_Inventory\Reports";  

        // Klasördeki tüm Excel dosyalarını al (xlsx uzantılı)
        var excelFiles = Directory.GetFiles(sharedFolderPath, "*.xlsx")
                                   .Select(file => new FileInfo(file))
                                   .OrderByDescending(file => file.LastWriteTime)  // En son değiştirilene göre sırala
                                   .Take(4)  // En son 4 dosyayı al
                                   .ToList();

        // Dosya listesi boşsa hata mesajı ver
        if (excelFiles.Count == 0)
        {
            Response.Write("Paylaşımlı klasörde hiç Excel dosyası bulunamadı.");
            return;
        }

        // Excel dosyalarını tek tek oku
        List<string[]> rows = new List<string[]>();

        foreach (var file in excelFiles)
        {
            string excelFilePath = file.FullName;  // Dosyanın tam yolu

            Application excelApp = new Application();
            Workbook workbook = excelApp.Workbooks.Open(excelFilePath);
            Worksheet worksheet = (Worksheet)workbook.Sheets[1]; // İlk sayfayı al

            Range range = worksheet.UsedRange;
            int rowCount = range.Rows.Count;

            // Excel dosyasındaki verileri oku
            for (int i = 1; i <= rowCount; i++)
            {
                string[] row = new string[3]; // 3 sütun olduğunu varsayıyoruz
                row[0] = range.Cells[i, 1].Value2.ToString();
                row[1] = range.Cells[i, 2].Value2.ToString();
                row[2] = range.Cells[i, 3].Value2.ToString();
                rows.Add(row);
            }

            // Çalışma kitabını ve Excel uygulamasını kapat
            workbook.Close(0);
            excelApp.Quit();
        }

        // JavaScript'e veri göndermek için JSON formatına dönüştür
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        string json = serializer.Serialize(rows);
        string script = $"<script>data = {json}; initializeTable();</script>";

        ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
    }
}
