
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="officetoWeb._default" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pie Chart Generator</title>
    <style>
        #chartsContainer {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            margin-bottom: 20px;
            border-top: 1px solid #ccc;
            padding: 10px;
        }
        #chartsContainer div{
            margin:20px;
        } 
    </style>
    
</head>

<body>
    <form id="form1" runat="server">
        <div>
            <input type="hidden" value="some hidden controls value" id="hdnTestControl" runat="server" name="hdnTestControl"/>
            <h1 style="text-align:center">Pi Charts</h1>
            <asp:Button ID="btnPostBack" runat="server" Text="PostBack" Style="display: none;" OnClick="btnPostBack_Click" />

            <div id="chartsContainer" runat="server"></div>
        </div>
    <script>
        var isReloaded = false;
        window.onload = function () {
            isReloaded = true;
        };

        var uniqueValues = JSON.parse(localStorage.getItem('dizi'));
        hdnInput = document.getElementById('hdnTestControl');
        hdnInput.value = null;

        Array.from(uniqueValues).forEach(values => {
            hdnInput.value += values.column + "~";
            Array.from(values.values).forEach(row => {
                hdnInput.value += row.value + "!" + row.count + "%";
            });
            hdnInput.value += "\n"
        });
        if (!isReloaded) {
            console.log("ilk");
            document.getElementById('<%= btnPostBack.ClientID %>').click();
        }
        else {
            console.log("son");
        }
    </script>

    </form>
</body>

</html>
