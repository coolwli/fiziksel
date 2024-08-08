<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YourNamespace.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Form Veri Gönderimi</title>
    <script type="text/javascript">
        function sendArray() {
            var array = [1, 2, 3, 4, 5]; // Göndermek istediğiniz dizi
            var hiddenField = document.getElementById('<%= hfArray.ClientID %>');
            hiddenField.value = JSON.stringify(array);
            document.getElementById('<%= btnSend.ClientID %>').click(); // Butona tıklama olayını tetikler
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hfArray" runat="server" />
        <asp:Button ID="btnSend" runat="server" Text="Diziyi Gönder" OnClick="btnSend_Click" />
    </form>
</body>
</html>
