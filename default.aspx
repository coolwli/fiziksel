<html>

<head runat="server">
    <title>All VMs</title>
    <style>
        :root {
            --primary-color: black;
            --secondary-color: #383d42;
            --background-color: #fff;
            --text-color: #333;
            --hover-color: #484848;
        }

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

        #vcDropdown {
            text-transform: uppercase;
            padding: 10px 15px;
            margin: 10px 2px;

        }

        #nameInput {
            width: 75%;
            padding: 9px 15px;
            margin: 10px 2px;


        }

        table {
            border-collapse: separate;
            border-spacing: 0;
            width: 100%;
            margin: 5px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            font-size: 10px;
        }

        th,
        td {
            border: none;
            padding: 8px;
            text-align: center;
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
            background-color: #f5f5f5;
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

        button {
            padding: 10px 20px;
            background-color: black;
            color: white;
            border: none;
            cursor: pointer;
            font-size: 16px;
            border-radius: 4px;
            transition: background-color 0.3s ease;
            outline: none;
        }

        button:hover {
            background-color: #484848;
        }

        #rowCounter {
            font-size: 15px;
            font-weight: bold;
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

        <div style="text-align:center">
            <asp:DropDownList ID="vcDropdown" runat="server" AutoPostBack="true">
                <asp:ListItem>ptekvcs01</asp:ListItem>
                <asp:ListItem>ptekvcsd01</asp:ListItem>
                <asp:ListItem>apgaraavcs801</asp:ListItem>
                <asp:ListItem>apgartstvcs201</asp:ListItem>
            </asp:DropDownList>
            <input type="text" id="nameInput" placeholder="Search for VMs..">
        </div>
        <div class="table-container">
            <table id="contentTable">
                <thead>
                    <tr>
                        <th>
                            Name
                        </th>
                        <th class="dropdown">
                            CPU<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="cpuDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Memory(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="memoryDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Total Disk(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="totaldiskDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Power State <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="powerstateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Cluster <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="clusterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Data Center <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="datacenterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Owner <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ownerDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Created Date <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="createddateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div"></div>
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
        <button id="reset-button">Reset</button>
        <p id="rowCounter"></p>
        <script>
            const MAX_PAGE = 10;
            const ROWS_PER_PAGE = 20;
            const paginationElement = document.getElementById("pagination");

            let rowCounter = Array.from(document.querySelectorAll("tbody tr")).length;
            let currentPage = 1;
            let filteredRows;
            let checkedCheckboxes = [];

            function displayRows() {
                const length = Math.min(currentPage * ROWS_PER_PAGE, rowCounter);
                document.querySelectorAll("tbody tr").forEach(row => {
                    row.style.display = 'none';
                });
                for (let i = (currentPage - 1) * ROWS_PER_PAGE; i < length; i++) {
                    filteredRows[i].style.display = '';
                }
            }

            function setupPagination(start, end) {
                paginationElement.innerHTML = '';
                if (end - start >= MAX_PAGE - 1) {
                    end = MAX_PAGE + start - 1;
                }
                for (let i = start; i <= end; i++) {
                    const pageLink = document.createElement('a');
                    pageLink.innerText = i;
                    pageLink.classList.add('page-link');
                    if (i === currentPage) {
                        pageLink.classList.add('active');
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
                const totalPages = Math.ceil(rowCounter / ROWS_PER_PAGE);
                const start = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
                const end = Math.min(totalPages, start + MAX_PAGE - 1);
                setupPagination(start, end);
            }

            function add1000rows() {
                const tableBody = document.getElementById("tableBody");
                const fragment = document.createDocumentFragment();
                for (let i = 0; i < 8000; i++) {
                    const row = document.createElement("tr");
                    for (let j = 0; j < 9; j++) {
                        const cell = document.createElement("td");
                        cell.textContent = `Row ${i + 1} Cell ${j + 1}`;
                        row.appendChild(cell);
                    }
                    row.classList.add('in-filter');
                    fragment.appendChild(row);
                }
                tableBody.appendChild(fragment);
            }
            function createCheckboxes() {
                const columns = Array.from(
                    document.querySelectorAll("th.dropdown")
                );
                columns.forEach((column) => {
                    const dropdownContent = column.querySelector(".dropdown-content");
                    const checkboxesDiv = dropdownContent.querySelector(".checkboxes");
                    const selectAllDiv = dropdownContent.querySelector(".select-all-div");

                    const selectAllCheckbox = document.createElement("input");
                    const selectAllLabel = document.createElement("label");

                    selectAllCheckbox.type = "checkbox";
                    selectAllCheckbox.addEventListener("change", function () {
                        const checkboxes = Array.from(checkboxesDiv.querySelectorAll("input[type='checkbox']"));
                        if (selectAllCheckbox.checked) {
                            checkedCheckboxes = checkedCheckboxes.filter((cb) => !checkboxes.includes(cb));
                            checkedCheckboxes.push(...checkboxes);

                        }
                        else {
                            checkedCheckboxes = checkedCheckboxes.filter((cb) => !checkboxes.includes(cb));

                        }
                        checkboxes.forEach((checkbox) => {
                            checkbox.checked = selectAllCheckbox.checked;
                        });
                        loadCheckboxes();
                    });

                    selectAllDiv.appendChild(selectAllCheckbox);
                    selectAllLabel.textContent = "Select All";
                    selectAllDiv.appendChild(selectAllLabel);

                    selectAllDiv.appendChild(document.createElement("br"));

                    checkboxesDiv.innerHTML = "";
                    const values = Array.from(new Set(Array.from(
                        document.querySelectorAll(`td:nth-child(${columns.indexOf(column) + 2})`)).map((td) => td.textContent)));

                    const fragment = document.createDocumentFragment();
                    values.forEach((value) => {
                        const div = document.createElement("div");
                        const checkbox = document.createElement("input");
                        checkbox.type = "checkbox";
                        checkbox.value = value;
                        checkbox.addEventListener("change", function () {
                            const selectAll = checkboxesDiv.querySelector("input[type='checkbox']");
                            if (!checkbox.checked && selectAll.checked) {
                                selectAll.checked = false;
                            }
                            if (checkbox.checked)
                                checkedCheckboxes.push(checkbox);
                            else
                                checkedCheckboxes = checkedCheckboxes.filter((cb) => cb !== checkbox);
                            loadCheckboxes();
                        });
                        div.appendChild(checkbox);

                        const label = document.createElement("label");
                        label.textContent = value;
                        div.appendChild(label);

                        div.appendChild(document.createElement("br"));
                        fragment.appendChild(div);
                    });
                    checkboxesDiv.appendChild(fragment);
                });

            }

            function generateColumnCheckboxes(dropdownContent) {
                const column = dropdownContent.parentElement;
                const columnIndex = Array.from(document.querySelectorAll("th")).indexOf(column) + 1;
                const checkboxesDiv = dropdownContent.querySelector(".checkboxes");
                const divs = checkboxesDiv.querySelectorAll("div");

                divs.forEach((div) => {
                    const checkbox = div.querySelector("input[type='checkbox']");
                    if (!checkbox.checked) {
                        div.remove();
                    }
                });

                const values = Array.from(new Set(Array.from(
                    document.querySelectorAll(`tr.in-filter td:nth-child(${columnIndex})`)).map((td) => td.textContent)));

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
                        selectAll = checkboxesDiv.querySelector("input[type='checkbox']");
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
                document.getElementById("nameInput").value = "";

                document.querySelectorAll(".filtered-div").forEach(div => {
                    div.classList.remove('filtered-div');
                });

                document.querySelectorAll("tbody tr").forEach(row => {
                    row.classList.add('in-filter');
                });
                if (checkedCheckboxes.length !== 0) {
                    checkedCheckboxes.forEach((checkbox, index) => {
                        filterTable(checkbox);
                        if (index === checkedCheckboxes.length - 1) {
                            document.querySelectorAll(".dropdown-content").forEach((column) => {
                                if (column !== checkbox.closest(".dropdown-content"))
                                    generateColumnCheckboxes(column);
                            });
                        }
                        console.log(checkedCheckboxes);

                    });
                }
                else {
                    document.querySelectorAll(".dropdown-content").forEach((column) => {
                        generateColumnCheckboxes(column);
                    });
                }


                updateRows();

            }
            function filterTable(checkbox) {
                if (!checkbox.parentElement) console.log("checkbox is null");
                const parentDropdown = checkbox.closest(".dropdown-content");
                const columnNum = Array.from(document.querySelectorAll(".dropdown-content")).indexOf(parentDropdown) + 2;
                const columnName = parentDropdown.id;

                if (parentDropdown.classList.contains('filtered-div')) {

                    document.querySelectorAll("tbody tr:not(.in-filter)").forEach(row => {
                        const cell = row.querySelector(`td:nth-child(${columnNum})`);
                        if (cell.textContent === checkbox.value) {
                            row.classList.add('in-filter');
                        }
                    });
                }
                else {
                    document.querySelectorAll("tr.in-filter").forEach(row => {
                        const cell = row.querySelector(`td:nth-child(${columnNum})`);

                        if (cell.textContent != checkbox.value) {
                            row.classList.remove('in-filter');
                        }

                    });
                    parentDropdown.classList.add('filtered-div');
                }

            }

            function updateRows() {
                currentPage = 1;

                filteredRows = Array.from(document.querySelectorAll("tbody tr.in-filter"));
                rowCounter = filteredRows.length;
                const counterElement = document.getElementById("rowCounter");
                counterElement.textContent = "Displayed Rows: " + rowCounter;
                updatePagination();
                displayRows();

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

            document.getElementById("nameInput").addEventListener("keyup", function () {
                const filter = this.value.toUpperCase();
                if (filter === "") {
                    filteredRows.forEach(row => {
                        row.style.display = "";
                    });
                }
                else {
                    filteredRows.forEach(row => {
                        const cellValue = row.querySelector("td:first-child");
                        if (cellValue.textContent.toUpperCase().indexOf(filter) > -1) {
                            row.style.display = "";
                        } else {
                            row.style.display = "none";
                        }

                    });
                }

            });

            document.getElementById("reset-button").addEventListener("click", function () {
                event.preventDefault();
                document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => {
                    checkbox.checked = false;
                });
                document.querySelectorAll("input[type='text']").forEach((input) => {
                    input.value = "";
                });
                checkedCheckboxes = [];

                loadCheckboxes();


            });

            add1000rows();
            createCheckboxes();
            updateRows();

        </script>

    </form>
</body>

</html>
