<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="windows_users._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>All VMs</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/table-style.css?v=1.0.0" />
    <style>
        .table-container{
            width:50%;
            margin:auto;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1 class="baslik" id="baslik" runat="server">Example</h1>

        </div>
        <div class="table-container">
            <div class="table-top">
                <h2 id="rowCounter"></h2>
                <button id="reset-button">Reset</button>
            </div>
            <div class="tabs"></div>
            <table id="contentTable">
                <thead>
                    <tr>
                        <th class="dropdown">
                            Host<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="nameDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                        <th class="dropdown">
                            User<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="userDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server">
                </tbody>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>

        <script src="tableOrganizer.js"></script>
        <script>
            function add1000rowsTable() {
                for (let i = 0; i < 1000; i++) {
                    
                }
            }
            add1000rowsTable();
            initializeTable();

        </script>
    </form>
</body>

</html>
