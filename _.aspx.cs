using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

public partial class ExcelReader : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sharedFolderPath = @"\\sunucuadi\f$\paylasim";  // Paylaşımlı klasör yolu

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
        foreach (var file in excelFiles)
        {
            string excelFilePath = file.FullName;  // Dosyanın tam yolu
            string connectionString = 
                @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelFilePath + @";Extended Properties=""Excel 12.0 Xml;HDR=YES;""";

            OleDbConnection connection = new OleDbConnection(connectionString);

            try
            {
                connection.Open();

                // Belirli bir sayfayı seçin (Örneğin, "Sheet1")
                string query = "SELECT [Column1], [Column2], [Column3] FROM [Sheet1$]";  // Verileri alacağınız sayfa

                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, connection);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                // Dosya adı ve veriyi ekrana yazdırma
                Response.Write("<b>Dosya Adı: </b>" + file.Name + "<br/>");

                // Veriyi ekranda yazdırma
                foreach (DataRow row in dt.Rows)
                {
                    Response.Write(row["Column1"] + " - " + row["Column2"] + " - " + row["Column3"] + "<br/>");
                }

                Response.Write("<br/>");
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
