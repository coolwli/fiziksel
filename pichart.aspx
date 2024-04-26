<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pichart.aspx.cs" Inherits="fizkselArayuz.pichart" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pie Chart Generator</title>
    <style>
        #chartContainer {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            margin-bottom: 20px;
            border-top: 1px solid #ccc;
            padding: 10px;
        }
        #chartContainer div{
            margin:20px;
        } 
    </style>
    
</head>

<body>
    <form id="form1" runat="server">

    <div>
        <asp:HiddenField ID="HiddenField1" runat="server" Value="of"/>
        <h1 style="text-align:center">Pi Charts</h1>
        <div id="chartContainer" runat="server"></div>
    </div>
    <script>
        document.getElementById('HiddenField1').value = localStorage.getItem('dizi');

        var uniqueValues = JSON.parse(localStorage.getItem('dizi'));

        Array.from(uniqueValues).forEach(values => {
            console.log(values.column);
            console.log(values.values);
        });
    </script>

    </form>
</body>

</html>
