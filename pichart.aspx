<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="hostscreen.aspx.cs" Inherits="vminfo.hostscreen" %>

<html>
<head runat="server">  
    <title>Host Screen</title>
    <link rel="stylesheet" type="text/css" href="screenStyle.css" />
    <style>
        #vmsTable td:hover {
            cursor: pointer;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1 class="baslik" id="baslik" runat="server">Host Informations</h1>
        </div>
        <div class="container">
            <div class="columnLeft">
                <div class="panel" style="height:auto">

                    <table>

                        <tr>
                            <td class="boldText">Cluster:</td>
                            <td id="cluster" class="linkText" runat="server"></td>
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
                            <td class="boldText">Average Disk Usage:</td>
                            <td id="avdisk" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average Network Usage:</td>
                            <td id="avnet" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average Memory Usage :</td>
                            <td id="avmem" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Average CPU Usage :</td>
                            <td id="avcpu" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host Model:</td>
                            <td id="hostmodel" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Manufacturer:</td>
                            <td id="manufacturer" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host CPU Core:</td>
                            <td id="cpucore" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">CPU Mhz:</td>
                            <td id="cpumhz" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host Memory Size:</td>
                            <td id="memorysize" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="boldText">Host Nic Count:</td>
                            <td id="hostnic" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host HBAs:</td>
                            <td id="hbas" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="boldText">Host Uptime Days:</td>
                            <td id="uptimedays" runat="server"></td>
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

                    <asp:Button ID="exportButton" runat="server" Text="Export to Excel" OnClick="exportToExcelClick"  CssClass="exportButton" />

                </div>

                <div class="panel">
                    <p class="blueText">last event logs</p>
                    <table id="eventsTable" class="detailsTable" runat="server">
                        <thead>
                        <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">Event</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Event Date</th>  

                        </tr>
                        </thead>
                        <tbody>
                               
                        </tbody>
                    </table>
                </div>

            </div>

            <div class="columnRight">
                <div class="panel">
                    <p class="blueText">all vms</p>
                    <table id="vmsTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">Name</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Owner</th>
                            <th style="width:auto;" onclick="sortTable(2,this)">Power State</th>
                            <th style="width:auto;" onclick="sortTable(3,this)">Total Disk(GB)</th>
                        </tr>
                        </thead>
                        <tbody>
                            
                        </tbody>
                    </table>
                </div>
                <div class="panel">
                    <p class="blueText">cpus</p>
                    <table id="cpuTable" class="detailsTable" runat="server">
                        <thead>
                        <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">ID</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Name</th>  
                        </tr>
                        </thead>
                        <tbody>
                               
                        </tbody>
                    </table>

                </div>
                <div class="panel">
                    <p class="blueText">datastores</p>
                    <table id="dsTable" class="detailsTable" runat="server">
                        <thead>
                        <tr>
                            <th style="width:auto;" onclick="sortTable(0,this)">DataStore Name</th>
                            <th style="width:auto;" onclick="sortTable(1,this)">Capacity</th>  
                            <th style="width:auto;" onclick="sortTable(2,this)">Free Space</th>  
                            <th style="width:auto;" onclick="sortTable(3,this)">Used </th>  
                        </tr>
                        </thead>
                        <tbody>
                               
                        </tbody>
                    </table>
                </div>

            </div>

        </div>
        
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>
        <script>
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get("id");
        
            const baslik = document.getElementsByClassName("baslik")[0];
            baslik.textContent = id;
       
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

            
            document.getElementById("cluster").addEventListener("click", function () {
                window.location.href = "clusterscreen.aspx?id=" + document.getElementById("cluster").textContent;
            });

            document.addEventListener("DOMContentLoaded", function () {
                const tablo = document.getElementById("vmsTable");

                function satirTiklandi(event) {

                    const satir = event.currentTarget;
                    const id = satir.querySelector("td:first-child").textContent;

                    window.location.href="vmscreen.aspx?id=" + id;
                }
                const satirListesi = tablo.querySelectorAll("tbody tr");
                satirListesi.forEach(function (satir) {
                if (satir != satirListesi[0])
                    satir.addEventListener("click", satirTiklandi);
                });
            });
            
        </script>

    </form>
</body>
</html>
