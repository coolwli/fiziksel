using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.IO;
using System.Web.UI;

namespace WebApplication1
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OlusturVeIndir();
        }

        private void OlusturVeIndir()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            // Excel dosyası oluştur
            using (var excelPackage = new ExcelPackage())
            {
                // Çalışma sayfasını oluştur
                var worksheet = excelPackage.Workbook.Worksheets.Add("Veri");

                // Verileri ekle
                worksheet.Cells["A1"].Value = "Kategori";
                worksheet.Cells["B1"].Value = "Değer";
                worksheet.Cells["C1"].Value = "Yüzde";

                worksheet.Cells["A2"].Value = "A";
                worksheet.Cells["B2"].Value = 10;
                worksheet.Cells["C2"].Formula = "B2/SUM(B2:B14)";

                worksheet.Cells["A3"].Value = "B";
                worksheet.Cells["B3"].Value = 20;
                worksheet.Cells["C3"].Formula = "B3/SUM(B2:B14)";

                worksheet.Cells["A4"].Value = "C";
                worksheet.Cells["B4"].Value = 30;
                worksheet.Cells["C4"].Formula = "B4/SUM(B2:B14)";

                // 10 satır daha veri ekle
                for (int i = 5; i <= 14; i++)
                {
                    worksheet.Cells["A" + i].Value = "D" + (i - 1);
                    worksheet.Cells["B" + i].Value = i * 10;
                    worksheet.Cells["C" + i].Formula = "B" + i + "/SUM(B2:B14)";
                }

                // Daire grafiği oluştur
                var chart = worksheet.Drawings.AddChart("PieChart", eChartType.Pie) as ExcelPieChart;
                chart.SetPosition(5, 0, 2, 0);
                chart.SetSize(400, 400);
                chart.Series.Add(worksheet.Cells["B2:B14"], worksheet.Cells["A2:A14"]);
                chart.Title.Text = "Daire Grafiği";
                chart.Border.LineStyle = eLineStyle.Solid;
                chart.Legend.Position = eLegendPosition.Right;

                // Dilim etiketlerini eklemek için
                chart.DataLabel.ShowCategory = true;
                chart.DataLabel.ShowLeaderLines = true;

                // Dilim dışındaki yüzdeleri eklemek için
                chart.DataLabel.ShowPercent = true;
                chart.DataLabel.ShowValue = true;

                // Geçici dosya yolunu oluştur
                string tempFilePath = Path.GetTempFileName();
                string excelFilePath = Path.ChangeExtension(tempFilePath, "xlsx");

                // Excel dosyasını kaydet
                File.WriteAllBytes(excelFilePath, excelPackage.GetAsByteArray());

                // Dosyayı indirme işlemi
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=example.xlsx");
                Response.TransmitFile(excelFilePath);
                Response.End();
            }
        }

    }
}
