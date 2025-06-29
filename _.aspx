<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="cpuMonitor._default" Async="true"
%>

<html>
    <head runat="server">
        <title>All VMs</title>
        <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
        <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/table-style.css" />

        <style>
            #page-name {
                color: #ffd02e;
            }
            .table-top {
                margin-top: 10px;
            }
            #page-selector a {
                cursor: pointer;
            }
            #alarms {
                min-height: 200px;
                display: none;
            }
            .alarm-table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }
            .alarm-table th,
            .alarm-table td {
                border: 1px solid #ddd;
                padding: 12px;
                text-align: left;
            }
            .alarm-table th {
                background-color: #f2f2f2;
                font-weight: bold;
            }
            .alarm-table tr:nth-child(even) {
                background-color: #f9f9f9;
            }
            .alarm-table tr:hover {
                background-color: #f5f5f5;
            }
            .alarm-dropdown {
                width: 100%;
                padding: 5px;
                border: 1px solid #ccc;
                border-radius: 4px;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div class="header">
                <a href="/">cloudunited</a>
                <span id="page-name">VM CPU Monitoring</span>
            </div>
            <div id="page-selector">
                <a class="active" onclick="setTable(this)">Pendik</a>
                <a onclick="setTable(this)">Ankara</a>
                <a onclick="setTable(this)">Alarms</a>
            </div>

            <div id="tables-div">
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
                <table id="contentTable">
                    <thead id="table-head"></thead>
                    <tbody id="tableBody"></tbody>
                </table>
                <div class="pagination" id="pagination"></div>
            </div>
            <asp:UpdatePanel ID="alarms" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-top">
                        <h2>Alarms</h2>
                    </div>
                    <table id="alarmsTable" class="alarm-table">
                        <thead>
                            <tr>
                                <th>VM Name</th>
                                <th>vCenter</th>
                                <th>CPU Usage (%)</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="phAlarms" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <footer>
                <p class="footer">© 2024 - Cloud United Team</p>
            </footer>
            <script src="table_organizer.js?v=1.3"></script>
            <script>
                const sortableColumnTypes = {
                    numericColumns: [4, 5, 6],
                    dateColumns: [],
                    textwNumColumns: [],
                };
                let columns = [
                    { name: "name", label: "VM Name" },
                    { name: "host", label: "Parent Host" },
                    { name: "cluster", label: "Parent Cluster" },
                    { name: "vCenter", label: "Parent vCenter" },
                    { name: "cpu", label: "VM CPU Usage(GHZ)" },
                    { name: "host_cpu", label: "ESXi CPU Capacity(GHZ)" },
                    { name: "prc", label: "VM CPU Usage On ESXi (%)" },
                ];
            </script>
        </form>
    </body>
</html>
