<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="authconfiger._default" %>
<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <meta charset="utf-8" />
    <title>Web Config Yetkili Kişiler</title>
    <style>
        /* Sayfa ve Konteyner Stili */
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f7fa;
            margin: 0;
            padding: 0;
            color: #333;
        }
        .container {
            width: 80%;
            max-width: 1000px;
            margin: 50px auto;
            background-color: #fff;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
        }
        h2 {
            text-align: center;
            font-size: 28px;
            font-weight: 600;
            color: #444;
            margin-bottom: 30px;
        }
        .form-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
        }
        .form-row label {
            font-weight: 600;
            color: #555;
            width: 25%;
            font-size: 16px;
            line-height: 36px;
        }
        .form-row input, .form-row select {
            width: 70%;
            padding: 12px 18px;
            font-size: 16px;
            border: 1px solid #ccc;
            border-radius: 8px;
            background-color: #f9f9f9;
            box-sizing: border-box;
            transition: all 0.3s ease;
        }
        .form-row input:focus, .form-row select:focus {
            border-color: #007bff;
            outline: none;
            background-color: #eaf3ff;
        }
        .btn {
            background-color: #007bff;
            color: white;
            padding: 14px 28px;
            font-size: 16px;
            font-weight: 600;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            width: 100%;
            margin-top: 20px;
        }
        .btn:hover {
            background-color: #0056b3;
        }
        .btn-remove {
            background-color: #e74c3c;
            color: white;
            padding: 8px 16px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 14px;
            border: none;
            margin-left: 10px;
            transition: background-color 0.3s ease;
        }
        .btn-remove:hover {
            background-color: #c0392b;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 30px;
        }
        th, td {
            padding: 14px;
            text-align: left;
            font-size: 16px;
            border-bottom: 1px solid #ddd;
            color: #555;
        }
        th {
            background-color: #f7f7f7;
            color: #444;
            font-weight: 600;
        }
        tr:nth-child(even) {
            background-color: #f9f9f9;
        }
        tr:hover {
            background-color: #f1f1f1;
        }
        .error-message {
            color: #e74c3c;
            background-color: #f8d7da;
            padding: 15px;
            border-radius: 8px;
            margin-top: 20px;
            font-weight: 600;
            display: none;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Web Config Dosyasındaki Yetkili Kişiler</h2>

            <!-- Config Dosyası Seçimi -->
            <div class="form-row">
                <label for="ddlConfigFiles">Config Dosyasını Seçin:</label>
                <asp:DropDownList ID="ddlConfigFiles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlConfigFiles_SelectedIndexChanged">
                    <asp:ListItem Text="Bir config dosyası seçin" Value="" />
                </asp:DropDownList>
            </div>

            <!-- Kullanıcı Adı Ekleme -->
            <div class="form-row">
                <label for="txtUsername">Yeni Kullanıcı Adı:</label>
                <asp:TextBox ID="txtUsername" runat="server" Placeholder="Kullanıcı adı girin..." />
            </div>

            <asp:Button ID="btnAddUser" runat="server" Text="Kullanıcı Ekle" OnClick="btnAddUser_Click" CssClass="btn" />

            <!-- Kullanıcı Listesi -->
            <div>
                <table id="authorizedUsersTable">
                    <thead>
                        <tr>
                            <th>Kullanıcı Adı</th>
                            <th>İşlem</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Buraya kullanıcılar dinamik olarak eklenecek -->
                    </tbody>
                </table>
            </div>

            <div id="errorMessage" class="error-message" runat="server" visible="false"></div>
        </div>

        <script>
            // Kullanıcı ekleme işlemi için JavaScript
            function addUsersToTable(usernames) {
                var table = document.getElementById("authorizedUsersTable").getElementsByTagName('tbody')[0];
                usernames.forEach(function(username) {
                    var row = table.insertRow();
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);

                    cell1.innerHTML = username; // Kullanıcı adı
                    cell2.innerHTML = '<button class="btn-remove" onclick="removeUserFromTable(this, \'' + username + '\')">Kaldır</button>';
                });
            }

            // Kullanıcıyı tablodan kaldırma işlemi
            function removeUserFromTable(button, username) {
                var row = button.parentNode.parentNode;
                row.parentNode.removeChild(row);

                // Config dosyasının yolunu alın
                var configFilePath = document.getElementById("ddlConfigFiles").value;

                // Kullanıcıyı Web.config'ten silme işlemi için server tarafına bir istek gönderelim
                PageMethods.RemoveUserFromConfig(configFilePath, username, onSuccess, onError);
            }

            function onSuccess(result) {
                // Silme işlemi başarılı olduğunda yapılacak işlemler
                console.log("Kullanıcı başarıyla silindi.");
            }

            function onError(error) {
                alert("Kullanıcı silinirken bir hata oluştu: " + error.get_message());
            }
        </script>
    </form>
</body>
</html>
