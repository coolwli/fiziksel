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
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
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
            <div id="alarms-div">
                <h2>Alarms</h2>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:PlaceHolder ID="phAlarms" runat="server">
                            <div class="alarm">
                                <span class="vm-name">example1</span>
                                <span class="vCenter">example2</span>
                                <span class="percentage">example3</span>
                                <asp:DropDownList ID="durumddl" runat="server" AutoPostBack="true" OnSelectedIndexChanged="durumddl_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:ListItem Text="Option 1" Value="1" />
                                    <asp:ListItem Text="Option 2" Value="2" />
                                    <asp:ListItem Text="Option 3" Value="3" />
                                </asp:DropDownList>
                            </div>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <footer>
                <p class="footer">© 2024 - Cloud United Team</p>
            </footer>
            <script src="table_organizer.js?v=1.21"></script>
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
