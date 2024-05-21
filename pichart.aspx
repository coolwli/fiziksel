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
            background-color: #f2f2f2;
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
            padding: 8px;
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
                <a href="vmhost.aspx">
                    <h2>VM Host</h2>
                </a>
                <a href="cluster.aspx">
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
                        <th class="dropdown">
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
            const data = generateSampleData();

            let currentPage = 1;
            let checkedCheckboxes = [];
            let ascending = true;
            let filteredData = data;

            function generateSampleData() {
                const data = [];
                for (let i = 1; i <= 1000; i++) {
                    data.push({
                        Name: `Name ${i}`,
                        vCenter: `vCenter ${i}`,
                        CPU: `CPU ${i}`,
                        Memory: Math.floor(Math.random() * 100),
                        TotalDisk: `Total Disk ${i}`,
                        PowerState: `Power State ${i}`,
                        Cluster: `Cluster ${i}`,
                        DataCenter: `Data Center ${i}`,
                        Owner: `Owner ${i}`,
                        CreatedDate: `Created Date ${i}`
                    });
                }
                return data;
            }

            function sortDatas(index) {
                filteredData.sort(function (a, b) {
                    if (!isNaN(a[Object.keys(a)[index]]) && !isNaN(b[Object.keys(b)[index]])) {
                        return ascending ? a[Object.keys(a)[index]] - b[Object.keys(b)[index]] : b[Object.keys(b)[index]] - a[Object.keys(a)[index]];
                    }
                    return ascending ? a[Object.keys(a)[index]].localeCompare(b[Object.keys(b)[index]]) : b[Object.keys(b)[index]].localeCompare(a[Object.keys(a)[index]]);
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
                            window.location.href = "vmhost.aspx" + "?id=" + row[Object.keys(row)[0]];
                        });
                    }
                    tableBody.appendChild(tr);
                });
            }

            function renderPagination() {
                const totalPages = Math.ceil(filteredData.length / ROWS_PER_PAGE);
                const paginationDiv = document.getElementById('pagination');
                paginationDiv.innerHTML = '';

                const maxLeft = (currentPage - Math.floor(MAX_PAGE / 2));
                const maxRight = (currentPage + Math.floor(MAX_PAGE / 2));

                let startPage = maxLeft;
                let endPage = maxRight;

                if (maxLeft < 1) {
                    startPage = 1;
                    endPage = MAX_PAGE;
                }

                if (maxRight > totalPages) {
                    startPage = totalPages - (MAX_PAGE - 1);
                    endPage = totalPages;

                    if (startPage < 1) {
                        startPage = 1;
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
                document.querySelectorAll(".select-all-div").forEach((div, index) => {
                    const selectAllCheckbox = div.querySelector("input[type='checkbox']");

                    selectAllCheckbox.addEventListener("change", function () {
                        const checkboxes = Array.from(div.nextElementSibling.querySelectorAll("input[type='checkbox']"));
                        if (selectAllCheckbox.checked) {
                            checkedCheckboxes = checkedCheckboxes.filter((cb) => !checkboxes.includes(cb));
                            checkedCheckboxes.push(...checkboxes);
                        }
                        else {
                            checkedCheckboxes = checkedCheckboxes.filter((cb) => !checkboxes.includes(cb));

                        }
                        checkboxes.forEach((checkbox) => { checkbox.checked = selectAllCheckbox.checked; });
                        loadCheckboxes();
                    });

                });
                document.querySelectorAll(".dropdown-content").forEach((column) => {
                    generateColumnCheckboxes(column);
                });
                renderPage(currentPage);
                renderPagination();



            }

            function generateColumnCheckboxes(dropdownContent) {
                const columnIndex = Array.from(document.querySelectorAll("th.dropdown")).indexOf(dropdownContent.parentElement);
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
                        if (checkbox.checked)
                            checkedCheckboxes.push(checkbox);
                        else
                            checkedCheckboxes = checkedCheckboxes.filter((cb) => cb !== checkbox);
                        loadCheckboxes();
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

            function loadCheckboxes() {
                document.querySelectorAll(".filtered-div").forEach(div => div.classList.remove('filtered-div'));
                filteredData = data;
                if (checkedCheckboxes.length !== 0) {
                    checkedCheckboxes.forEach((checkbox) => {
                        filterTable(checkbox);
                    });
                    lastColumn = checkedCheckboxes[checkedCheckboxes.length - 1].closest(".dropdown-content");
                    document.querySelectorAll(".dropdown-content").forEach((column) => {
                        if (column !== lastColumn)
                            generateColumnCheckboxes(column);
                    });
                }
                else {
                    document.querySelectorAll(".dropdown-content").forEach((column) => {
                        generateColumnCheckboxes(column);
                    });
                }
                updateCounter();
                currentPage = 1;
                renderPagination();
            }

            function filterTable(checkbox) {
                const parentDropdown = checkbox.closest(".dropdown-content");
                const columnNum = Array.from(document.querySelectorAll(".dropdown-content")).indexOf(parentDropdown);

                if (parentDropdown.classList.contains('filtered-div')) {
                    data.forEach(row => {
                        const cell = row[Object.keys(row)[columnNum]].toString();
                        if (cell === checkbox.value && !filteredData.includes(row)) {
                            filteredData.push(row);
                        }
                    });
                }
                else {
                    filteredData.forEach(row => {
                        const cell = row[Object.keys(row)[columnNum]].toString();
                        if (cell !== checkbox.value) {
                            filteredData = filteredData.filter((r) => r !== row);
                        }
                    });
                    parentDropdown.classList.add('filtered-div');
                }
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
                document.getElementById('rowCounter').textContent = `Showing ${filteredData.length} rows..`;
                renderPage(currentPage);
            }

            document.querySelectorAll("th").forEach((th, index) => {
                th.addEventListener('click', function (event) {
                    if (!event.target.closest('.dropdown-content')) {
                        ascending = !ascending;
                        sortDatas(index);
                    }
                });
            });

            document.getElementById('reset-button').addEventListener('click', () => {
                event.preventDefault();
                document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => { checkbox.checked = false; });
                document.querySelectorAll("input[type='text']").forEach((input) => { input.value = ""; });
                checkedCheckboxes = [];
                ascending = true;
                loadCheckboxes();
                updateCounter();
                renderPagination();
                sortDatas(0);
            });

            initializeTable();
        </script>

    </form>
</body>

</html>
