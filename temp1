<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmScreen.aspx.cs" Inherits="vminfo.vmScreen" %>

<!DOCTYPE html>
<head runat="server">
    <style>
        *,
        *:before,
        *:after {
            margin: 0;
            font-family: Verdana;
            
            box-sizing: border-box;
        }

        body {
            font-family: Verdana, Geneva, Tahoma, sans-serif;

            background-color: #F8F9FA;
        }

        .boldText {
            text-transform: uppercase;
            margin-bottom: 5px;
            font-weight: bold;
            font-size: small;
        }

        .blueText {
            letter-spacing: 2px;
            color: #54AFE4;
            font-weight: bold;
            margin-bottom: 10px;
            text-transform: uppercase;
        }
        .linkText {
            color: #54AFE4;
            text-decoration:underline;
            margin-bottom: 5px;
            font-weight: bold;
            cursor:pointer;
        }

        .header {
            overflow: hidden;
            background-color: #212529;
            padding: 10px 10px;
        }

            .header h1 {
                margin: 20px;
                float: left;
                text-align: center;
                text-decoration: none;
                line-height: 10px;
                border-radius: 4px;
                color: #F8F9FA;
            }

        .columnLeft {
            float: left;
            width: 45%;
            margin-top: 20px;
            margin-left: 10px;
        }

        .columnRight {
            float: left;
            width: 50%;
            margin-top: 20px;
            margin-left: 10px;
        }

        .panelsRight {
            height: 300px;
            background-color: #F8F9FA;
            overflow-y: auto;
            border-radius: 0px;
            box-shadow: 0px 0px 10px #888;
            border-color: #EF5160;
            margin-bottom: 10px;
            margin-left: 5px;
            margin-right: 5px;
            padding: 5px;
        }

        .detailsTable {
            border-collapse: collapse;
            width: 100%;
            border: 1px solid #ddd;
            font-size: 12px;
        }

        .detailsTable th, .detailsTable td {
            text-align: left;
            padding: 8px;
        }

        .detailsTable tr {
            border-bottom: 1px solid #ddd;
        }

        .detailsTable th:hover {
            cursor: pointer;
        }

        .detailsTable tr:hover {
            background-color: #dee2e6;
        }


        .exportButton{
            font-size:14px;
            padding: 10px 20px;
        }

    </style>    
    <title>VM Screen</title>

</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1 class="baslik" id="baslik" runat="server">Host Informations</h1>
        </div>
        <div>
            <div class="columnLeft">
                <div class="panelsRight" style="height:auto">

                    <table>
                        <tr>
                            <td class="boldText">Power:</td>
                            <td id="power" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host:</td>
                            <td id="host" class="linkText" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Cluster:</td>
                            <td id="cluster" class="linkText" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="boldText">Created Date:</td>
                            <td id="createdDate" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">OS:</td>
                            <td id="os" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="boldText">Data Center:</td>
                            <td id="datacenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">vCenter:</td>
                            <td id="vcenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average Network Usage :</td>
                            <td id="averageNetwork" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average Disk Usage :</td>
                            <td id="averageDisk" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="boldText">Average Memory Usage :</td>
                            <td id="averageMemory" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average CPU Usage :</td>
                            <td id="averageCpu" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host Model:</td>
                            <td id="hostmodel" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">NumCpu:</td>
                            <td id="numcpu" runat="server"></td>
                        </tr>
                    
                        <tr>
                            <td class="boldText">Cores Per Socket:</td>
                            <td id="corespersocket" runat="server"></td>
                        </tr>
                         <tr>
                            <td class="boldText">Total Disk GB:</td>
                            <td id="totaldisk" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Total Memory:</td>
                            <td id="totalmemory" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Last Updated Date:</td>
                            <td id="lasttime" runat="server"></td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <br />
                    <br />
                    <table>
                       <tr>
                            <td class="boldText">VM Notes:</td>
                            <td id="notes" runat="server"></td>
                        </tr>
                    </table>
                    <br />
                    <asp:Button ID="exportButton" runat="server" Text="Export to Excel" OnClick="exportToExcelClick"  CssClass="exportButton" />
                </div>

                <div class="panelsRight" style="height:400px">
                    <p class="blueText">last event logs</p>
                    <table id="eventsTable" class="detailsTable" runat="server">
                        <thead>
                        <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">Event Date</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Event</th>  

                        </tr>
                        </thead>
                        <tbody>
                               
                        </tbody>
                    </table>
                </div>

            </div>

            <div class="columnRight">
                <div class="panelsRight">
                    <p class="blueText">Network Cards</p>
                    <table id="networkTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">Name</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Network</th>
                            <th style="width:auto;" onclick="sortTable(2,this)">MAC</th>
                            <th style="width:auto;" onclick="sortTable(2,this)">Type</th>
                        </tr>
                        </thead>
                        <tbody>
                            
                        </tbody>
                    </table>
                </div>

                <div class="panelsRight">
                    <p class="blueText">disks</p>
                    <table id="disksTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">Name</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Capacity</th>
                            <th style="width:auto;" onclick="sortTable(2,this)">Storage Format</th>
                            <th style="width:auto;" onclick="sortTable(3,this)">Datastore</th>
                        </tr>
                        </thead>
                        <tbody>
                            
                        </tbody>
                    </table>
                </div>

            </div>
        </div>
        <script>
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get("id");
        
            const baslik = document.getElementsByClassName("baslik")[0];
            baslik.textContent = id;
        </script>
        <script>
            function sortTable(columnIndex,column) {
                var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
                table = column.closest('table');
                switching = true;
                dir = "asc";
                while (switching) {
                    switching = false;
                        rows = table.rows;

                        for (i = 1; i < rows.length - 1; i++) {
                            shouldSwitch = false;
                            x = rows[i].getElementsByTagName("td")[columnIndex];
                            y = rows[i + 1].getElementsByTagName("td")[columnIndex];

                            if (dir == "asc") {
                                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                                    shouldSwitch = true;
                                    break;
                                }
                            } else if (dir == "desc") {
                                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                                    shouldSwitch = true;
                                    break;
                                }
                            }
                        }

                        if (shouldSwitch) {
                            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
                            switching = true;
                            switchcount++;
                        } else {
                            if (switchcount == 0 && dir == "asc") {
                                dir = "desc";
                                switching = true;
                            }
                        }
                    }
            }

            document.getElementById("host").addEventListener("click", function () {
                window.location.href = "vmhostScreen.aspx?id=" + document.getElementById("host").textContent;
            });
            document.getElementById("cluster").addEventListener("click", function () {
                window.location.href = "clusterScreen.aspx?id=" + document.getElementById("cluster").textContent;
            });
            
        </script>
        <script>
            function baslikRenklendir() {
                var diskPanels = document.getElementsByClassName("diskPanel");
                for (i = 0; i < diskPanels.length; i++) {
                    var panel = diskPanels[i];
                    var percentText = panel.querySelector('.percentUsed');
                    var text = percentText.innerText;
                    var percent = parseInt(text.match(/%(\d+)/)[1]);
                    var color = 'rgb(' + percent + '%,' + (100 - percent) + '%,0%';
                    panel.querySelector(".diskPanelHead").style.backgroundColor = color;
                }
            }
        </script>
    </form>
</body>
</html>

