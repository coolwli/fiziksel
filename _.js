const MAX_PAGE = 10;
let rowsPerPage = 20;
let currentPage = 1;
let isAscending = true;
let baseData = [];
let filteredData = [];
let filename = "vmpedia.csv";

function sortData(data, ascending = true) {
    if (!data || data.length === 0) return data;

    let sample = data.find((v) => v !== null && v !== undefined);
    const getType = (val) => {
        if (!isNaN(Date.parse(val))) return "date";
        if (!isNaN(parseFloat(val)) && isFinite(val)) return "number";
        if (typeof val === "string" && /^\d+(\.\d+)? /.test(val)) return "textwNum";
        return "string";
    };
    const type = getType(sample);

    return data.sort((a, b) => {
        let valA = a,
            valB = b;
        if (type === "number") {
            valA = parseFloat(a) || 0;
            valB = parseFloat(b) || 0;
        } else if (type === "date") {
            valA = new Date(a).getTime() || 0;
            valB = new Date(b).getTime() || 0;
        } else if (type === "textwNum") {
            valA = parseFloat((a || "").split(" ")[0]) || 0;
            valB = parseFloat((b || "").split(" ")[0]) || 0;
        }
        return ascending ? (valA > valB ? 1 : valA < valB ? -1 : 0) : valA < valB ? 1 : valA > valB ? -1 : 0;
    });
}

function renderTablePage(pageNumber, data) {
    const startIndex = (pageNumber - 1) * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    const paginatedData = data.slice(startIndex, endIndex);

    const tableBody = document.getElementById("tableBody");
    tableBody.innerHTML = "";

    if (paginatedData.length === 0) {
        const row = document.createElement("tr");
        const cell = document.createElement("td");
        cell.colSpan = columns.length;
        cell.textContent = "No data available";
        row.appendChild(cell);
        tableBody.appendChild(row);
        return;
    }

    paginatedData.forEach((row) => {
        const tableRow = document.createElement("tr");
        columns.forEach((column) => {
            if (column.dontShow) return;
            const cell = document.createElement("td");
            cell.textContent = row[column.name];
            tableRow.appendChild(cell);
        });
        if (rowClick == true) {
            tableRow.addEventListener("click", function () {
                window.location.href = screenName + ".aspx" + "?id=" + row[idNum] + "&vc=" + row[vcNum];
            });
        }
        tableBody.appendChild(tableRow);
    });
}

function renderPaginationControls(data) {
    const totalPages = Math.ceil(data.length / rowsPerPage);
    const paginationContainer = document.getElementById("pagination");
    paginationContainer.innerHTML = "";

    let startPage = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
    let endPage = Math.min(totalPages, currentPage + Math.floor(MAX_PAGE / 2));

    for (let i = startPage; i <= endPage; i++) {
        const button = document.createElement("button");
        button.textContent = i;
        button.className = "page-link";
        if (i === currentPage) {
            button.classList.add("active");
        }
        button.addEventListener("click", () => {
            currentPage = i;
            renderTablePage(currentPage, data);
            renderPaginationControls(data);
        });
        paginationContainer.appendChild(button);
    }
}

function createTableColumns(data) {
    const tableHeader = document.getElementById("table-head");
    tableHeader.innerHTML = "";

    columns.forEach((column, index) => {
        if (column.dontShow) return;

        const headerCell = document.createElement("th");
        headerCell.addEventListener("click", () => {
            if (event.target.tagName !== "TH") return;
            sortData(filteredData[columns[index].name], isAscending);
            currentPage = 1;
            renderTablePage(currentPage, filteredData);
            isAscending = !isAscending;
        });

        if (column.hasSearchBar || column.onlyTH) {
            headerCell.innerHTML = column.label;
            tableHeader.appendChild(headerCell);
            return;
        }

        headerCell.classList.add("dropdown");
        headerCell.innerHTML = `${column.label}<span class="dropdown-arrow">&#9660;</span>`;

        const dropdownContent = document.createElement("div");
        dropdownContent.classList.add("dropdown-content");

        const searchInput = document.createElement("input");
        searchInput.type = "text";
        searchInput.placeholder = "Search";
        searchInput.addEventListener("keyup", () => filterCheckboxes(searchInput));

        dropdownContent.appendChild(searchInput);

        const selectAllContainer = document.createElement("div");
        selectAllContainer.classList.add("select-all-div");
        const selectAllCheckbox = document.createElement("input");
        selectAllCheckbox.type = "checkbox";
        selectAllCheckbox.addEventListener("change", () => {
            const checkboxes = dropdownContent.querySelectorAll(
                '.checkboxes div:not([style*="display: none"]) input[type="checkbox"]'
            );
            checkboxes.forEach((checkbox) => (checkbox.checked = selectAllCheckbox.checked));
            applyFilters(dropdownContent);
        });
        const selectAllLabel = document.createElement("label");
        selectAllLabel.textContent = "Select All";
        selectAllContainer.appendChild(selectAllCheckbox);
        selectAllContainer.appendChild(selectAllLabel);

        dropdownContent.appendChild(selectAllContainer);

        const checkboxesContainer = document.createElement("div");
        checkboxesContainer.classList.add("checkboxes");
        dropdownContent.appendChild(checkboxesContainer);

        dropdownContent.appendChild(document.createElement("br"));
        headerCell.appendChild(dropdownContent);
        tableHeader.appendChild(headerCell);

        generateCheckboxesForColumn(dropdownContent, index, data);
    });
}

function filterCheckboxes(input) {
    const checkboxes = input.parentElement.querySelectorAll(".checkboxes input[type='checkbox']");
    checkboxes.forEach((checkbox) => {
        const label = checkbox.nextElementSibling;
        if (label.textContent.toLowerCase().includes(input.value.toLowerCase())) {
            checkbox.parentElement.style.display = "";
        } else {
            checkbox.parentElement.style.display = "none";
        }
    });
}

function generateCheckboxesForColumn(dropdownContent, columnIndex, data) {
    const checkboxesContainer = dropdownContent.querySelector(".checkboxes");

    checkboxesContainer.querySelectorAll("div").forEach((div) => {
        const checkbox = div.querySelector("input[type='checkbox']");
        if (!checkbox.checked) {
            div.remove();
        }
    });

    const uniqueValues = getUniqueValues(columnIndex, data);

    const fragment = document.createDocumentFragment();

    uniqueValues.forEach((value) => {
        if (checkboxesContainer.querySelector(`input[value = '${value}']`)) return;

        const checkboxContainer = document.createElement("div");
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.value = value;
        checkbox.addEventListener("change", () => applyFilters(dropdownContent));

        const label = document.createElement("label");
        label.textContent = value;

        checkboxContainer.appendChild(checkbox);
        checkboxContainer.appendChild(label);
        checkboxContainer.appendChild(document.createElement("br"));

        fragment.appendChild(checkboxContainer);
    });

    checkboxesContainer.appendChild(fragment);
}

function getUniqueValues(columnIndex, data) {
    const uniqueValues = [...new Set(data.map((row) => row[columns[columnIndex].name].toString().trim()))];
    return sortData(uniqueValues);
}

function applyFilters(lastSelectedDropdown = null) {
    const selectedFilters = Array.from(document.querySelectorAll("th")).map((column) => {
        if (column.classList.contains("dropdown")) {
            const checkboxesContainer = column.querySelector(".checkboxes");
            if (checkboxesContainer) {
                return Array.from(checkboxesContainer.querySelectorAll("input[type='checkbox']:checked")).map(
                    (checkbox) => checkbox.value
                );
            }
        }
        return [];
    });

    filteredData = baseData.filter((row) => {
        return selectedFilters.every((values, columnIndex) => {
            if (values.length === 0) return true;
            const columnName = columns[columnIndex].name;
            return values.includes(row[columnName].toString());
        });
    });

    columns.forEach((column, index) => {
        if (column.hasSearchBar || column.onlyTH || column.dontShow) return;

        const headerCell = document.querySelectorAll("th")[index];
        const dropdownContent = headerCell.querySelector(".dropdown-content");

        if (dropdownContent !== lastSelectedDropdown || selectedFilters.flat().length === 0) {
            generateCheckboxesForColumn(dropdownContent, index, filteredData);
        }
    });

    currentPage = 1;
    renderTablePage(currentPage, filteredData);
    renderPaginationControls(filteredData);
    updateRowCounter(filteredData);
}

function updateRowCounter(data) {
    document.getElementById("rowCounter").textContent = `${data.length} Rows Listed..`;
}

function setRowsPerPage(selectedButton, data = filteredData) {
    const buttons = document.querySelector(".button-container").querySelectorAll(".button");
    buttons.forEach((button) => button.classList.remove("active"));
    selectedButton.classList.add("active");
    rowsPerPage = parseInt(selectedButton.textContent);
    currentPage = 1;
    renderPaginationControls(data);
    renderTablePage(currentPage, data);
}

function initializeTable() {
    filteredData = baseData;
    createTableColumns(filteredData);
    createSearchInput();
    applyFilters();
    sortData(filteredData[columns[0].name], (isAscending = true));
    renderTablePage(currentPage, filteredData);
}

function createSearchInput() {
    columns.forEach((column, index) => {
        if (column.hasSearchBar) {
            const searchInput = document.createElement("input");
            searchInput.type = "text";
            searchInput.placeholder = `Search ${column.label}`;
            searchInput.addEventListener("input", () => applyFilters(null));
            document.querySelector(".table-top").prepend(searchInput);
        }
    });
}

function resetTableFilters() {
    document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => (checkbox.checked = false));
    document.querySelectorAll(".hall-button").forEach((button) => {
        if (button.innerText !== "All Halls") button.classList.remove("active");
        else button.classList.add("active");
    });
    document.querySelectorAll("input[type='text']").forEach((input) => (input.value = ""));
    filteredData = baseData;
    currentPage = 1;
    sortData(filteredData[columns[0].name], (isAscending = true));
    applyFilters();
}

function exportTableToCSV(filename) {
    if (!filteredData.length) return;
    const headers = Object.keys(filteredData[0]);
    const headerRow = headers
        .map((header) => {
            if (typeof header === "string" && (header.includes(",") || header.includes('"') || header.includes("\n"))) {
                return `"${header.replace(/"/g, '""')}"`;
            }
            return header;
        })
        .join(",");
    const dataRows = filteredData
        .map((row) =>
            Object.values(row)
                .map((cell) => {
                    if (typeof cell === "string" && (cell.includes(",") || cell.includes('"') || cell.includes("\n"))) {
                        return `"${cell.replace(/"/g, '""')}"`;
                    }
                    return cell;
                })
                .join(",")
        )
        .join("\n");
    const csvContent = headerRow + "\n" + dataRows;
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

document.getElementById("reset-button").addEventListener("click", (event) => {
    event.preventDefault();
    resetTableFilters();
});

document.getElementById("export-button").addEventListener("click", (event) => {
    event.preventDefault();
    exportTableToCSV();
});
