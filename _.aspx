<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmscreen.aspx.cs" Inherits="vminfo.vmscreen" Async="true" %>

<html>
<head runat="server">
    <style>
        
    </style>    
    <title>VM Screen</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="screenStyle.css" />

</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1 class="baslik" id="baslik" runat="server">Host Informations</h1>
        </div>
        <div class="container">
            <div class="columnLeft">
                <div class="panel" style="height:auto; padding-bottom: 20px; ">
                    <div class="tabs">
                        <button class="back-button" onclick="event.preventDefault(); window.location.href='default.aspx'">Go Back</button>
                        <button class="tab-button active" onclick="openTab('dataColumn',this)">Datas</button>
                        <button class="tab-button" onclick="openTab('performanceColumn',this)">Performances</button>
                    </div>

                    <table>
                        <tr>
                            <td class="bold-text">Power:</td>
                            <td id="power" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Host:</td>
                            <td id="host" class="link-text" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Cluster:</td>
                            <td id="cluster" class="link-text" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="bold-text">Created Date:</td>
                            <td id="createdDate" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">OS:</td>
                            <td id="os" runat="server"></td>
                        </tr>

                        <tr>
                            <td class="bold-text">Data Center:</td>
                            <td id="datacenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">vCenter:</td>
                            <td id="vcenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Host Model:</td>
                            <td id="hostmodel" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">CPU:</td>
                            <td id="numcpu" runat="server"></td>
                        </tr>
                    
                        <tr>
                            <td class="bold-text">Memory:</td>
                            <td id="totalmemory" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                    </table>
                    <div style="width:96%;margin:auto;">
                        <div class="disk-info">
                            <p class="disk-used" id="usedDisk" runat="server">Used Disk: <strong>300 GB</strong></p>
                            <p class="disk-free" id="freeDisk" runat="server">Free Disk: <strong>200 GB</strong></p>
                        </div>
                        <div class="bar">
                            <span style="max-width: 100%" id="usedPercentage" runat="server"></span>
                            <div class="bar-label" id="usedBar" runat="server">60%</div>
                        </div>
                        <p style="font-size: 12px; color: #6c757d; text-align: center;" id="totalDisk" runat="server">Total Disk: <strong>500 GB</strong></p>
                    </div>         
                    <table style="margin-top:30px;">
                       <tr>
                            <td class="bold-text">VM Notes:</td>
                            <td id="notes" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Last Updated Date:</td>
                            <td id="lasttime" runat="server"></td>
                        </tr>
                    </table>

                </div>
            </div>

            <div class="columnRight" id="dataColumn">
                <div class="panel">
                    <p class="blue-text">Network Cards</p>
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

                <div class="panel">
                    <p class="blue-text">disks</p>
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
                <div class="panel" style="height: 360px;">
                <p class="blue-text">last event logs</p>
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
            <div class="columnRight" id="performanceColumn" style="display:none;">
                a
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
       
            function sortTable(columnIndex, column) {
                var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
                table = column.closest('table');
                switching = true;
                dir = "asc";
                rows = Array.from(table.rows).slice(1);
    
                while (switching) {
                    switching = false;

                    for (i = 0; i < rows.length - 1; i++) {
                        shouldSwitch = false;
                        xValue = rows[i].getElementsByTagName("td")[columnIndex].innerText;
                        yValue = rows[i + 1].getElementsByTagName("td")[columnIndex].innerText;
            
                        if (!isNaN(xValue) && !isNaN(yValue)) {
                            xValue = parseFloat(xValue);
                            yValue = parseFloat(yValue);
                        }
                        if (dir == "asc") {
                            if (xValue > yValue) {
                                shouldSwitch = true;
                                break;
                            }
                        } else if (dir == "desc") {
                            if (xValue < yValue) {
                                shouldSwitch = true;
                                break;
                            }
                        }
                    }
                    if (shouldSwitch) {
                        [rows[i], rows[i + 1]] = [rows[i + 1], rows[i]];
                        switching = true;
                        switchcount++;
                    } else {
                        if (switchcount == 0 && dir == "asc") {
                            dir = "desc";
                            switching = true;
                        }
                    }
                }

                for (i = 0; i < rows.length; i++) {
                    table.appendChild(rows[i]);
                }
            }

            function openTab(panelId, button) {
                event.preventDefault();
                var i, tabcontent, tabbuttons;
                tabcontent = document.getElementsByClassName("columnRight");
                tabbuttons = document.getElementsByClassName("tab-button");

                for (i = 0; i < tabcontent.length; i++) {
                    tabcontent[i].style.display = "none";
                }
                for (i = 0; i < tabbuttons.length; i++) {
                    tabbuttons[i].classList.remove("active");
                }

                document.getElementById(panelId).style.display = "block";
                button.classList.add("active");
            }

            document.getElementById("host").addEventListener("click", function () {
                window.location.href = "hostscreen.aspx?id=" + document.getElementById("host").textContent;
            });
            document.getElementById("cluster").addEventListener("click", function () {
                window.location.href = "clusterscreen.aspx?id=" + document.getElementById("cluster").textContent;
            });
        </script>

    </form>
</body>
</html>
