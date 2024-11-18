<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="webconfigs._default" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Web Config Yetkili Kişiler</title>
    <style>
        /* Genel stil ayarları */
        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f4f7fc;
            margin: 0;
            padding: 0;
            color: #333;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            overflow: hidden;
        }

        .container {
            width: 100%;
            max-width: 900px;
            background-color: #ffffff;
            padding: 40px;
            border-radius: 16px;
            box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
            margin: 20px;
            overflow: hidden;
        }

        h2 {
            text-align: center;
            font-size: 32px;
            color: #4f4f4f;
            margin-bottom: 40px;
            font-weight: 600;
        }

        /* Form alanları */
        .form-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
        }

        .form-row label {
            font-weight: 500;
            color: #5a5a5a;
            width: 35%;
            font-size: 16px;
            line-height: 36px;
        }

        .form-row input, .form-row select {
            width: 60%;
            padding: 14px 20px;
            font-size: 16px;
            border: 1px solid #d3d9e1;
            border-radius: 8px;
            background-color: #fafbff;
            color: #4f4f4f;
            transition: all 0.3s ease;
        }

        .form-row input:focus, .form-row select:focus {
            border-color: #5c6bc0;
            box-shadow: 0 0 5px rgba(92, 107, 192, 0.5);
            outline: none;
        }

        .error-message {
            color: #e74c3c;
            background-color: #f8d7da;
            padding: 15px;
            border-radius: 8px;
            margin-top: 20px;
            font-weight: 600;
            text-align: center;
            display: none;
        }

        /* Tablo (GridView) */
        .table-container {
            margin-top: 30px;
            padding-bottom: 20px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th, td {
            padding: 16px;
            text-align: left;
            font-size: 16px;
            border-bottom: 1px solid #e4e8f1;
        }

        th {
            background-color: #f7f7f9;
            color: #5a5a5a;
            font-weight: 600;
        }

        tr:nth-child(even) {
            background-color: #f9f9fb;
        }

        tr:hover {
            background-color: #f2f3f7;
        }

        /* Button styles */
        .btn-remove {
            background-color: #ff6f61;
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            border: none;
            transition: background-color 0.3s;
        }

        .btn-remove:hover {
            background-color: #e5554e;
        }

        .btn {
            background-color: #6a60f1;
            color: white;
            padding: 16px 24px;
            font-size: 18px;
            font-weight: 600;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            width: 100%;
            margin-top: 30px;
            transition: background-color 0.3s;
        }

        .btn:hover {
            background-color: #5a50e3;
        }

        .btn:active {
            background-color: #4c44c6;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Web Config Dosyasındaki Yetkili Kişiler</h2>

            <!-- Config Dosyasını Seçme -->
            <div class="form-row">
                <label for="ddlConfigFiles">Config Dosyasını Seçin:</label>
                <asp:DropDownList ID="ddlConfigFiles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlConfigFiles_SelectedIndexChanged">
                    <asp:ListItem Text="Bir config dosyası seçin" Value="" />
                </asp:DropDownList>
            </div>

            <!-- Kullanıcı Listesi (GridView ile) -->
            <div class="table-container">
                <asp:GridView ID="gvAuthorizedUsers" runat="server" AutoGenerateColumns="False" 
                    OnRowCommand="gvAuthorizedUsers_RowCommand" CssClass="table" EmptyDataText="Hiç kullanıcı yok">
                    <Columns>
                        <asp:BoundField DataField="UserName" HeaderText="Kullanıcı Adı" SortExpression="UserName" />
                        <asp:TemplateField HeaderText="İşlem">
                            <ItemTemplate>
                                <asp:Button ID="btnRemove" runat="server" Text="Kaldır" CommandName="Remove" 
                                    CommandArgument='<%# Eval("UserName") %>' CssClass="btn-remove" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Kullanıcı Adı Ekleme -->
            <div class="form-row">
                <label for="txtUserName">Kullanıcı Adı:</label>
                <asp:TextBox ID="txtUserName" runat="server" placeholder="Kullanıcı Adı Girin" />
            </div>
            <div class="form-row">
                <asp:Button ID="btnAddUser" runat="server" Text="Kullanıcı Ekle" CssClass="btn" OnClick="btnAddUser_Click" />
            </div>

            <!-- Hata Mesajı -->
            <div id="errorMessage" class="error-message" runat="server"></div>
        </div>
    </form>

</body>
</html>
