<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="config.aspx.cs" Inherits="webconfigs.config" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Config Yetkili Kişiler</title>
    <style>
        body {
            font-family: 'Roboto', Arial, sans-serif;
            background-color: #f7f9fb;
            margin: 0;
            padding: 0;
        }

        h2 {
            font-size: 26px;
            font-weight: 500;
            color: #333;
            margin-bottom: 20px;
        }

        .container {
            width: 80%;
            max-width: 1000px;
            margin: 30px auto;
            padding: 30px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        /* Config Dosyası Seçimi için Satır Düzeni */
        .form-row-config {
            display: flex;
            align-items: center;
            margin-bottom: 20px;
        }

        .form-row-config label {
            font-size: 16px;
            color: #666;
            margin-right: 10px;
        }

        .form-row-config select {
            padding: 12px;
            font-size: 16px;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: #f5f5f5;
            transition: all 0.3s ease;
        }

        .form-row-config select:focus {
            border-color: #007BFF;
            outline: none;
            background-color: #eaf5ff;
        }

        /* Yeni Form Kontrolleri için Satır Düzeni */
        .form-row {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
        }

        .form-row input,
        .form-row select {
            flex: 1;
            padding: 12px;
            font-size: 16px;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: #f5f5f5;
            transition: all 0.3s ease;
        }

        .form-row input:focus,
        .form-row select:focus {
            border-color: #007BFF;
            outline: none;
            background-color: #eaf5ff;
        }

        .form-row label {
            font-size: 16px;
            color: #666;
            margin-right: 10px;
        }

        .form-actions {
            display: flex;
            gap: 20px;
            justify-content: space-between;
            margin-top: 20px;
        }

        .form-actions .btn {
            padding: 12px 20px;
            font-size: 16px;
            font-weight: 600;
            color: white;
            background-color: #007BFF;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            width: 100%;
        }

        .form-actions .btn:hover {
            background-color: #0056b3;
        }

        .grid-container {
            margin-top: 30px;
        }

        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .table th, .table td {
            padding: 12px;
            text-align: left;
            font-size: 16px;
            border: 1px solid #ddd;
        }

        .table th {
            background-color: #f7f7f7;
            font-weight: 600;
        }

        .table tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table tbody tr:hover {
            background-color: #f1f1f1;
        }

        /* Kaldır Butonu Tasarımı */
        .table .btn-remove {
            background-color: #e74c3c;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 20px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
            transition: all 0.3s ease;
        }

        .table .btn-remove:hover {
            background-color: #c0392b;
        }

        .table .btn-remove i {
            font-size: 16px; /* İkon boyutunu ayarlıyoruz */
        }

        .error-message {
            color: #e74c3c;
            font-size: 14px;
            text-align: center;
            margin-top: 20px;
            display: none;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Web Config Dosyalarındaki Yetkili Kişiler</h2>

            <!-- Config Dosyası Seçimi Satırı -->
            <div class="form-row-config">
                <label for="ddlConfigFiles">Config Dosyasını Seçin:</label>
                <asp:DropDownList ID="ddlConfigFiles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlConfigFiles_SelectedIndexChanged">
                    <asp:ListItem Text="Bir config dosyası seçin" Value="" />
                </asp:DropDownList>
            </div>

            <!-- Diğer Kontrollerin Yer Aldığı Satır -->
            <div class="form-row">
                <label for="txtUsername">Yeni Kullanıcı Adı:</label>
                <asp:TextBox ID="txtUsername" runat="server" Placeholder="Kullanıcı adı girin..." />

                <label for="ddlAction">İşlem Seçin:</label>
                <asp:DropDownList ID="ddlAction" runat="server">
                    <asp:ListItem Text="Allow" Value="allow" />
                    <asp:ListItem Text="Deny" Value="deny" />
                </asp:DropDownList>

                <asp:Button ID="btnAddUser" runat="server" Text="Kullanıcı Ekle" OnClick="btnAddUser_Click" CssClass="btn" />
            </div>

            <!-- Kullanıcı Listesi Tablosu -->
            <div class="grid-container">
                <asp:GridView ID="gvAuthorizedUsers" runat="server" AutoGenerateColumns="false" CssClass="table" OnRowCommand="gvAuthorizedUsers_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Username" HeaderText="Kullanıcı Adı" SortExpression="Username" />
                        <asp:BoundField DataField="Action" HeaderText="Aksiyon" SortExpression="Action" />
                        <asp:ButtonField CommandName="RemoveUser" Text="Kaldır" ButtonType="Button" HeaderText="İşlem" ItemStyle-CssClass="btn-remove"/>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="errorMessage" class="error-message" runat="server" visible="false"></div>
        </div>
    </form>
</body>
</html>
