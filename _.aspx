<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vmscreen.aspx.cs" Inherits="vmpedia.vmscreen" Async="true" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>VM Screen</title>
    <link rel="stylesheet" href="https://cloudunited/Styles/default-style.css" />
    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
    <script src="chartsOrganizer.js?v=1.3.0"></script>
    <style>
        body {
            background-color: #f2f2f2;
            margin: 0;
            font-family: Arial, sans-serif;
        }

        .header {
            background: #ffffff;
            padding: 15px;
            position: relative;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .header a {
            text-decoration: none;
            font-weight: bold;
            color: #333;
            font-size: 20px;
        }

        #page-name {
            color: #bf5af2;
            margin-left: 10px;
        }

        #logo {
            position: absolute;
            top: 0;
            right: 40px;
            width: 50px;
            height: 50px;
        }

        .container {
            display: flex;
            flex-wrap: wrap;
            margin-top: 70px;
        }

        .columnLeft {
            width: 40%;
            padding: 0 15px;
        }

        .columnRight {
            width: 58%;
            padding: 0 15px;
            display: flex;
            flex-direction: column;
        }

        .panel {
            background-color: #ffffff;
            padding: 15px;
            margin-bottom: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .tabs, .dropdown-container {
            display: flex;
            align-items: center;
            background-color: #f7f7f7;
            padding: 10px;
            border-bottom: 1px solid #ddd;
            border-radius: 5px;
        }

        .back-button {
            background-color: #cccccc;
            color: #333;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            transition: 0.3s;
        }

        .back-button:hover {
            background-color: #999999;
        }

        table {
            width: 100%;
            font-size: 12px;
        }

        table td {
            padding: 5px 0;
        }

        .disk-info {
            display: flex;
            justify-content: space-between;
            font-size: 12px;
            margin-top: 10px;
        }

        .bar {
            background-color: #cccccc;
            height: 25px;
            border-radius: 10px;
            margin: 10px 0;
            position: relative;
            overflow: hidden;
        }

        .bar span {
            background-color: #bf5af2;
            height: 100%;
            position: absolute;
            left: 0;
            top: 0;
            transition: width 0.3s;
        }

        .bar-label {
            position: absolute;
            width: 100%;
            text-align: center;
            line-height: 25px;
            color: #ffffff;
            font-weight: bold;
        }

        .detailsTable {
            width: 100%;
            border-collapse: collapse;
            font-size: 12px;
        }

        .detailsTable th, .detailsTable td {
            padding: 8px;
            border-bottom: 1px solid #ddd;
        }

        .detailsTable th:hover {
            cursor: pointer;
        }

        .detailsTable tr:hover {
            background-color: #dee2e6;
        }

        canvas {
            background-color: #ffffff;
            border: 1px solid #ddd;
            border-radius: 8px;
            margin-top: 15px;
            box-sizing: border-box;
        }

        .panel-group {
            display: flex;
            gap: 10px;
            overflow-x: auto;
        }

        .disk-panel {
            padding: 10px;
            border: 1px solid #ddd;
            background-color: #ffffff;
            border-radius: 8px;
            min-width: 120px;
            cursor: pointer;
            text-align: center;
        }

        .disk-panel.active {
            background-color: #cccccc;
            font-weight: bold;
        }

        label {
            margin-right: 10px;
        }

        select {
            padding: 4px;
            border-radius: 4px;
            border: 1px solid #ccc;
            background-color: #f9f9f9;
            font-size: 14px;
        }

        .footer {
            text-align: center;
            font-size: 12px;
            padding: 20px;
            background-color: #f2f2f2;
            color: #999999;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <a href="/">cloudunited</a>
            <span id="page-name"></span>
            <img id="logo" src="https://cloudunited/Styles/logo.png" alt="Logo" />
        </div>

        <div class="container">
            <div class="columnLeft">
                <div class="panel">
                    <div class="tabs">
                        <button class="back-button" onclick="event.preventDefault(); window.location.href='default.aspx'">Go Back</button>
                    </div>

                    <table>
                        <tr><td><b>IPv4:</b></td><td id="ip" runat="server"></td></tr>
                        <tr><td><b>Power State:</b></td><td id="power" runat="server"></td></tr>
                        <tr><td><b>OS:</b></td><td id="os" runat="server"></td></tr>
                        <tr><td><b>Host:</b></td><td id="host" runat="server"></td></tr>
                        <tr><td><b>Cluster:</b></td><td id="cluster" runat="server"></td></tr>
                        <tr><td><b>Data Center:</b></td><td id="datacenter" runat="server"></td></tr>
                        <tr><td><b>vCenter:</b></td><td id="vcenter" runat="server"></td></tr>
                        <tr><td><b>Owner:</b></td><td id="owner" runat="server"></td></tr>
                        <tr><td><b>VM Tools Version:</b></td><td id="vmtools" runat="server"></td></tr>
                        <tr><td><b>Created Date:</b></td><td id="createdDate" runat="server"></td></tr>
                        <tr><td><b>CPU:</b></td><td id="numcpu" runat="server"></td></tr>
                        <tr><td><b>Memory:</b></td><td id="totalmemory" runat="server"></td></tr>
                    </table>

                    <div class="disk-info">
                        <p id="usedDisk" runat="server"></p>
                        <p id="freeDisk" runat="server"></p>
                    </div>
                    <div class="bar">
                        <span id="usedPercentage" runat="server"></span>
                        <div class="bar-label" id="usedBar" runat="server"></div>
                    </div>
                    <p style="text-align:center;" id="totalDisk" runat="server"></p>
                </div>

                <div class="panel">
                    <p><b>Disks</b></p>
                    <table id="disksTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                                <th onclick="sortTable(0,this)">Name</th>
                                <th onclick="sortTable(1,this)">Capacity</th>
                                <th onclick="sortTable(2,this)">Provisioning Type</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>

                <div class="panel">
                    <p><b>Tags</b></p>
                    <table id="tagsTable" class="detailsTable" runat="server">
                        <thead>
                            <tr>
                                <th onclick="sortTable(0,this)">Tag Name</th>
                                <th onclick="sortTable(1,this)">Value</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div class="columnRight">
                <div class="dropdown-container">
                    <label for="timeRange">Select Time Range:</label>
                    <select id="timeRange" onchange="fetchDisk();">
                        <option value="1">1 Day</option>
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
    </form>
</body>
</html>
