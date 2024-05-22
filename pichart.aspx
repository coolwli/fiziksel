<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmhostScreen.aspx.cs" Inherits="vminfo.vmhostScreen" %>

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

        #vmsTable td:hover {
            cursor: pointer;
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
    <title>VM Host Screen</title>

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

                <div class="panelsRight">
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
                <div class="panelsRight">
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
                <div class="panelsRight">
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
                <div class="panelsRight">
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

            document.getElementById("cluster").addEventListener("click", function () {
                window.location.href = "clusterScreen.aspx?id=" + document.getElementById("cluster").textContent;
            });
        </script>

        <script>
            document.addEventListener("DOMContentLoaded", function () {
                const tablo = document.getElementById("vmsTable");

                function satirTiklandi(event) {

                    const satir = event.currentTarget;
                    const id = satir.querySelector("td:first-child").textContent;

                    window.location.href="vmScreen.aspx?id=" + id;
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
