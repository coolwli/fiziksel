<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="windows_users._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Administrator Users</title>
    <link rel="stylesheet" type="text/css" href="default_style.css" />
    <link rel="stylesheet" type="text/css" href="table_style.css" />
    <style>
        .header {
            overflow: hidden;
            background-color: var(--primary-color);
        }

        .header a {
            color: white;
            text-decoration: none;
            float: left;
            padding: 10px 15px;
            line-height: 10px;
            position: relative;
            text-align: center;
            border-radius: 4px;
            font-size:11px;
        }

        .header a:hover {
            background-color: #383d42;
        }

        .header a.active {
            background-color: #383d42;
            color: white;
        }

        .header a:not(:last-child)::after {
            content: '';
            position: absolute;
            top: 50%;
            right: -1px;
            height: 20px;
            width: 2px;
            background-color: white;
            transform: translateY(-50%);
        }
        .table-container{
            width:60%;
            margin:auto;
        }

        #contentTable td {
            border: 1px solid #ccc;
            padding: 8px;
            text-align: left;
        }
        #contentTable tr:hover {
            background-color: #e2e2e2;
            cursor:default;
        }
        .row-color-1{
            background-color:#e9e9e9 !important;
        }
        .row-color-2{
            background-color:#ffffff !important;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div>
                <a class="active">
                    <h2>Administrators</h2>
                </a>
                <a href="remote.aspx">
                    <h2>Remote Desktop Users</h2>
                </a>
                <a href="local.aspx">
                    <h2>Local Users</h2>
                </a>
                <a href="windowsnetwork.aspx">
                    <h2>Windows Networks</h2>
                </a>
            </div>
        </div>
        <div class="table-container">
            <div class="table-top">
                <h2 id="rowCounter"></h2>
                <div class="button-container">
                    <div class="button active" onclick="setRowsPerPage(this)">20</div>
                    <div class="button" onclick="setRowsPerPage(this)">50</div>
                    <div class="button" onclick="setRowsPerPage(this)">100</div>
                </div>
                <button id="reset-button">Reset</button>
                <button id="export-button">Export</button>
                <asp:HiddenField ID="hiddenField" runat="server" />
                <asp:Button ID="hiddenButton" runat="server" OnClick="hiddenButton_Click" Style="display:none;" />
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
                            </div>
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
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server">
                </tbody>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>
        <footer>
            <p class="footer">© 2024 - Agile WinWin Team</p>
        </footer>

        <script src="table_organizer.js"></script>
    </form>
</body>

</html>
