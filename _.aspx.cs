using System;
using System.Data.SqlClient;
using System.Text;

public partial class YourPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Örnek bir string dizisi
        string[] isimler = { "Ahmet", "Mehmet", "Ayşe" };
        
        // String dizisini SQL sorgusunda kullanılabilir bir formata dönüştürme
        string inClause = GetInClause(isimler);

        // SQL sorgusunu oluşturma
        string query = $"SELECT * FROM kullanicilar WHERE isim IN ({inClause})";

        // Veritabanı bağlantısı ve sorgu çalıştırma
        string connectionString = "your_connection_string_here";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Veritabanı sonucunu işleme
                // Örneğin, reader["isim"] ile isim sütununa erişebilirsiniz
            }
        }
    }

    private string GetInClause(string[] items)
    {
        // Dizideki elemanları SQL sorgusu için uygun formatta birleştirir
        StringBuilder sb = new StringBuilder();
        foreach (string item in items)
        {
            sb.Append($"'{item}',");
        }
        
        // Sonundaki son virgülü kaldırma
        if (sb.Length > 0)
        {
            sb.Length--;
        }
        
        return sb.ToString();
    }
}
