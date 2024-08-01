<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmscreen.aspx.cs" Inherits="vminfo.vmscreen" Async="true" %>

<html>
<head runat="server">
    <style>
        
    </style>    
    <title>VM Screen</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="screenStyle.css" />
    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
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
                        <button class="tab-button active" onclick="openTab('dataColumn',this,0)">Datas</button>
                        <button class="tab-button" onclick="openTab('performanceColumn',this,1)">Performances</button>
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
                
                <div class="chart-panel">
                    <canvas id="cpuChart"></canvas>
                    <canvas id="memoryChart"></canvas>
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

            function openTab(panelId, button,performanceButton) {
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
        <script>
            
            const createChart = (ctx, data, dates, label, borderColor) => {
                const min = Math.min(...data.map(d => d.y));
                const max = Math.max(...data.map(d => d.y));

                const minIndex = data.findIndex(d => d.y === min);
                const maxIndex = data.findIndex(d => d.y === max);
                
                const config = {
                    type: 'line',
                    data: {
                        labels: dates,
                        datasets: [
                            {
                                label: 'Minimum: %' + min,
                                data: [{
                                    x: dates[minIndex],
                                    y: min
                                }],
                                borderColor: 'red',
                                fill: true,
                                pointRadius: 6,
                                pointHoverRadius: 4,
                                pointBackgroundColor: 'red'
                            }, {
                                label: 'Maximum: %' + max,
                                data: [{
                                    x: dates[maxIndex],
                                    y: max
                                }],
                                borderColor: 'black',
                                fill: true,
                                pointRadius: 6,
                                pointHoverRadius: 4,
                                pointBackgroundColor: 'black'
                            },
                            {
                                label: label,
                                data: data,
                                borderColor: borderColor,
                                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                                fill: true,
                                pointRadius: 0,
                                pointHoverRadius: 4,
                                borderWidth: 1,
                                tension: 0.01
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            title: {
                                display: true,
                                text: `${label}`
                            },
                        },
                        interaction: {
                            intersect: false,
                        },
                        scales: {
                            x: {
                                type: 'time',
                                time: {
                                    unit: 'day'
                                },
                                display: true,
                                title: {
                                    display: true,
                                    text: 'Date'
                                }
                            },
                            y: {
                                display: true,
                                title: {
                                    display: true,
                                    text: 'Value'
                                },
                                suggestedMin: Math.max(0, Math.min(...data.map(d => d.y)) - 10),
                                suggestedMax: Math.max(...data.map(d => d.y)) + 10
                            }
                        }
                    },
                };
                console.log(config.data.labels.length);
                return new Chart(ctx, config);
            };

            const fetchData = async () => {
                const processedCPUData = processLargeData(cpuDatas, dates);
                const processedMemoryData = processLargeData(memoryDatas, dates);

                createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
                createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
            };

            const processLargeData = (data, dates) => {
                if (data.length <= 250) {
                    return {
                        data: data.map((value, index) => ({
                            x: dates[index],
                            y: value
                        })),
                        dates: dates
                    };
                }

                const reducedData = [];
                const reducedDates = [];
                const step = Math.ceil(data.length / 250);
                let sum = 0;
                let count = 0;
                let sumDates = 0;

                for (let i = 0; i < data.length; i++) {
                    sum += data[i];
                    sumDates +=new Date(dates[i]).getTime(); // Add date as time in milliseconds
                    count++;

                    if ((i + 1) % step === 0) {
                        const average = sum / count;
                        const averageDate = new Date(sumDates / count); // Calculate average date
                        reducedData.push({
                            x: averageDate,
                            y: average
                        });
                        reducedDates.push(averageDate);
                        sum = 0;
                        sumDates = 0;
                        count = 0;
                    }
                }

                // Handle any remaining data after the loop
                if (count > 0) {
                    const average = sum / count;
                    const averageDate = new Date(sumDates / count);
                    reducedData.push({
                        x: averageDate,
                        y: average
                    });
                    reducedDates.push(averageDate);
                }

                // Handle min and max values explicitly
                const min = Math.min(...data);
                const max = Math.max(...data);
                const minIndex = data.indexOf(min);
                const maxIndex = data.indexOf(max);

                // Remove old min/max data if they exist
                const minDate = new Date(dates[minIndex]);
                const maxDate = new Date(dates[maxIndex]);

                // Ensure min/max data is not duplicated
                const isMinInReducedData = reducedData.some(d => d.x.getTime() === minDate.getTime());
                const isMaxInReducedData = reducedData.some(d => d.x.getTime() === maxDate.getTime());

                if (!isMinInReducedData) {
                    reducedData.push({ x: minDate, y: min });
                    reducedDates.push(minDate);
                }
                if (!isMaxInReducedData) {
                    reducedData.push({ x: maxDate, y: max });
                    reducedDates.push(maxDate);
                }

                // Sort by date to ensure the order is correct
                reducedData.sort((a, b) => a.x - b.x);
                reducedDates.sort((a, b) => a - b);

                return { dates: reducedDates, data: reducedData };
            };


            const cpuCtx = document.getElementById('cpuChart').getContext('2d');
            const memoryCtx = document.getElementById('memoryChart').getContext('2d');

        </script>

    </form>
</body>
</html>
