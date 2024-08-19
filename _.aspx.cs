<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmscreen.aspx.cs" Inherits="vminfo.vmscreen" Async="true" %>

<html>
<head runat="server">
    <title>VM Screen</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="screenStyle.css" />
    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
    <style>
        .panel-group {
            gap: 5px;
            display: flex;
            font-size:14px;
            flex-direction: row;
            overflow: auto;
            margin-top: 5px;
        }
        .disk-panel{
            min-width: max-content;
            padding: 10px;
            border-radius: 8px;
            background-color: #fff;
            border: 1px solid #ddd;
            cursor:pointer;
        }

        .disk-panel.active {
            background-color: #cccccc;
            font-weight: bold;
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
                <div class="panel" style="height:auto; padding-bottom: 20px; ">
                    <div class="tabs">
                        <button class="back-button" onclick="event.preventDefault(); window.location.href='default.aspx'">Go Back</button>
                        <button class="tab-button active" onclick="openTab('dataColumn',this)">Data</button>
                        <button class="tab-button" onclick="openTab('performanceColumn',this)">Performance & Disk</button>
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
                    </table>
                    <div style="width:96%;margin:auto; margin-top:70px;">
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
            <div class="columnRight" id="performanceColumn" style="display:none; padding:3px;">
                <div class="dropdown-container">
                    <label for="timeRange">Select Time Range: </label>
                    <select id="timeRange" onchange="fetchDisk();">
                        <option value="1" selected>1 Day</option>
                        <option value="7">7 Days</option>
                        <option value="30">30 Days</option>
                        <option value="90">90 Days</option>
                        <option value="360">1 Year</option>
                    </select>
                </div>
                <canvas id="cpuChart" style="display:none;"></canvas>
                <canvas id="memoryChart" style="display:none;"></canvas>
                <canvas id="diskChart" style="display:none;"></canvas>
                <div class="panel-group" id="diskPanelContainer"></div>
            </div>
            

        </div> 
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>

        <script src="chartsOrganizer.js"></script>
        <script>
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get("id");

            const baslik = document.getElementById("baslik");
            baslik.textContent = id;

            const addClickListener = (elementId, url) => {
                document.getElementById(elementId).addEventListener("click", function () {
                    window.location.href = `${url}?id=${this.textContent}`;
                });
            };

            addClickListener("host", "hostscreen.aspx");
            addClickListener("cluster", "clusterscreen.aspx");

            function sortTable(columnIndex, column) {
                const table = column.closest('table');
                let rows = Array.from(table.rows).slice(1);
                let dir = "asc", switchcount = 0, switching = true;

                while (switching) {
                    switching = false;
                    for (let i = 0; i < rows.length - 1; i++) {
                        let xValue = rows[i].cells[columnIndex].innerText;
                        let yValue = rows[i + 1].cells[columnIndex].innerText;

                        if (!isNaN(xValue) && !isNaN(yValue)) {
                            xValue = parseFloat(xValue);
                            yValue = parseFloat(yValue);
                        }

                        if ((dir === "asc" && xValue > yValue) || (dir === "desc" && xValue < yValue)) {
                            [rows[i], rows[i + 1]] = [rows[i + 1], rows[i]];
                            switching = true;
                            switchcount++;
                            break;
                        }
                    }
                    if (!switchcount && dir === "asc") {
                        dir = "desc";
                        switching = true;
                    }
                }
                table.tBodies[0].append(...rows);
            }

            function openTab(panelId, button) {
                event.preventDefault();
                document.querySelectorAll(".columnRight").forEach(content => content.style.display = "none");
                document.querySelectorAll(".tab-button").forEach(tabButton => tabButton.classList.remove("active"));

                document.getElementById(panelId).style.display = "flex";
                button.classList.add("active");
            }
            
        </script>
        <script>
            const diskCtx = document.getElementById('diskChart').getContext('2d');
            let diskDataMap = {};
            let diskChart;
            const updateDiskPanels = (diskLabels) => {
                const container = document.getElementById('diskPanelContainer');
                container.innerHTML = '';

                diskLabels.forEach(label => {
                    const panel = document.createElement('div');
                    panel.className = 'disk-panel';
                    panel.setAttribute('data-disk', label);

                    const averageUsage = calculateAverage(diskDataMap[label]);

                    panel.innerHTML = 
                        `${label}: ${averageUsage}%`;

                    panel.addEventListener('click', () => handlePanelClick(label));
                    container.appendChild(panel);
                });
            };
            const updateDiskChart = (diskLabel) => {
                const diskData = diskDataMap[diskLabel];
                const filteredDiskData = diskData.slice(-filteredDates.length);
                const processedDiskData = processLargeData(filteredDiskData, filteredDates);
                if (diskChart) diskChart.destroy();
                if (processedDiskData.data.length>0)
                    diskChart = createChart(diskCtx, processedDiskData.data, processedDiskData.dates, `Disk (${diskLabel}) Usage`, 'rgba(255, 159, 64, 1)');
            };
            const handlePanelClick = (diskLabel) => {
                document.querySelectorAll('.disk-panel').forEach(panel => {
                    panel.classList.remove('active');
                });
                document.querySelector(`.disk-panel[data-disk="${diskLabel}"]`).classList.add('active');
                updateDiskChart(diskLabel);
            };

            const calculateAverage = (data) => {
                const sum = data.reduce((acc, d) => acc + d, 0);
                return (sum / data.length).toFixed(2); 
            };

            function fetchDisk() {
                fetchData();
                const diskLabels = Object.keys(diskDataMap);
                diskLabels.sort();
                updateDiskPanels(diskLabels);
                if (diskLabels.length > 0) 
                    handlePanelClick(diskLabels[0]);

            }
        </script>
        

    </form>
</body>
</html>
