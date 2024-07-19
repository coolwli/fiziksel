using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected async void btnGetApiToken_Click(object sender, EventArgs e)
    {
        string apiUrl = "https://ptekvrops01.fw.garanti.com.tr/suite-api/api/auth/token/acquire?_no_links=true";
        string requestBody = "{ \"username\": \"kullanici_adiniz\", \"password\": \"sifreniz\" }"; // Kullanıcı adı ve şifre bilgilerinizi buraya ekleyin

        try
        {
            string result = await GetApiTokenAsync(apiUrl, requestBody);
            lblTokenResponse.Text = "Token Yanıtı: " + result;
        }
        catch (Exception ex)
        {
            lblTokenResponse.Text = "Hata: " + ex.Message;
        }
    }

    private async Task<string> GetApiTokenAsync(string url, string requestBody)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";

        byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
        request.ContentLength = byteArray.Length;

        using (Stream dataStream = await request.GetRequestStreamAsync())
        {
            dataStream.Write(byteArray, 0, byteArray.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
