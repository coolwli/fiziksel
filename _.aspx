<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="odmvms._default" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ODM Replicated VMs</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/table-style.css" />
    <style>
        #admin-button{
            margin-left: 10px;
            background-color: #ea4242;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div id="logo"></div>

            <div>
                <h1 class="baslik" id="baslik" runat="server">Pendik Production & Test Replicated VMs</h1>
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
                <% if(User.IsInRole("GT-Agile CloudUnited")){  %>
                <button id="admin-button">CloudUnited Admin Control !</button>
                <% } %>
                <asp:HiddenField ID="hiddenField" runat="server" />
                <asp:Button ID="hiddenButton" runat="server" OnClick="hiddenButton_Click" Style="display:none;" />
            </div>
            <div class="tabs"></div>
            <table id="contentTable">
                <thead>
                    <tr>
                        <th class="dropdown">
                            Name<span class="dropdown-arrow">&#9660;</span>
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
                            Power State<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="psDropdown">
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
                            IPv4<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ipDropdown">
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
                            OS<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="osDropdown">
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
                            Cluster<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="clDropdown">
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
                            VCenter<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="vcDropdown">
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
                            DataStore<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="datastore">
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
            <p class="footer">© 2024 - Agile CloudUnited Team</p>
        </footer>

        <script src="table_organizer.js?v=1.0.0"></script>
        <script>
            document.getElementById('logo').addEventListener('click', function () {
                window.location.href='/';
            });
        </script>
    </form>
</body>

</html>
