<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmscreen.aspx.cs" Inherits="vmpedia.vmscreen" Async="true" %>

<html>
<head runat="server">
    <title>VM Screen</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />

    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
    <style>
        #page-name {
            color: #bf5af2;
        }
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

        body{
            background-color:#f2f2f2;
        }

        .container {
            display: flex;
            flex-wrap: wrap;
            margin-top:70px;
        }

        .columnLeft {
            width: 40%;
            margin-top: 20px;
            margin-left: auto;
        }
        .tabs {
            display: flex;
            background-color: #f7f7f7;
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }

        .back-button {
            margin-right: auto;
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            background-color: #ccc;
            color: #333;
            cursor: pointer;
            transition: background-color 0.3s;
        }

            .back-button:hover {
                background-color: #999;
            }

        .tab-button {
            padding: 10px 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color:  #ccc;
            cursor: pointer;
            transition: background-color 0.3s;
            color: #333;
            margin-left: 5px;
        }

            .tab-button.active,
            .tab-button:hover {
                background-color: var(--secondary-color);
            }


        .columnRight {
            display:flex;
            flex-direction:column;
            width: 58%;
            margin-top: 20px;
            margin-right: auto;
        }

        .link-text{
            cursor:text;
        }

        .panel {
            height: 300px;
            background-color: #ffffff;
            overflow-y: auto;
            border-color: #EF5160;
            margin-bottom: 25px;
            margin-left: 5px;
            margin-right: 5px;
            padding: 5px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }

        .disk-info {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            font-size: 12px;
        }

            .disk-info p {
                margin: 0;
            }


            .disk-info .disk-used {
                text-align: right;
            }

        .bar {
            background-color: #ccc;
            border-radius: 10px;
            height: 25px;
            overflow: hidden;
            position: relative;
            width: 100%;
            margin: 10px 0;
        }

            .bar span {
                display: block;
                height: 100%;
                width: 0;
                background-color: var(--secondary-color);
                position: absolute;
                top: 0;
                left: 0;
                transition: width 0.3s;
            }

        .bar-label {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            color: #fff;
            font-weight: bold;
            font-size: 14px;
        }

        .detailsTable {
            border-collapse: collapse;
            width: 100%;
            border: 1px solid #ddd;
            font-size: 10px;
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


        .dropdown-container {
            display: flex;
            justify-content: flex-end;
            background: #fff;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        label {
            font-size: 16px;
            color: #333;
            margin-right: 10px;
        }

        select {
            border-radius: 4px;
            border: 1px solid #ccc;
            background-color: #f9f9f9;
            font-size: 14px;
            outline: none;
            width: 120px;
            transition: border-color 0.3s;
            height: 25px;
        }

        select:focus {
            border-color: #007bff;
        }


        canvas {
            border-radius: 8px;
            background-color: #fff;
            border: 1px solid #ddd;
            min-width: 100%;
            max-height: 100%;
            box-sizing: border-box;
            margin-top:15px;
        }



    </style>    
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <a href="/">cloudunited</a>
            <span id="page-name"></span>
            <img id="logo" style="height: 50px;right: 40px;position: absolute;top: 0px;width: 50px;" src="https://cloudunited/Styles/logo.png">

        </div>
        <div class="container">
            <div class="columnLeft">
                <div class="panel" style="height:auto; padding-bottom: 20px; ">
                    <div class="tabs">
                        <button class="back-button" onclick="event.preventDefault(); window.location.href='default.aspx'">Go Back</button>
                    </div>

                    <table>
                        <tr>
                            <td class="bold-text">IPv4:</td>
                            <td id="ip" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Power State:</td>
                            <td id="power" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="bold-text">OS:</td>
                            <td id="os" runat="server"></td>
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
                            <td class="bold-text">Data Center:</td>
                            <td id="datacenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">vCenter:</td>
                            <td id="vcenter" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Owner:</td>
                            <td id="owner" runat="server"></td>
                        </tr>
                        <tr>
                            <td><br /></td>
                        </tr>
                        <tr>
                            <td class="bold-text">VM Tools Version:</td>
                            <td id="vmtools" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="bold-text">Created Date:</td>
                            <td id="createdDate" runat="server"></td>
                        </tr>

                        <tr>
                            <td><br /></td>
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
                </div>

                <div class="panel">
                    <p class="blue-text">disks</p>
                    <table id="disksTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                                <th style="width:auto;" onclick="sortTable(0,this)">Name</th>
                                <th style="width:auto;" onclick="sortTable(1,this)">Capacity</th>
                                <th style="width:auto;" onclick="sortTable(2,this)">Provisioning Type</th>
                            </tr>
                        </thead>
                        <tbody>
                            
                        </tbody>
                    </table>
                </div>

                <div class="panel">
                    <p class="blue-text">Tags</p>
                    <table id="tagsTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                                <th style="width:auto;" onclick="sortTable(0,this)">Tag Name</th>
                                <th style="width:auto;" onclick="sortTable(1,this)">Value</th>
                            </tr>
                        </thead>
                        <tbody>
                            
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="columnRight" style="padding:3px;">
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
                <div class="panel-group" id="diskPanelContainer" style="display:none;"></div>
            </div>
            

        </div> 
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>

        <script src="chartsOrganizer.js?v=1.3.0"></script>

        <script>
            function initializeProperties() {
                document.getElementById("page-name").textContent = vmProperties['config|name'];
                document.getElementById("createdDate").textContent = new Date(parseFloat(vmProperties['config|createDate'])).toLocaleString("tr-TR");
                document.getElementById("ip").textContent = vmProperties['summary|guest|ipAddress'];
                document.getElementById("power").textContent = vmProperties['summary|runtime|powerState'];
                document.getElementById("os").textContent = vmProperties['summary|guest|fullName'];
                document.getElementById("host").textContent = vmProperties['summary|parentHost'];
                document.getElementById("cluster").textContent = vmProperties['summary|parentCluster'];
                document.getElementById("datacenter").textContent = vmProperties['summary|parentDatacenter'];
                document.getElementById("vcenter").textContent = vmProperties['summary|parentVcenter'];
                document.getElementById("owner").textContent = vmProperties['summary|parentFolder'];
                document.getElementById("vmtools").textContent = vmProperties['summary|guest|toolsVersion'];
                document.getElementById("numcpu").innerHTML ="<b>"+ vmProperties['config|hardware|numSockets']+"</b> Socket(s) <b>"+ vmProperties['config|hardware|numCpu'] + "</b> Core";
                document.getElementById("totalmemory").textContent = parseFloat(vmProperties['config|memoryAllocation|shares|shares'])/10240 +" GB"; 
                document.getElementById("usedDisk").textContent = parseFloat(vmProperties['guestfilesystem|usage_total']).toFixed(1) + " GB";
                document.getElementById("freeDisk").textContent = parseFloat(vmProperties['config|hardware|diskSpace'] - vmProperties['guestfilesystem|usage_total']).toFixed(1)+ " GB";
                document.getElementById("totalDisk").textContent = parseFloat(vmProperties['config|hardware|diskSpace']).toFixed(1)+ " GB";
                document.getElementById("usedPercentage").style["width"] = vmProperties['guestfilesystem|percentage_total']+"%" ;
                document.getElementById("usedBar").textContent = parseFloat(vmProperties['guestfilesystem|percentage_total']).toFixed(1) + "%";

                const tagTable = document.getElementById("tagsTable");
                const diskTable = document.getElementById("disksTable");
                let disks = {};

                Object.entries(vmProperties).forEach(([key,value]) => {
                    if (key.startsWith("virtualDisk")) {
                        let diskKey = key.split("|")[0];
                        let property = key.split("|")[1];

                        if (!disks[diskKey]) {
                            disks[diskKey] = { label: "", confGB: "", type: "" };
                        }

                        if (property == "label") disks[diskKey].label = value;
                        else if (property == "configuredGB") disks[diskKey].confGB = value;
                        else if (property == "provisioning_type") disks[diskKey].type = value;


                    }
                    else if (key.startsWith("summary|customTag")) {
                        let row = document.createElement("tr");

                        let keyCell = document.createElement("td");
                        keyCell.textContent = key.split(':')[1].split('|')[0];

                        let valueCell = document.createElement("td");
                        valueCell.textContent = value;

                        row.appendChild(keyCell);
                        row.appendChild(valueCell);

                        tagTable.appendChild(row);
                    }
                });

                Object.values(disks).forEach(disk => {
                    if (disk.label == "[Deleted]") return;
                    let row = document.createElement("tr");

                    let c1 = document.createElement("td");
                    c1.textContent = disk.label;

                    let c2 = document.createElement("td");
                    c2.textContent = disk.confGB;

                    let c3 = document.createElement("td");
                    c3.textContent = disk.type;

                    row.appendChild(c1);
                    row.appendChild(c2);
                    row.appendChild(c3);

                    diskTable.appendChild(row);
                });

            }
        </script>
       
        

    </form>
</body>
</html>
