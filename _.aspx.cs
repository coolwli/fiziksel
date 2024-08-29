const MAX_PAGE = 10;
let rowsPerPage = 20;
let currentPage = 1;
let ascending = true;
let data = [];
let filteredData = [];
let screenName = "";

// Utility Functions
function getColumnValue(row, index) {
    return row[Object.keys(row)[index]];
}

function compareValues(a, b, index) {
    const aValue = getColumnValue(a, index);
    const bValue = getColumnValue(b, index);

    if (!isNaN(aValue) && !isNaN(bValue)) {
        return ascending ? aValue - bValue : bValue - aValue;
    }

    if (index === 11) {
        if (aValue === "-" && bValue !== "-") return 1;
        if (bValue === "-" && aValue !== "-") return -1;
        return ascending ? new Date(aValue) - new Date(bValue) : new Date(bValue) - new Date(aValue);
    }

    return ascending 
        ? aValue.trim().localeCompare(bValue.trim()) 
        : bValue.trim().localeCompare(aValue.trim());
}

// Sorting Function
function sortData(index) {
    filteredData.sort((a, b) => compareValues(a, b, index));
    currentPage = 1;
    renderPage(currentPage);
}

// Pagination Rendering
function renderPagination() {
    const totalPages = Math.ceil(filteredData.length / rowsPerPage);
    const paginationDiv = document.getElementById('pagination');
    paginationDiv.innerHTML = '';

    const startPage = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
    const endPage = Math.min(totalPages, currentPage + Math.floor(MAX_PAGE / 2));

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
        if (i === currentPage) button.classList.add('active');
        button.addEventListener('click', () => {
            currentPage = i;
            renderPage(currentPage);
            renderPagination();
        });
        paginationDiv.appendChild(button);
    }
}

// Page Rendering
function renderPage(pageNumber) {
    const start = (pageNumber - 1) * rowsPerPage;
    const end = start + rowsPerPage;
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
        tr.addEventListener('click', () => {
            window.location.href = `${screenName}.aspx?id=${row[Object.keys(row)[0]]}`;
        });
        tableBody.appendChild(tr);
    });
}

// Filtering Function
function filterTable() {
    const columns = document.querySelectorAll(".dropdown-content");
    const checkedValues = Array.from(columns).map(column => 
        Array.from(column.querySelectorAll("input[type='checkbox']:checked")).map(checkbox => checkbox.value)
    );

    filteredData = data.filter(row => {
        return checkedValues.every((values, index) => 
            values.length === 0 || values.includes(row[Object.keys(row)[index + 1]].toString())
        );
    });

    currentPage = 1;
    renderPagination();
    renderPage(currentPage);
    updateCounter();
}

// Column Checkbox Generation
function generateColumnCheckboxes(dropdownContent) {
    const columnIndex = Array.from(document.querySelectorAll(".dropdown-content")).indexOf(dropdownContent) + 1;
    const checkboxesDiv = dropdownContent.querySelector(".checkboxes");
    checkboxesDiv.innerHTML = '';

    const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]].toString().trim()))];
    values.sort((a, b) => compareValues({ [Object.keys(data[0])[columnIndex]]: a }, { [Object.keys(data[0])[columnIndex]]: b }, columnIndex));

    values.forEach(value => {
        if (checkboxesDiv.querySelector(`input[value='${value}']`)) return;

        const div = document.createElement("div");
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.value = value;
        checkbox.addEventListener("change", function () {
            filterTable();
        });

        const label = document.createElement("label");
        label.textContent = value;

        div.appendChild(checkbox);
        div.appendChild(label);
        div.appendChild(document.createElement("br"));
        checkboxesDiv.appendChild(div);
    });
}

// Search Checkbox Function
function searchCheckboxes(searchInput) {
    const filter = searchInput.value.toUpperCase();
    const checkboxesDiv = searchInput.parentElement.querySelector(".checkboxes");
    Array.from(checkboxesDiv.getElementsByTagName("div")).forEach(div => {
        const label = div.querySelector("label");
        const txtValue = label.textContent || label.innerText;
        div.style.display = txtValue.toUpperCase().includes(filter) ? "" : "none";
    });
}

// Counter Update
function updateCounter() {
    document.getElementById('rowCounter').textContent = `${filteredData.length} Rows Listed..`;
}

// Rows Per Page Adjustment
function setRowsPerPage(selectedButton) {
    const buttons = document.querySelectorAll(".button-container .button");
    buttons.forEach(button => button.classList.remove('active'));
    selectedButton.classList.add('active');
    rowsPerPage = parseInt(selectedButton.textContent, 10);
    currentPage = 1;
    renderPagination();
    renderPage(currentPage);
}

// Initialization
function initializeTable() {
    filteredData = data;
    document.querySelectorAll(".select-all-div input[type='checkbox']").forEach(selectAllCheckbox => {
        selectAllCheckbox.addEventListener("change", () => {
            const checkboxes = selectAllCheckbox.closest(".dropdown-content").querySelectorAll("input[type='checkbox']:not([style*='display: none'])");
            checkboxes.forEach(checkbox => checkbox.checked = selectAllCheckbox.checked);
            filterTable();
        });
    });

    document.querySelectorAll(".dropdown-content").forEach(generateColumnCheckboxes);
    renderPagination();
    updateCounter();
    sortData(1);
}

// Event Listeners
document.querySelectorAll("th").forEach((th, index) => {
    th.addEventListener('click', (event) => {
        if (!event.target.closest('.dropdown-content')) {
            ascending = !ascending;
            sortData(index);
        }
    });
});

document.getElementById('reset-button').addEventListener('click', (event) => {
    event.preventDefault();
    document.querySelectorAll("input[type='checkbox']").forEach(checkbox => checkbox.checked = false);
    document.querySelectorAll("input[type='text']").forEach(input => input.value = "");
    ascending = true;
    filterTable();
    sortData(0);
});
