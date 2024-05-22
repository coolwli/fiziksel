<html>

<head runat="server">
    <title>All VMs</title>
    <style>
        body {
            font-family: Verdana;
            margin: 0;
            padding: 0;
            background-color: #fff;
        }

        .header {
            overflow: hidden;
            background-color: black;
        }

        .header a {
            color: white;
            text-decoration: none;
            float: left;
            text-transform: uppercase;
            padding: 10px 15px;
            line-height: 10px;
            position: relative;
            text-align: center;
            border-radius: 4px;
        }

        .header a:hover {
            background-color: #383d42;
        }

        .header a.active {
            background-color: #383d42;
            color: white;
        }

        .header a:not(:last-child)::after {
            content: '|';
            position: absolute;
            right: -10px;
            top: 50%;
            transform: translateY(-50%);
        }

        #logo {
            background-image: url('logo.png');
            background-repeat: no-repeat;
            background-size: cover;
            float: right;
            width: 55px;
            height: 70px;
            margin-right: 10px;
        }

        footer {
            text-align: center;
            padding: 10px;
        }

        .footer {
            display: block;
            font-size: 14px;
            color: #888;
        }

        table {
            border-collapse: collapse;
            width: 97%;
            margin: auto;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            font-size: 9px;
        }

        th,
        td {
            border: none;
            padding: 8px;
            text-align: left;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            font-weight: bold;
            cursor: pointer;
            width: auto
        }

        tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        tr:hover {
            background-color: #efefef;
            cursor: pointer;
        }

        .dropdown-content {
            display: none;
            position: absolute;
            background-color: #f9f9f9;
            min-width: 160px;
            box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
            padding: 12px 16px;
            z-index: 1;
            max-height: 200px;
            overflow-y: auto;
            text-align: left;
        }

        .dropdown:hover .dropdown-content {
            display: block;
        }

        .dropdown-arrow {
            margin-left: 5px;
            font-size: 8px;
        }

        .table-top {
            width: 97%;
            display: flex;
            justify-content: space-between;
            margin: 8px auto 4px auto
        }

        button {
            color: white;
            border: none;
            cursor: pointer;
            font-size: 12px;
            transition: background-color 0.3s ease;
            outline: none;
        }

        #reset-button {
            background-color: black;
            padding: 5px 8px;
            margin-left: 10px;
        }

        #reset-button:hover {
            background-color: #484848;
        }

        #rowCounter {
            margin-right: auto;
            font-size: 10px;
        }

        .pagination {
            display: flex;
            justify-content: center;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-link {
            display: inline-block;
            padding: 10px;
            margin: 0 4px;
            text-decoration: none;
            cursor: pointer;
            color: #333;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .page-link.active {
            background-color: black;
            color: white;
        }
    </style>

</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <div>
                <a class="active">
                    <h2>VM</h2>
                </a>
                <a href="hosts.aspx">
                    <h2>VM Host</h2>
                </a>
                <a href="clusters.aspx">
                    <h2>Cluster</h2>
                </a>
            </div>
            <div id="logo"></div>
        </div>
        <div class="table-top">
            <h2 id="rowCounter"></h2>
            <button id="reset-button">Reset</button>
        </div>
        <div class="table-container">
            <table id="contentTable">
                <thead>
                    <tr>
                        <th style="width:16%" class="dropdown">
                            Name<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="nameDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown">
                            vCenter<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="vcDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            CPU<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="cpuDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Memory(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="memoryDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Total Disk(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="totaldiskDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown">
                            Power State <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="powerstateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Cluster <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="clusterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Data Center <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="datacenterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown" style="width:20%">
                            Owner <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ownerDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown" style="width:16%">
                            Created Date <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="createddateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
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
            const ROWS_PER_PAGE = 20;
            const MAX_PAGE = 10;
            var screenName = "";
            let data = [];
            let currentPage = 1;
            let ascending = true;
            let filteredData = [];

            function sortData(index) {
                filteredData.sort((a, b) => {
                    let aValue = a[Object.keys(a)[index]];
                    let bValue = b[Object.keys(b)[index]];
                    if (!isNaN(aValue) && !isNaN(bValue)) {
                        return ascending ? aValue - bValue : bValue - aValue;
                    }
                    return ascending ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
                });
                currentPage = 1;
                renderPage(currentPage);
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
                        tr.addEventListener('click', function () {
                            window.location.href = screenName + ".aspx" + "?id=" + row[Object.keys(row)[0]];
                        });
                    }
                    tableBody.appendChild(tr);
                });
            }

            function renderPagination() {
                const totalPages = Math.ceil(filteredData.length / ROWS_PER_PAGE);
                const paginationDiv = document.getElementById('pagination');
                paginationDiv.innerHTML = '';

                let startPage = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
                let endPage = Math.min(totalPages, currentPage + Math.floor(MAX_PAGE / 2));

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
                    if (i === currentPage) {
                        button.classList.add('active');
                    }
                    button.addEventListener('click', () => {
                        currentPage = i;
                        renderPage(currentPage);
                        renderPagination();
                    });
                    paginationDiv.appendChild(button);
                }
            }

            function initializeTable() {
                filteredData = data;
                document.querySelectorAll(".select-all-div input[type='checkbox']").forEach(selectAllCheckbox => {
                    selectAllCheckbox.addEventListener("change", () => {
                        const checkboxes = Array.from(selectAllCheckbox.closest(".dropdown-content").querySelectorAll("input[type='checkbox']"));
                        checkboxes.forEach(checkbox => { checkbox.checked = selectAllCheckbox.checked; });
                        filterTable();
                    });
                });
                document.querySelectorAll(".dropdown-content").forEach((column) => {
                    generateColumnCheckboxes(column);
                });
                renderPagination();
                updateCounter();
                sortData(0);
            }

            function generateColumnCheckboxes(dropdownContent) {
                const columnIndex = Array.from(document.querySelectorAll(".dropdown-content")).indexOf(dropdownContent);
                const checkboxesDiv = dropdownContent.querySelector(".checkboxes");

                checkboxesDiv.querySelectorAll("div").forEach((div) => {
                    const checkbox = div.querySelector("input[type='checkbox']");
                    if (!checkbox.checked) {
                        div.remove();
                    }
                });

                const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]]))];

                values.sort(function (a, b) {
                    if (!isNaN(a) && !isNaN(b)) {
                        return ascending ? a - b : b - a;
                    }
                    if (a == null) return -1;
                    if (b == null) return 1;

                    return ascending ? a.localeCompare(b) : b.localeCompare(a);
                });

                const fragment = document.createDocumentFragment();
                values.forEach((value) => {
                    if (checkboxesDiv.querySelector(`input[value='${value}']`)) {
                        return;
                    }
                    const div = document.createElement("div");
                    const checkbox = document.createElement("input");
                    checkbox.type = "checkbox";
                    checkbox.value = value;
                    checkbox.addEventListener("change", function () {
                        selectAll = dropdownContent.querySelector(".select-all-div input[type='checkbox']");
                        if (!checkbox.checked && selectAll.checked) {
                            selectAll.checked = false;
                        }

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
                const lastSelectedColumn = checkbox ? checkbox.closest(".dropdown-content") : null;

                const columns = document.querySelectorAll(".dropdown-content");
                const checkedValues = Array.from(columns).map(column => {
                    return Array.from(column.querySelectorAll("input[type='checkbox']:checked")).map(checkbox => checkbox.value);
                });
                console.log(checkedValues);
                filteredData = data.filter(row => {
                    return checkedValues.every((values, columnIndex) => {
                        if (values.length === 0) return true;
                        return values.includes(row[Object.keys(row)[columnIndex]].toString());
                    });
                });
                columns.forEach((column) => {
                    if (column !== lastSelectedColumn) {
                        generateColumnCheckboxes(column);
                    }
                });

                currentPage = 1;
                renderPagination();
                renderPage(currentPage);
                updateCounter();
            }

            function searchCheckboxes(searchInput) {
                const filter = searchInput.value.toUpperCase();
                const checkboxesDiv = searchInput.parentElement.querySelector(".checkboxes");
                const divs = checkboxesDiv.getElementsByTagName("div");
                for (let i = 0; i < divs.length; i++) {
                    const label = divs[i].getElementsByTagName("label")[0];
                    const txtValue = label.textContent || label.innerText;
                    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                        divs[i].style.display = "";
                    } else {
                        divs[i].style.display = "none";
                    }
                }
            }

            function updateCounter() {
                document.getElementById('rowCounter').textContent = `${filteredData.length} Satır Listelendi..`;
            }

            document.querySelectorAll("th").forEach((th, index) => {
                th.addEventListener('click', function (event) {
                    if (!event.target.closest('.dropdown-content')) {
                        ascending = !ascending;
                        sortData(index);
                    }
                });
            });

            document.getElementById('reset-button').addEventListener('click', () => {
                event.preventDefault();
                document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => { checkbox.checked = false; });
                document.querySelectorAll("input[type='text']").forEach((input) => { input.value = ""; });
                ascending = true;
                filterTable();
                sortData(0);
            });
        </script>
        <script>
            data = generateSampleData();
            function generateSampleData() {
                let data = [];
                for (let i = 0; i < 10000; i++) {
                    data.push({
                        "Name": "VM" + i,
                        "vCenter": "vCenter" + i,
                        "CPU": Math.floor(Math.random() * 10),
                        "Memory(GB)": Math.floor(Math.random() * 10),
                        "Total Disk(GB)": Math.floor(Math.random() * 10),
                        "Power State": Math.random() > 0.5 ? "Powered On" : "Powered Off",
                        "Cluster": "Cluster" + i,
                        "Data Center": "Data Center" + i,
                        "Owner": "Owner" + i,
                        "Created Date": new Date(Math.floor(Math.random() * Date.now())).toLocaleDateString()
                    });
                }
                return data;
            }
            initializeTable();

        </script>
    </form>
</body>

</html>
