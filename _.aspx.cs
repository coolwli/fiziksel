using System;
using System.IO;
using System.Linq;
using System.Text;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // CSV dosyalarının bulunduğu klasör
        string folderPath = Server.MapPath("~/App_Data/");
        
        // Klasördeki tüm CSV dosyalarını alın
        var csvFiles = Directory.GetFiles(folderPath, "*.csv");
        
        if (csvFiles.Length == 0)
        {
            LiteralData.Text = "Klasörde CSV dosyası bulunamadı.";
            return;
        }
        
        // En son eklenen dosyayı bul
        string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
        
        // En son eklenen CSV dosyasını okuyun ve ilk sütundaki verileri alın
        StringBuilder sb = new StringBuilder();

        try
        {
            using (StreamReader reader = new StreamReader(latestFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Satırı virgülle ayırarak sütunları elde edin
                    string[] columns = line.Split(',');

                    // İlk sütundaki veriyi alın
                    if (columns.Length > 0)
                    {
                        sb.AppendLine(columns[0]);
                    }
                }
            }

            // Verileri Literal kontrolüne atayın
            LiteralData.Text = sb.ToString().Replace(Environment.NewLine, "<br/>");
        }
        catch (Exception ex)
        {
            // Hata durumunda uygun bir mesaj gösterin
            LiteralData.Text = "Hata: " + ex.Message;
        }
    }
}
