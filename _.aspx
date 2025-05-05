    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="hwpedia._default" %>

<!DOCTYPE html>
<html lang="tr">

<head>
    <meta charset="UTF-8">
    <title>HWPEDIA</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />


    <style>
        #page-name {
            color: #0071EB;
        }
        .panel-container {
            display: flex;
            height: 450px;
            margin: 70px auto;
            width: 100%;
            font-size: 10px;
            margin-bottom:20px;
        }

        .left-panel {
            height: 100%;
            display: grid;
            width: 70%;
            grid-template-columns: repeat(5, 1fr);
            grid-template-rows: repeat(2, auto);
            margin-left: auto;
        }

        .panel {
            border: 1px solid #ddd;
            padding: 1px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            height: 210px;
            margin: 5px;
            overflow-y:auto;
            background-color:white;
        }

        .large-panel {
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 0 5px 0 0;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            height: 98%;
            width: 15%;
            overflow-y:auto;
            margin: 5px 10px 0px 10px;
            margin-right: auto;
            background-color:white;

        }

        .checkbox-panel {
            max-height: 76%;
            overflow-y: auto;
        }

        .large-panel .checkbox-panel {
            max-height: 82%;
            margin-left: 10px;
        }

        .search-input {
            width: 90%;
            padding: 5px;
            margin: 0;
            margin-bottom: 2px;
            font-size: 10px;
            box-sizing: border-box;
        }

        .large-panel .search-input {
            margin-top: 5px;
            margin-bottom: 5px;
            margin-left: 10px;
        }

        .dropdown {
            width: 90%;
            padding: 5px;
            margin-top: 10px;
            margin-left: 10px;
            box-sizing: border-box;
        }

        .checkbox-content {
            margin: 0;
        }

        .score-table {
            border: none;
            border-collapse: collapse;
            margin-top: 5px;
            font-size: 8px;
            width: 100%;
            height: 160px;
        }

        .score-table td {
            font-weight: bold;
            padding: 3px 8px;
            border: none;
        }

        .buttons-panel {
            background-color: transparent;
            border: none;
            box-shadow: none;
            display: flex;
            justify-content: center;
            margin: auto;
            margin-right: 10px;
        }
        .panel-button {
            margin: 5px;
            padding: 10px 0px;
            width: 8%;
            border-radius: 3px;
        }
        .controls {
            display: flex;
            justify-content: space-between;
            margin: 10px 0;
        }

        #row-count{
            margin-left:7px;
            margin-right: auto;
            font-size: 12px;
        }

        .nav-btn {
            background-color: #fff;
            border: 1px solid #ccc;
            border-radius: 50%;
            padding: 10px;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .nav-btn:hover {
            background-color: #f5f5f5;
        }

        .active {
            background-color: #717171;
        }
        .table-top{
            width: 90%;
            display: flex;
            justify-content: space-between;
            margin: 8px auto 4px auto;
            align-items: center;
        }
        .table-container {
            width: 90%;
            overflow-y: auto;
            align-items: center;
            justify-content: center;
            margin: auto;
            font-size: 10px;
        }

        .content-table {
            border-collapse: collapse;
            font-size: 10px;
        }

        th,
        td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }

        th {
            cursor: pointer;
            background-color: #f2f2f2;
        }

        th:not(:first-child) {
            width: auto;
            min-width: 170px;
        }

        .button-container {
            display: flex;
            justify-content: center;
            font-size: small;
            margin-right:7px;
        }

        .button-container .button {
            background-color: #ccc;
            color: black;
            padding:10px 18px;
        }

        .button-container .active {
            background-color: var(--primary-color);
            color: white;
        }
                

        .pagination {
            display: flex;
            justify-content: center;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-link {
            background-color: white;
            display: inline-block;
            padding: 8px;
            margin: 0 4px;
            text-decoration: none;
            cursor: pointer;
            color: #333;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .page-link:hover {
            color: white;
        }

        .page-link.active {
            background-color: var(--primary-color);
            color: white;
        }

        .gray-text{
            font-size: 8px;
            font-weight: bolder;
            margin: 5px 0px 1px 12px;
        }

        #server-search{
            padding: 12px;
            border-radius: 4px;
            font-size: 12px;
            width: 250px;
            border:1px solid #777;
            box-shadow:0 4px 10px rgba(0,0,0,0.1);
        }


        h5 {
            margin: 5px;
            font-size: 10px;
        }

    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <a href="/">cloudunited</a>
            <span id="page-name">HWPEDIA</span>

        </div>
        <div id="panel-container" class="panel-container">
            <div class="left-panel">
                <div class="panel filter-panel">
                    <h5>Vendor</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>HWType</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Model</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel" >
                    <h3>HW Vendor Scores</h3>
                    <table id="vendor-table" class="score-table"></table>
                </div>
                
                <div class="panel" >
                    <h3>HW Type Scores</h3>
                    <table id="hwtype-table" class="score-table"></table>
                </div>
                <div class="panel filter-panel">
                    <h5>Kapsam-BBVA Metrics</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Enviroment</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>OS</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel" >
                    <h5>Notes</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h3>HW Resources Scores</h3>

                    <table class="score-table">
                        <tr>
                            <td>Total Cpu Socket</td>
                            <td id="soket-count">0</td>
                        </tr>

                        <tr>
                            <td>Total Core</td>
                            <td id="core-count">0</td>
                        </tr>
                        <tr>
                            <td>Total CPU</td>
                            <td id="cpu-count">0</td>
                        </tr>
                        <tr>
                            <td>Total Memory</td>
                            <td id="memory-count">0</td>
                        </tr>

                    </table>
                </div>
            </div>

            <div class="large-panel filter-panel dropdown-panel">
                <h5 style="display:none;">Select Extra Column</h5>
                <select id="column-dropdown" class="dropdown">
                    <option>Select Extra Column</option>
                </select>
                <input style="display:none;" type="text" placeholder="Search for Filtering" class="search-input">

                <div class="checkbox-panel"></div>
            </div>

        </div>
        <asp:HiddenField ID="hiddenField" runat="server" />
        <asp:Button ID="hiddenButton" runat="server" OnClick="hiddenButton_Click" Style="display:none;" />
        <div class="table-top">
            <input type="text" placeholder="Search for Server Name" id="server-search">
            <h2 id="row-count"></h2>
            <div class="button-container">
                <div class="button active" onclick="setRowsPerPage(this)">10</div>
                <div class="button" onclick="setRowsPerPage(this)">20</div>
                <div class="button" onclick="setRowsPerPage(this)">50</div>
            </div>
            <button id="reset-button" class="panel-button" type="button">Reset Filters</button>
            <button id="export-button" class="panel-button" type="button">Export Data</button>
            <button id="chart-button" class="panel-button" type="button">Show as Pie Chart</button>
        </div>

        <div class="table-container">
            <table id="contentTable" class="content-table" >
                <thead>
                    <tr>
                        <th>Server</th>
                        <th>IPv4</th>
                        <th>Description</th>
                        <th>Vendor</th>
                        <th>Model</th>
                        <th>Serial No</th>
                        <th>CPU Soket</th>
                        <th>CPU Core</th>
                        <th>Toplam Core</th>
                        <th>Memory</th>
                        <th>OS</th>
                        <th>Firmware</th>
                        <th>HWType</th>
                        <th>DC Location</th>
                        <th>Domain</th>
                        <th>Enviroment</th>
                        <th>Location</th>
                        <th>Company</th>
                        <th>Kapsam-BBVA Metrics</th>
                        <th>Responsible Group</th>
                        <th>Notes</th>
                        <th>Maint Start Date</th>
                        <th>Maint Finish Date</th>
                        <th>Inventory Date</th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server">
                </tbody>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>

        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>
        <script src="table_organizer.js?v=1.1.0"></script>
        <script src="panel_organizer.js?v=1.2.0"></script>
        <script>
            function showCharts() {
                let uniqueValues = [
                    {
                        column: "",
                        values: [
                            { value: "", count: 1 }
                        ]
                    }
                ];

                function findValue(currentColumnIndex, value) {
                    return uniqueValues[currentColumnIndex].values.findIndex(currentValue => currentValue.value.trim() === value.trim());
                }

                const columns = Array.from(document.getElementById("contentTable").querySelectorAll('th')).slice(3);
                columns.forEach(col => uniqueValues.push({ column: col.innerText, values: [] }));
                filteredData.forEach(row => {
                    for (let i = 0; i < uniqueValues.length; i++) {
                        const cellValue = row[Object.keys(row)[i+2]];
                        const index = findValue(i, cellValue);
                        if (index === -1) {
                            uniqueValues[i].values.push({ value: cellValue, count: 1 });
                        } else {
                            uniqueValues[i].values[index].count++;
                        }
                    }
                });

                const columnsToRemove = [0,11];
                columnsToRemove.forEach(colIndex => uniqueValues.splice(colIndex, 1));

                uniqueValues.forEach(columnData => {
                    columnData.values.sort((a, b) => b.count - a.count);
                });

                localStorage.setItem("chartDatas", JSON.stringify(uniqueValues));
                const storedValues = JSON.parse(localStorage.getItem('chartDatas'));
                window.location.href = "/piecharts";
            }

            document.getElementById("chart-button").addEventListener("click", showCharts);
        </script>
    </form>

</body>

</html>
