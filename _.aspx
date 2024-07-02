<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="fiziksel._default" %>

<!DOCTYPE html>
<html lang="tr">

<head>
    <meta charset="UTF-8">
    <title>Fiziksel Envanter</title>

    <link rel="stylesheet" type="text/css" href="styles.css" />

    <style>
        /* Container Styles */
        .panel-container {
            display: flex;
            padding: 10px;
            margin: auto;
            width: 80%;
            justify-content: center;
            font-size: 10px;
        }

        .left-panel {
            display: flex;
            flex-wrap: wrap;
            justify-content: flex-end;
        }

        .right-panel {
            width: 500px;
            padding-top: 10px;
        }

        /* Panel Styles */
        .panel,
        .left-panel .extra-panel {
            border: 1px solid #ddd;
            padding: 2px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            width: 190px;
            height: 170px;
            margin: 10px 5px 0 0;
        }

        .buttons-panel {
            padding-top: 10px;
            width: 190px;
            height: 170px;
            margin-bottom: 10px;
        }

        .right-panel .extra-panel {
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 0 5px 0 0;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            width: 100%;
            height: 365px;
            margin-bottom: 10px;
        }

        .checkbox-panel {
            max-height: 120px;
            overflow-y: auto;
        }

        .extra-panel .checkbox-panel {
            max-height: 290px;
            margin-left: 10px;
        }

        /* Input Styles */
        .search-input {
            width: 90%;
            padding: 5px;
            margin: 0;
            margin-bottom: 2px;
            font-size: 10px;
            box-sizing: border-box;
        }

        .extra-panel .search-input {
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

        /* Table Styles */
        #score-table {
            border: none;
            border-collapse: collapse;
            margin-top: 5px;
            width: 100%;
        }

        #score-table td {
            font-weight: bold;
            padding: 8px;
            text-align: left;
            border: none;
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
            background-color: #f2f2f2;
        }

        th:not(:first-child) {
            width: auto;
            min-width: 170px;
        }

        /* Button Styles */
        .button-container {
            display: flex;
            justify-content: center;
        }

        .button-container .button {
            background-color: #ccc;
            color: black;
        }

        .button-container .active {
            background-color: var(--primary-color);
            color: white;
        }

        .panel-button {
            margin-top: 5px;
            margin-left: 15px;
            padding: 10px 0px;
            width: 150px;
            border-radius: 3px;
        }

        /* Icon Styles */
        .edit-icon {
            min-width: 16px;
            background-image: url('edit.png');
            background-repeat: no-repeat;
            background-size: 70%;
            background-position: center;
            cursor: pointer;
        }

        /* Pagination Styles */
        .pagination {
            display: flex;
            justify-content: center;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-link {
            display: inline-block;
            padding: 8px;
            margin: 0 4px;
            text-decoration: none;
            cursor: pointer;
            color: #333;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .page-link.active {
            background-color: var(--primary-color);
            color: white;
        }

        /* Heading Styles */
        h5 {
            margin: 0 0 10px;
            font-size: 10px;
            color: #333;
        }

        h2 {
            margin-bottom: 5px;
        }
    </style>

</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <div>
                <h1 class="baslik" id="baslik" runat="server">Fiziksel Envanter</h1>
            </div>
            <div id="logo"></div>

        </div>
        <div id="panel-container" class="panel-container">
            <div class="left-panel">
                <div class="panel">
                    <h5>Adı</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>Üretici Firma</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>Model</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>Kapsam-BBVA Metrics</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>Enviroment</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>Özel Durumu</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div class="panel">
                    <h5>İşletim Sistemi</h5>
                    <input type="text" placeholder="Search for Filtering" class="search-input">
                    <div class="checkbox-panel"></div>
                </div>
                <div id="score-panel" class="extra-panel">
                    <table id="score-table">
                        <tr>
                            <td>Total Cpu Socket</td>
                            <td id="soket-count">0</td>
                        </tr>

                        <tr>
                            <td>Total Core</td>
                            <td id="core-count">0</td>
                        </tr>
                        <tr>
                            <td>Total Memory</td>
                            <td id="memory-count">0</td>
                        </tr>
                    </table>
                </div>
                <div id="info-panel" class="extra-panel" style="text-align: center;">
                    <div>
                        <h2>Rows per Page</h2>
                        <div class="button-container">
                            <div class="button active" onclick="selectRow(this)">10</div>
                            <div class="button" onclick="selectRow(this)">20</div>
                            <div class="button" onclick="selectRow(this)">50</div>
                        </div>
                    </div>
                    <h2>Row Count</h2>
                    <h2 id="row-count">0</h2>

                </div>
                <div class="buttons-panel">
                    <button id="reset-button" class="panel-button" type="button">Reset Filters</button>
                    <button id="add-button" class="panel-button">Add a new Machine</button>
                    <button id="chart-button" class="panel-button" type="button">Show as Pie Chart</button>


                </div>
            </div>
            <div class="right-panel">
                <div class="extra-panel">
                    <h5 style="display:none;">Select Extra Column</h5>
                    <select id="column-dropdown" class="dropdown">
                        <option>Select Extra Column</option>
                    </select>
                    <input style="display:none;" type="text" placeholder="Search for Filtering" class="search-input">

                    <div class="checkbox-panel"></div>
                </div>

            </div>




        </div>
        <div class="table-container">
            <table id="contentTable" class="content-table" runat="server">
                <tr>
                    <th></th>
                    <th>Adı</th>
                    <th>Seri NO</th>
                    <th>Açıklama</th>
                    <th>Üretici Firma</th>
                    <th>Model</th>
                    <th>CPU Soket</th>
                    <th>CPU Core</th>
                    <th>Toplam Core</th>
                    <th>Memory</th>
                    <th>Cihaz Tipi</th>
                    <th>Hall</th>
                    <th>Row</th>
                    <th>Rack</th>
                    <th>Blade Şasi</th>
                    <th>Domain Bilgisi</th>
                    <th>Enviroment</th>
                    <th>Firma</th>
                    <th>Sahiplik</th>
                    <th>Lokasyon</th>
                    <th>Kapsam-BBVA Metrics</th>
                    <th>Cluster</th>
                    <th>İşletim Sistemi</th>
                    <th>Sorumlu Grup</th>
                    <th>Satın Alma Tarihi</th>
                    <th>Planlanan Devreden Çıkarma Tarihi</th>
                    <th>Bakım Başlangıç Tarihi</th>
                    <th>Bakım Bitiş Tarihi</th>
                    <th>Özel Durumu</th>
                    <th>Support</th>
                </tr>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>

        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>
        <script>
            const maxPage = 10;
            let rowsPerPage = 10;
            let currentPage = 1;
            let tableRows = [];
            const paginationElement = document.getElementById('pagination');

            function displayRows() {
                tableRows = Array.from(contentTable.querySelectorAll('tr.visible'));
                let length = currentPage * rowsPerPage;
                if (length > tableRows.length) length = tableRows.length;

                for (let i = 1; i < document.getElementById("contentTable").rows.length; i++) {
                    document.getElementById("contentTable").rows[i].style.display = 'none';
                }
                for (let i = (currentPage - 1) * rowsPerPage; i < length; i++) {
                    tableRows[i].style.display = '';
                }
                setupPagination(1, Math.ceil(tableRows.length / rowsPerPage));
            }

            function setupPagination(start, end) {
                paginationElement.innerHTML = '';
                if ((end - start) >= (maxPage - 1)) end = maxPage + start - 1;

                for (let i = start; i <= end; i++) {
                    const pageLink = document.createElement('a');
                    pageLink.href = '#';
                    pageLink.innerText = i;

                    if (i === currentPage) {
                        pageLink.classList.add('page-link', 'active');
                    } else {
                        pageLink.classList.add('page-link');
                    }

                    pageLink.addEventListener('click', function () {
                        currentPage = i;
                        displayRows();
                        updatePagination();
                    });

                    paginationElement.appendChild(pageLink);
                }
            }

            function updatePagination() {
                const pageLinks = document.querySelectorAll('.page-link');
                pageLinks.forEach(link => link.classList.remove('active'));

                if (Math.ceil(tableRows.length / rowsPerPage) > maxPage) {
                    if (currentPage > (maxPage / 2)) {
                        setupPagination(currentPage - (maxPage / 2), Math.ceil(tableRows.length / rowsPerPage));
                        pageLinks[maxPage / 2].classList.add('active');
                    } else {
                        setupPagination(1, Math.ceil(tableRows.length / rowsPerPage));
                        pageLinks[currentPage - 1].classList.add('active');
                    }
                } else {
                    pageLinks[currentPage - 1].classList.add('active');
                }
            }

            document.addEventListener("DOMContentLoaded", function () {
                const panelContainer = document.getElementById("panel-container");
                const contentTable = document.getElementById("contentTable");
                const resetButton = document.getElementById("reset-button");
                const rowCountElement = document.getElementById("row-count");
                const tableRows = Array.from(contentTable.querySelectorAll('tr')).slice(1);
                let labels = [];
                let lastChangedColumn = null;

                function getColumnIndex(columnName) {
                    return Array.from(contentTable.rows[0].cells).findIndex(cell => cell.innerText.trim() === columnName);
                }

                function updateRowCount() {
                    const visibleRows = tableRows.filter(row => row.classList.contains('visible'));
                    rowCountElement.textContent = visibleRows.length;
                }

                function calculateTotalValues(columnIndex) {
                    const visibleRows = tableRows.filter(row => row.classList.contains('visible'));
                    let totalValue = 0;

                    visibleRows.forEach(row => {
                        const cellValue = parseInt(row.cells[columnIndex].innerText.trim()) || 0;
                        totalValue += cellValue;
                    });

                    return totalValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                }

                function createCheckbox(panel, labelFor, labelText) {
                    const cont = document.createElement("div");
                    cont.className = "checkbox-content";

                    const checkbox = document.createElement("input");
                    checkbox.type = "checkbox";
                    checkbox.id = labelFor.replace(/[^a-zA-Z0-9]/g, '_').toLowerCase();

                    const label = document.createElement("label");
                    label.setAttribute("for", checkbox.id);
                    label.innerText = labelText;

                    if (panel.querySelector('[for="' + checkbox.id + '"]')) return;

                    cont.appendChild(checkbox);
                    cont.appendChild(label);
                    panel.querySelector('.checkbox-panel').appendChild(cont);
                }

                function populatePanelCheckboxes(panel, columnIndex) {
                    const visibleRows = tableRows.filter(row => row.classList.contains('visible'));

                    panel.querySelectorAll('input[type="checkbox"]').forEach(function (checkbox) {
                        if (!checkbox.checked) {
                            checkbox.closest('.checkbox-content').remove();
                        }
                    });

                    const uniqueValues = new Set(visibleRows.map(row => row.cells[columnIndex].innerText.trim()));
                    uniqueValues.forEach(function (cellContent, index) {
                        const checkboxId = "checkbox" + index + "-" + columnIndex;
                        createCheckbox(panel, checkboxId, cellContent);
                    });
                }

                function resetCheckboxes() {
                    lastChangedColumn = null;
                    labels = [];
                    panelContainer.querySelectorAll('.checkbox-panel input[type="checkbox"]').forEach(checkbox => checkbox.checked = false);
                    filterTable();
                    updateRowCount();
                }

                function filterTable(isBack) {
                    currentPage = 1;
                    Array.from(document.querySelectorAll('h5')).forEach(h5 => h5.classList.remove('filtered'));

                    if (labels.length === 0) {
                        tableRows.forEach(row => row.classList.add('visible'));
                        lastChangedColumn = null;
                    } else {
                        if (isBack) {
                            tableRows.forEach(row => row.classList.add('visible'));
                        }

                        labels.forEach(function (label) {
                            const columnName = label.closest(".panel") ? label.closest(".panel").querySelector("h5") : label.closest(".extra-panel").querySelector("h5");
                            if (columnName.classList.contains('filtered')) {
                                tableRows.forEach(row => {
                                    if (!row.classList.contains('visible')) return;

                                    const cellValue = row.cells[getColumnIndex(columnName.innerText)].innerText;
                                    if (cellValue == label.innerText) row.classList.add('visible');
                                    else row.classList.remove('visible');
                                });
                            } else {
                                columnName.classList.add('filtered');

                                tableRows.forEach(row => {
                                    if (!row.classList.contains('visible')) return;

                                    const cellValue = row.cells[getColumnIndex(columnName.innerText)].innerText;
                                    if (cellValue == label.innerText) row.classList.add('visible');
                                    else row.classList.remove('visible');
                                });
                            }
                        });
                    }

                    panelContainer.querySelectorAll('.panel').forEach(panel => {
                        const columnName = panel.querySelector('h5').innerText.trim();
                        const columnIndex = getColumnIndex(columnName);
                        if (columnName !== lastChangedColumn) {
                            populatePanelCheckboxes(panel, columnIndex);
                        }
                    });

                    if (lastChangedColumn != document.getElementById("column-dropdown").closest(".extra-panel").querySelector("h5").innerText) {
                        updateExtraPanel();
                    }
                    updateRowCount();
                    document.getElementById("soket-count").innerText = calculateTotalValues(6);
                    document.getElementById("core-count").innerText = calculateTotalValues(8);
                    document.getElementById("memory-count").innerText = calculateTotalValues(9);
                    displayRows();
                }

                function handleCheckboxChange(event) {
                    if (event.target.type === "checkbox") {
                        const label = document.querySelector('label[for="' + event.target.id + '"]');

                        if (event.target.checked) {
                            lastChangedColumn = label.closest(".panel") ? label.closest(".panel").querySelector("h5").innerText : label.closest(".extra-panel").querySelector("h5").innerText;
                            labels.push(label);
                            filterTable();
                        } else {
                            labels = labels.filter(item => item !== label);
                            lastChangedColumn = null;
                            filterTable(true);
                        }
                    }
                }

                function handleSearchInput(event) {
                    const panel = event.currentTarget.parentNode;
                    const columnName = panel.querySelector('h5').innerText.trim();
                    const columnIndex = getColumnIndex(columnName);
                    const searchText = event.target.value.trim().toLowerCase();
                    filterCheckboxes(panel, searchText);
                }

                function filterCheckboxes(panel, searchText) {
                    const checkboxes = panel.querySelector('.checkbox-panel').querySelectorAll('.checkbox-content');
                    const lowerCaseSearchText = searchText.toLowerCase();

                    checkboxes.forEach(function (checkbox) {
                        const label = checkbox.querySelector('label');
                        const labelText = label.innerText.toLowerCase();
                        checkbox.style.display = labelText.includes(lowerCaseSearchText) ? "block" : "none";
                    });
                }

                function handleDropdownChange() {
                    const dropdown = document.getElementById('column-dropdown');
                    const extraCbs = Array.from(dropdown.closest(".extra-panel").querySelectorAll('input[type="checkbox"]:checked'));

                    extraCbs.forEach(function (checkbox) {
                        const label = checkbox.closest('.checkbox-content').querySelector('label');
                        labels = labels.filter(item => item !== label);
                    });

                    dropdown.closest(".extra-panel").querySelector(".checkbox-panel").innerHTML = "";
                    dropdown.closest(".extra-panel").querySelector("h5").innerText = dropdown.value;
                    filterTable();
                }

                function updateExtraPanel() {
                    const dropdown = document.getElementById('column-dropdown');
                    if (dropdown.value != "Select Extra Column") {
                        dropdown.closest(".extra-panel").querySelector('.search-input').style.display = "";
                        const panel = dropdown.closest(".extra-panel");
                        const columnIndex = getColumnIndex(dropdown.value);
                        populatePanelCheckboxes(panel, columnIndex);
                    } else {
                        dropdown.closest(".extra-panel").querySelector('.search-input').style.display = "none";
                    }
                }

                function addOptionsDropdown() {
                    const dropdown = document.getElementById('column-dropdown');
                    dropdown.innerHTML = '';
                    const firstOption = document.createElement('option');
                    firstOption.value = "Select Extra Column";
                    firstOption.textContent = "Select Extra Column";
                    dropdown.appendChild(firstOption);

                    contentTable.querySelectorAll('th').forEach(column => {
                        if (["", "Adı", "Üretici Firma", "Kapsam-BBVA Metrics", "Enviroment", "Özel Durumu", "İşletim Sistemi", "Model"].includes(column.innerText)) return;

                        const option = document.createElement('option');
                        option.value = column.innerText;
                        option.textContent = column.innerText;
                        dropdown.appendChild(option);
                    });
                }

                function editRow(rowName) {
                    window.location.href = "edit.aspx?name=" + rowName;
                }

                function setEditIcons() {
                    tableRows.forEach(row => {
                        const icon = row.querySelectorAll('td')[0];
                        icon.classList.add('edit-icon');
                        icon.addEventListener('click', function () {
                            editRow(row.querySelectorAll('td')[1].innerText);
                        });
                    });
                }

                document.getElementById('add-button').addEventListener('click', function (event) {
                    event.preventDefault();
                    window.location.href = 'create.aspx';
                });

                document.getElementById('column-dropdown').addEventListener('change', handleDropdownChange);

                panelContainer.querySelectorAll('.panel').forEach(panel => {
                    const searchInput = panel.querySelector('.search-input');
                    searchInput.addEventListener('input', handleSearchInput);
                });

                document.querySelector('.right-panel').querySelector('.extra-panel').querySelector('.search-input').addEventListener('input', handleSearchInput);
                panelContainer.addEventListener("change", handleCheckboxChange);
                resetButton.addEventListener("click", resetCheckboxes);

                filterTable();
                setEditIcons();
                addOptionsDropdown();
            });

            function selectRow(selectedButton) {
                const buttons = document.querySelector(".button-container").querySelectorAll('.button');
                buttons.forEach(button => button.classList.remove('active'));
                selectedButton.classList.add('active');
                rowsPerPage = selectedButton.textContent;
                currentPage = 1;
                displayRows();
            }
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

                const columns = Array.from(document.getElementById("contentTable").querySelectorAll('th')).slice(1);
                columns.forEach(col => uniqueValues.push({ column: col.innerText, values: [] }));

                const visibleRows = Array.from(document.getElementById("contentTable").querySelectorAll('tr.visible')).slice(1);
                visibleRows.forEach(row => {
                    for (let i = 4; i < uniqueValues.length; i++) {
                        const cellValue = row.cells[i].innerText;
                        const index = findValue(i, cellValue);
                        if (index === -1) {
                            uniqueValues[i].values.push({ value: cellValue, count: 1 });
                        } else {
                            uniqueValues[i].values[index].count++;
                        }
                    }
                });

                const columnsToRemove = [24, 23, 22, 21, 19, 15, 3, 2, 1, 0];
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
