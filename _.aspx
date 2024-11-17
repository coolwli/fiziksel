<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="config.aspx.cs" Inherits="webconfigs.config" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <meta charset="utf-8" />
    <title>Web Config Yetkili Kişiler</title>
    <style>
        /* Genel Sayfa ve Konteyner Stili */
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f1f3f6;
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

        .form-row input,
        .form-row select {
            width: 70%;
            padding: 12px 18px;
            font-size: 16px;
            border: 1px solid #ccc;
            border-radius: 8px;
            background-color: #f9f9f9;
            box-sizing: border-box;
            transition: all 0.3s ease;
        }

        .form-row input:focus,
        .form-row select:focus {
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

        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 30px;
        }

        .table th, .table td {
            padding: 14px;
            text-align: left;
            font-size: 16px;
            border-bottom: 1px solid #ddd;
        }

        .table th {
            background-color: #f7f7f7;
            color: #444;
            font-weight: 600;
        }

        .table td {
            background-color: #fff;
            color: #555;
        }

        .table .btn-remove {
            font-size: 14px;
            padding: 8px 12px;
        }

        .table tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table tr:hover {
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
            <div class="table">
                <asp:GridView ID="gvAuthorizedUsers" runat="server" AutoGenerateColumns="false" CssClass="table" OnRowCommand="gvAuthorizedUsers_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Username" HeaderText="Kullanıcı Adı" SortExpression="Username" />
                        <asp:ButtonField CommandName="RemoveUser" Text="Kaldır" ButtonType="Button" HeaderText="İşlem" ItemStyle-CssClass="btn-remove"/>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="errorMessage" class="error-message" runat="server" visible="false"></div>
        </div>
    </form>
</body>
</html>
