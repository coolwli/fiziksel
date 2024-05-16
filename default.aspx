<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="vminfo._default" %>

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
            float:left;
            text-transform: uppercase;
            padding: 10px 15px;
            line-height: 10px;
            position: relative;
            text-align: center;
            border-radius: 4px;
        }
        .header a:hover {
            background-color:  #383d42;
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
            background-size: cover;float:right;
            width: 55px;
            height: 70px;
            margin-right:10px;
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

        #vcDropdown{
            text-transform: uppercase;
            padding: 10px 15px;
            margin:10px 2px;

        }
        #nameInput {
            width: 75%;
            padding: 9px 15px;
            margin:10px 2px;


        }
        table {
            border-collapse: separate;
            border-spacing: 0;
            width: 100%;
            margin: 5px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            font-size:10px;
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
            width:auto
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
            text-align:left;
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
            margin-bottom:20px;
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
                <a class="active"><h2>VM</h2></a>
                <a href="vmhost.aspx"><h2>VM Host</h2></a>
                <a href="cluster.aspx"><h2>Cluster</h2></a>
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
            <input type="text" id="nameInput"  placeholder="Search for VMs..">
        </div>
        <div class="table-container">
            <table id="contentTable" >
                <thead>
                    <tr>
                        <th>
                            Name
                        </th>
                        <th class="dropdown">
                            CPU<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="cpuDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Memory(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="memoryDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Total Disk(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="totaldiskDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Power State <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="powerstateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Cluster <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="clusterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Data Center <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="datacenterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Owner <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ownerDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Created Date <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="createddateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server" >
                </tbody>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>
        <button id="reset-button">Reset</button>
        <p id="rowCounter"></p>
        <script>
            const maxPage = 10;
            const rowsPerPage = 20;
            const paginationElement = document.getElementById("pagination");

            var rowCounter = Array.from(document.querySelectorAll("tbody tr")).length;
            var currentPage=1;
            var filtredRows;

            function displayRows() {
                let length = currentPage * rowsPerPage;
                if (length > rowCounter) length = rowCounter;
                //console.log(length-(currentPage-1) * rowsPerPage);

                for (let i = 0; i < Array.from(document.querySelectorAll("tbody tr")).length; i++) {
                    console.log(i);
                    Array.from(document.querySelectorAll("tbody tr"))[i].style.display = 'none';

                }
                for (let i = (currentPage - 1) * rowsPerPage; i < length; i++) {
                    console.log(i);
                    filtredRows[i].style.display = '';
                }

            }

            function setupPagination(start, end) {

                if (rowCounter <= 0) return;
                paginationElement.innerHTML = '';
                if ((end - start) >= (maxPage - 1)) end = maxPage + start - 1;

                for (let i = start; i <= end; i++) {
                    const pageLink = document.createElement('a');
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
                if (Math.ceil(rowCounter / rowsPerPage) > maxPage) {
                    if (currentPage > (maxPage / 2)) {
                        setupPagination(currentPage - (maxPage / 2), Math.ceil(rowCounter / rowsPerPage));
                        pageLinks[maxPage / 2].classList.add('active');

                    }
                    else {
                        setupPagination(1, Math.ceil(rowCounter / rowsPerPage));
                        pageLinks[currentPage - 1].classList.add('active');
                    }
                }
                else {
                    pageLinks[currentPage - 1].classList.add('active');

                }
            }
            
        </script>
        <script>

            function generateTable() {
                const columns = Array.from(
                    document.querySelectorAll("th.dropdown")
                );
                columns.forEach((column) => {
                    const dropdownContent = column.querySelector(".dropdown-content");
                    const checkboxesDiv = dropdownContent.querySelector(".checkboxes");
                    const columnName = dropdownContent.id;

                    checkboxesDiv.innerHTML = "";
                    const selectAllCheckbox = document.createElement("input");
                    selectAllCheckbox.type = "checkbox";
                    selectAllCheckbox.addEventListener("change", function () {
                        const checkboxes = checkboxesDiv.querySelectorAll(
                            "input[type='checkbox']"
                        );
                        checkboxes.forEach((checkbox) => {
                            checkbox.checked = selectAllCheckbox.checked;
                        });
                        filterTable(columnName);
                    });
                    const selectAllDiv = document.createElement("div");

                    selectAllDiv.appendChild(selectAllCheckbox);

                    const selectAllLabel = document.createElement("label");
                    selectAllLabel.textContent = "Select All";
                    selectAllDiv.appendChild(selectAllLabel);

                    const values = Array.from(
                        new Set(
                            Array.from(
                                document.querySelectorAll(
                                    `td:nth-child(${columns.indexOf(column) +2})`
                                )
                            ).map((td) => td.textContent)
                        )
                    );
                    selectAllDiv.appendChild(document.createElement("br"));
                    checkboxesDiv.appendChild(selectAllDiv);

                    values.forEach((value) => {
                        const div = document.createElement("div");
                        const checkbox = document.createElement("input");
                        checkbox.type = "checkbox";
                        checkbox.value = value;
                        checkbox.addEventListener("change", function () {
                            selectAll = checkboxesDiv.querySelector("input[type='checkbox']");
                            if (!checkbox.checked && selectAll.checked) {
                                selectAll.checked = false;
                            }
                            filterTable(columnName);
                        });
                        div.appendChild(checkbox);

                        const label = document.createElement("label");
                        label.textContent = value;
                        div.appendChild(label);

                        div.appendChild(document.createElement("br"));
                        checkboxesDiv.appendChild(div);
                    });
                });
                updateCount();
                displayRows();
                setupPagination(1, Math.ceil(rowCounter / rowsPerPage));

            }

            function generateColumnCheckboxes(dropdownContent) {
                const column = dropdownContent.parentElement;
                const columnName = dropdownContent.id;
                const checkboxesDiv = dropdownContent.querySelector(".checkboxes");
                const divs = checkboxesDiv.querySelectorAll("div");

                divs.forEach((div) => {
                    const checkbox = div.querySelector("input");
                    const label = div.querySelector("label");
                    if (!checkbox.checked && label.textContent !== "Select All") {
                        checkboxesDiv.removeChild(div);
                    }
                });
                const values = Array.from(
                    new Set(
                        filtredRows.flatMap(row => Array.from(row.querySelectorAll(
                            `td:nth-child(${Array.from(document.querySelectorAll("th.dropdown")).indexOf(column) + 1})`
                        ))).map((td) => td.textContent)
                    )
                );

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
                        filterTable(columnName);
                    });
                    div.appendChild(checkbox);

                    const label = document.createElement("label");
                    label.textContent = value;
                    div.appendChild(label);

                    div.appendChild(document.createElement("br"));
                    checkboxesDiv.appendChild(div);
                });
            }

            function filterTable(columnName) {
                const columns = Array.from(
                    document.querySelectorAll("th.dropdown")
                );
                const filters = {};

                columns.forEach((column) => {
                    const columnName = column.textContent.trim();
                    const dropdownContent = column.querySelector(".dropdown-content");
                    const checkedValues = Array.from(
                        dropdownContent.querySelectorAll("input:checked")
                    ).map((checkbox) => checkbox.value);
                    filters[columnName.toLowerCase()] = checkedValues;
                });

                const rows = Array.from(document.querySelectorAll("tbody tr"));
                rows.forEach((row) => {
                    const cells = Array.from(row.querySelectorAll("td"));
                    const inFilter = cells.every((cell, index) => {
                        const columnName = columns[index].textContent
                            .trim()
                            .toLowerCase();
                        const cellValue = cell.textContent;
                        return (
                            filters[columnName].length === 0 ||
                            filters[columnName].includes(cellValue)
                        );
                    })
                        ? true
                        : false;
                    if (inFilter) {
                        row.classList.add('in-filter');
                    }
                    else {
                        row.classList.remove('in-filter');
                    }
                });
                updateCount();
                currentPage = 1;
                setupPagination(1, Math.ceil(rowCounter / rowsPerPage));
                updatePagination();
                displayRows();


                Array.from(document.querySelectorAll(".dropdown-content")).forEach((column) => {
                    if (column.id !== columnName|| column.querySelectorAll("input:checked").length === 0) {
                        console.log(column.id,columnName);
                        generateColumnCheckboxes(column);
                    }
                });

            }

            function updateCount() {
                filtredRows = Array.from(document.querySelectorAll("tbody tr.in-filter"));
                rowCounter = filtredRows.length;
                const counterElement = document.getElementById("rowCounter");
                counterElement.textContent = "Displayed Rows: " + rowCounter;

            }

            generateTable();

        </script>
        <script>
            const resetButton = document.getElementById("reset-button");

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

            resetButton.addEventListener("click", function () {
                document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => {
                        checkbox.checked = false;
                    });
                filterTable(null);
            });


        </script>

    </form>
</body>
</html>
