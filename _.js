<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YourNamespace.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>vROps API ile VM ID'si Almak</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <button id="getVMId" type="button">VM ID Al</button>
            <div id="vmId"></div>
        </div>
    </form>

    <script>
        document.getElementById('getVMId').addEventListener('click', function() {
            const tokenUrl = 'https://your-vrops-instance/suite-api/api/auth/token/acquire';
            const vmIdUrl = 'https://your-vrops-instance/suite-api/api/resources'; // VM bilgilerini almak için uygun API URL'si

            const username = 'your-username'; // Kullanıcı adınızı buraya koyun
            const password = 'your-password'; // Şifrenizi buraya koyun
            const credentials = btoa(username + ':' + password);

            // Token almak için istek gönder
            fetch(tokenUrl, {
                method: 'POST',
                headers: {
                    'Authorization': 'Basic ' + credentials,
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                const token = data.token;

                // Token ile VM ID'sini almak için istek gönder
                return fetch(vmIdUrl, {
                    method: 'GET',
                    headers: {
                        'Authorization': 'vRealizeOpsToken ' + token,
                        'Accept': 'application/json'
                    }
                });
            })
            .then(response => response.json())
            .then(data => {
                // Veriyi işleyin ve VM ID'sini ekrana yazdırın
                const vmId = data.resources[0].id; // İlgili JSON yanıtına bağlı olarak güncelleyin
                document.getElementById('vmId').innerText = 'VM ID: ' + vmId;
            })
            .catch(error => {
                console.error('Hata:', error);
            });
        });
    </script>
</body>
</html>
