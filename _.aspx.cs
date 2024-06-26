using System;
using System.Data;
using System.Data.SqlClient;


namespace fiziksel
{
    public partial class create : System.Web.UI.Page
    {
        string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=gtreportdb;Integrated Security=True";

        public void createNew()
        {
            try
            {


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"INSERT INTO fiziksel  " +
                        $"(Adı,[Seri No],Açıklama,[Üretici Firma],Model,[CPU Soket Sayısı],[CPU Core Sayısı],[Toplam Core Adet]," +
                        $"[Memory (GB)],[Cihaz Tipi],Hall,Row,Rack,[Blade Şasi],[Domain Bilgisi (UCS-HP)],Enviroment,Firma,Sahiplik," +
                        $"Lokasyon,[Kapsam-BBVA Metrics],Cluster,[İşletim Sistemi],[Sorumlu Grup],[Satın Alma Tarihi]," +
                        $"[Planlanan Devreden Çıkarma Tarihi],[Bakım Başlangıç Tarihi],[Bakım Bitiş Tarihi],[Özel Durumu],Support) " +
                        $"VALUES " +
                        $"('{adi.Value}','{seri_no.Value}','{aciklama.Value}','{uretici_firma.Value}','{model.Value}'," +
                        $"'{cpu_soket.Value}','{cpu_core.Value}','{toplam_core.Value}','{memory.Value}','{cihaz_tipi.Value}'," +
                        $"'{hall.Value}','{row.Value}','{rack.Value}','{blade_schasi.Value}','{domain_bilgisi.Value}'," +
                        $"'{enviroment.Value}','{firma.Value}','{sahiplik.Value}','{lokasyon.Value}','{kapsam_bbva_metrics.Value}'," +
                        $"'{cluster.Value}','{isletim_sistemi.Value}','{sorumlu_grup.Value}','{satin_alma_tarihi.Value}'," +
                        $"'{planlanan_devreden_cikarma_tarihi.Value}','{bakim_baslangic_tarihi.Value}','{bakim_bitis_tarihi.Value}'," +
                        $"'{ozel_durumu.Value}','{support.Value}')";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected void onlyButton_Click(object sender, EventArgs e)
        {
            createNew();
            Response.Redirect("default.aspx");

        }

        protected void bothButton_Click(object sender, EventArgs e)
        {
            createNew();
            Response.Redirect("default.aspx");


        }
    }
}
