<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="hwpedia._default" %>


<!DOCTYPE html>
<html lang="tr">

<head>
    <meta charset="UTF-8">
    <title>HWPEDIA</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />

    <style>
        .panel-container {
            display: flex;
            height: 330px;
            margin: auto;
            width: 100%;
            font-size: 8px;
            margin-bottom:20px;
        }

        .left-panel {
            height: 100%;
            display: grid;
            width: 74%;
            grid-template-columns: repeat(7, 1fr);
            grid-template-rows: repeat(2, auto);
        }

        .panel {
            border: 1px solid #ddd;
            padding: 1px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            height: 140px;
            margin: 5px;
            overflow-y:auto;
        }

        .large-panel {
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 0 5px 0 0;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            height: 100%;
            width: 12%;
            overflow-y:auto;
            margin: 0px 10px 0px 10px;
        }

        .checkbox-panel {
            max-height: 65%;
            overflow-y: auto;
        }

        .large-panel .checkbox-panel {
            max-height: 75%;
            margin-left: 10px;
        }

        .search-input {
            width: 90%;
            padding: 4px;
            margin: 0;
            margin-bottom: 2px;
            font-size: 8px;
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
        }

        .score-table td {
            font-weight: bold;
            border: none;
        }

        .buttons-panel {
            background-color: transparent;
            border: none;
            box-shadow: none;
            align-items: center;
            display: flex;
            flex-direction: column;
        }
        .panel-button {
            margin-top: 5px;
            padding: 10px 0px;
            width: 95%;
            border-radius: 3px;
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
            width:95%;
            margin:auto;

        }

        .button-container .button {
            background-color: #ccc;
            color: black;
            padding:15px;
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

        h5 {
            margin: 5px;
            font-size: 10px;
        }

    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <div id="logo"></div>
            <div>
                <h1 class="baslik" id="baslik" runat="server">HWPEDIA</h1>
            </div>

        </div>
        <div id="panel-container" class="panel-container">
            <div class="left-panel">
                <div class="panel filter-panel">
                    <h5>Server</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Vendor</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Model</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
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
                <div class="panel">
                    <table class="score-table" id="os-table">

                    </table>                    
                </div>
                <div class="panel">
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
                <div class="panel filter-panel">
                    <h5>OS</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>HWType</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Company</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>DC Location</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel filter-panel">
                    <h5>Responsible Group</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel" style="text-align: center;">
                    <h2>Rows per Page</h2>
                    <div class="button-container">
                        <div class="button active" onclick="setRowsPerPage(this)">10</div>
                        <div class="button" onclick="setRowsPerPage(this)">20</div>
                        <div class="button" onclick="setRowsPerPage(this)">50</div>
                    </div>
                    <h2>Row Count</h2>
                    <h2 id="row-count">0</h2>
                </div>
                <div class="panel buttons-panel">
                    <button id="reset-button" class="panel-button" type="button">Reset Filters</button>
                    <button id="export-button" class="panel-button" type="button">Export Data</button>
                    <button id="chart-button" class="panel-button" type="button">Show as Pie Chart</button>
                </div>
            </div>
            <div class="large-panel filter-panel">
                <h5>Inventory Date</h5>
                <input type="text" placeholder="Search for Filtering" class="search-input">
                <div class="checkbox-panel"></div>
            </div>

            <div class="large-panel filter-panel">
                <h5 style="display:none;">Select Extra Column</h5>
                <select id="column-dropdown" class="dropdown">
                    <option>Select Extra Column</option>
                </select>
                <input style="display:none;" type="text" placeholder="Search for Filtering" class="search-input">

                <div class="checkbox-panel"></div>
            </div>

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

        <script>
            const MAX_PAGE = 10;
            let ROWS_PER_PAGE = 10;
            let CURRENT_PAGE = 1;
            let data = [];
            let ascending = true;
            let filteredData = [];

            function sortDatas(index) {
                filteredData.sort((a, b) => {
                    let aValue = a[Object.keys(a)[index]];
                    let bValue = b[Object.keys(b)[index]];

                    if (!isNaN(aValue) && !isNaN(bValue)) {
                        return ascending ? aValue - bValue : bValue - aValue;
                    }
                    
                    if (index == 20 ||index == 21||index == 22) {
                        return ascending ? new Date(aValue) - new Date(bValue) : new Date(bValue) - new Date(aValue);
                    }

                    return ascending ? aValue.trim().localeCompare(bValue.trim()) : bValue.trim().localeCompare(aValue.trim());
                });
                CURRENT_PAGE = 1;
                renderPage(CURRENT_PAGE);
                renderPagination();
            }

            function renderPage(pageNumber) {
                const start = (pageNumber - 1) * ROWS_PER_PAGE;
                const end = start + ROWS_PER_PAGE;
                const paginatedData = filteredData.slice(start, end);

                const tableBody = document.getElementById('tableBody');
                tableBody.innerHTML = '';

                paginatedData.forEach(row => {
                    const tr = document.createElement('tr');
                    for (const cellData of Object.values(row)) {
                        const td = document.createElement('td');
                        td.textContent = cellData;
                        tr.appendChild(td);
                    }
                    tableBody.appendChild(tr);
                });
                setCounters();
            }

            function renderPagination() {
                const totalPages = Math.ceil(filteredData.length / ROWS_PER_PAGE);
                const paginationDiv = document.getElementById('pagination');
                paginationDiv.innerHTML = '';

                let startPage = Math.max(1, CURRENT_PAGE - Math.floor(MAX_PAGE / 2));
                let endPage = Math.min(totalPages, CURRENT_PAGE + Math.floor(MAX_PAGE / 2));

                if (endPage - startPage < MAX_PAGE) {
                    if (startPage === 1) {
                        endPage = Math.min(totalPages, startPage + MAX_PAGE - 1);
                    } else if (endPage === totalPages) {
                        startPage = Math.max(1, endPage - MAX_PAGE + 1);
                    }
                }

                for (let i = startPage; i <= endPage; i++) {
                    const button = document.createElement('button');
                    button.textContent = i;
                    button.className = 'page-link';
                    if (i === CURRENT_PAGE) {
                        button.classList.add('active');
                    }
                    button.addEventListener('click', () => {
                        CURRENT_PAGE = i;
                        renderPage(CURRENT_PAGE);
                        renderPagination();
                    });
                    paginationDiv.appendChild(button);
                }
            }

            function initializeTable() {
                filteredData = data;
                sortDatas(0);
                document.querySelectorAll(".filter-panel").forEach(column => {
                    generateColumnCheckboxes(column);
                });
                addOptionsDropdown();
            }

            function getColumnIndex(columnName) {
                const contentTable = document.getElementById("contentTable");
                return Array.from(contentTable.rows[0].cells).findIndex(cell => cell.innerText.trim() === columnName);
            }

            function setRowsPerPage(selectedButton) {
                const buttons = document.querySelector(".button-container").querySelectorAll('.button');
                buttons.forEach(button => button.classList.remove('active'));
                selectedButton.classList.add('active');
                ROWS_PER_PAGE = parseInt(selectedButton.textContent);
                CURRENT_PAGE = 1;
                renderPagination();
                renderPage(CURRENT_PAGE);
            }

            function generateColumnCheckboxes(dropdownContent) {
                const columnIndex = getColumnIndex(dropdownContent.querySelector("h5").textContent);
                const checkboxesDiv = dropdownContent.querySelector(".checkbox-panel");
                if (columnIndex === -1) return;

                checkboxesDiv.querySelectorAll("div").forEach(div => {
                    const checkbox = div.querySelector("input[type='checkbox']");
                    if (!checkbox.checked) {
                        div.remove();
                    }
                });

                const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]].toString().trim()))];
                values.sort((a, b) => {
                    if (!isNaN(a) && !isNaN(b)) {
                        return ascending ? a - b : b - a;
                    }
                    if (a == null) return -1;
                    if (b == null) return 1;
                    return ascending ? a.localeCompare(b) : b.localeCompare(a);
                });

                const fragment = document.createDocumentFragment();
                values.forEach(value => {
                    if (checkboxesDiv.querySelector(`input[value='${value}']`)) return;

                    const div = document.createElement("div");
                    const checkbox = document.createElement("input");
                    checkbox.type = "checkbox";
                    checkbox.value = value;
                    checkbox.addEventListener("change", () => {
                        filterTable(checkbox);
                    });

                    const label = document.createElement("label");
                    label.textContent = value;

                    div.appendChild(checkbox);
                    div.appendChild(label);
                    div.appendChild(document.createElement("br"));
                    fragment.appendChild(div);
                });
                checkboxesDiv.appendChild(fragment);
            }

            function filterTable(checkbox) {
                filteredData = data;
                const columns = document.querySelectorAll(".filter-panel");
                columns.forEach(column => {
                    const checkboxes = column.querySelectorAll("input[type='checkbox']:checked");
                    if (checkboxes.length === 0) return;
                    const columnIndex = getColumnIndex(column.querySelector("h5").textContent);
                    const checkboxesValues = Array.from(checkboxes).map(checkbox => checkbox.value);
                    filteredData = filteredData.filter(row => checkboxesValues.includes(row[Object.keys(row)[columnIndex]].toString().trim()));
                });

                columns.forEach(column => {
                    if (!checkbox || column !== checkbox.closest(".filter-panel")) {
                        generateColumnCheckboxes(column);
                    }
                });

                CURRENT_PAGE = 1;
                renderPage(CURRENT_PAGE);
                renderPagination();
            }

            function searchCheckboxes(searchInput) {
                const filter = searchInput.value.toUpperCase();
                const checkboxesDiv = searchInput.parentElement.querySelector(".checkbox-panel");
                const divs = checkboxesDiv.getElementsByTagName("div");
                for (let i = 0; i < divs.length; i++) {
                    const label = divs[i].getElementsByTagName("label")[0];
                    const txtValue = label.textContent || label.innerText;
                    divs[i].style.display = txtValue.toUpperCase().indexOf(filter) > -1 ? "" : "none";
                }
            }

            function calculateTotalValues(columnIndex) {
                return filteredData.reduce((total, row) => total + (!isNaN(row[Object.keys(row)[columnIndex]]) ? Number(row[Object.keys(row)[columnIndex]]):0), 0);
            }

            function setCounters() {
                const osCount = {};
                filteredData.forEach(row => {
                    const osCell=row[Object.keys(row)[10]];
                    const osValue = osCell.trim();
        
                    if (osCount[osValue]) {
                        osCount[osValue]++;
                    } else {
                        osCount[osValue] = 1;
                    }
                });
        
                const table = document.getElementById("os-table");
                for (const os in osCount) {
                    if (osCount.hasOwnProperty(os)) {
                        const row = table.insertRow();
                        const osCell = row.insertCell(0);
                        const countCell = row.insertCell(1);
                        osCell.textContent = os;
                        countCell.textContent = osCount[os];
                    }
                }
                document.getElementById('row-count').textContent = filteredData.length;
                document.getElementById('soket-count').textContent = calculateTotalValues(6);
                document.getElementById('core-count').textContent = calculateTotalValues(7);
                document.getElementById('cpu-count').textContent = calculateTotalValues(8);
                document.getElementById('memory-count').textContent = calculateTotalValues(9);

            }

            function handleDropdownChange() {
                const selectedColumn = document.getElementById('column-dropdown').value;
                document.querySelector('.large-panel h5')[1].textContent = selectedColumn;
                const input = document.querySelector('.large-panel input[type="text"]')[1];
                const checkboxesDiv = document.querySelector('.large-panel .checkbox-panel')[1];
                checkboxesDiv.innerHTML = '';
                input.style.display = selectedColumn === "Select Extra Column" ? "none" : "block";
                filterTable();
            }

            function addOptionsDropdown() {
                const dropdown = document.getElementById('column-dropdown');
                dropdown.innerHTML = '';
                const firstOption = document.createElement('option');
                firstOption.value = "Select Extra Column";
                firstOption.textContent = "Select Extra Column";
                dropdown.appendChild(firstOption);

                const contentTable = document.getElementById('contentTable');
                contentTable.querySelectorAll('th').forEach(column => {
                    const columnName = column.innerText.trim();
                    const excludedColumns = ["Server", "Vendor","Model", "Kapsam-BBVA Metrics", "Enviroment", "OS", "HWType","Company","DC Location","Responsible Group"];
                    if (!excludedColumns.includes(columnName)) {
                        const option = document.createElement('option');
                        option.value = columnName;
                        option.textContent = columnName;
                        dropdown.appendChild(option);
                    }
                });
            }

            document.addEventListener('DOMContentLoaded', () => {
                document.querySelectorAll("th").forEach((th, index) => {
                    th.addEventListener('click', () => {
                        ascending = !ascending;
                        sortDatas(index);
                    });
                });

                document.getElementById('reset-button').addEventListener('click', (event) => {
                    event.preventDefault();
                    document.querySelectorAll("input[type='checkbox']").forEach(checkbox => checkbox.checked = false);
                    document.querySelectorAll("input[type='text']").forEach(input => input.value = "");
                    ascending = true;
                    filterTable();
                    sortDatas(0);
                });



                document.getElementById('column-dropdown').addEventListener('change', handleDropdownChange);

                document.getElementById('logo').addEventListener('click', function () {
                    window.location.href='/';
                });

                document.querySelectorAll('.search-input').forEach(input => {
                    input.addEventListener('keyup', () => {
                        searchCheckboxes(input);
                    });
                });
            });
        </script>
        
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
                console.log(storedValues);
                window.location.href = "/piecharts";
            }

            document.getElementById("chart-button").addEventListener("click", showCharts);
        </script>
    </form>

</body>

</html>
