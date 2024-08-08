<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YourNamespace.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Form Veri Gönderimi</title>
    <script type="text/javascript">
        var data = []; // Diziyi buraya tanımlayın

        function add1000rows() {
            for (let i = 0; i < 1000; i++) {
                data.push({
                    "Name": `Name ${i}`,
                    "vCenter": `vCenter ${i}`,
                    "CPU": Math.floor(Math.random() * 10),
                    "Memory(GB)": Math.floor(Math.random() * 10),
                    "Total Disk(GB)": Math.floor(Math.random() * 10),
                    "Power State": Math.random() > 0.5 ? "Powered On" : "Powered Off",
                    "Cluster": `Cluster ${i}`,
                    "Data Center": `Data Center ${i}`,
                    "Owner": `Owner ${i}`,
                    "Created Date": new Date(2021, Math.floor(Math.random() * 12), Math.floor(Math.random() * 28)).toLocaleDateString()
                });
            }
        }

        function sendArray() {
            add1000rows(); // Veriyi oluştur

            var hiddenField = document.getElementById('<%= hfArray.ClientID %>');
            hiddenField.value = JSON.stringify(data); // JSON formatında veriyi gizli alana yaz

            document.getElementById('<%= form1.ClientID %>').submit(); // Formu gönder
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hfArray" runat="server" />
        <asp:Button ID="btnSend" runat="server" Text="Diziyi Gönder" OnClientClick="sendArray(); return false;" />
    </form>
</body>
</html>












using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace YourNamespace
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string jsonArray = hfArray.Value;
                if (!string.IsNullOrEmpty(jsonArray))
                {
                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    var dataList = jsSerializer.Deserialize<List<Dictionary<string, object>>>(jsonArray);
                    // Burada dataList ile işlemler yapabilirsiniz
                    // Örneğin, veritabanına kaydetmek veya başka bir işlem yapmak
                }
            }
        }
    }
}








