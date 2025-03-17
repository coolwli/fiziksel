const MAX_PAGE = 10;
let rowsPerPage = 50;
let baseData = [];

let currentPage = 1;
let isAscending = true;
let filteredData = [];

function sortTableByColumn(columnIndex) {
    filteredData.sort((rowA, rowB) => {
        const valueA = rowA[columns[columnIndex].name].toString().trim();
        const valueB = rowB[columns[columnIndex].name].toString().trim();
        if (!isNaN(valueA) && !isNaN(valueB)) {
            return isAscending ? valueA - valueB : valueB - valueA;
        }
        if (Date.parse(valueA) && Date.parse(valueB)) {
            return isAscending ? new Date(valueA) - new Date(valueB) : new Date(valueB) - new Date(valueA);
        }
        return isAscending ? valueA.localeCompare(valueB) : valueB.localeCompare(valueA);
    });
    currentPage = 1;
    renderTablePage(currentPage, filteredData);
}

function renderTablePage(pageNumber, data) {
    const startIndex = (pageNumber - 1) * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    const paginatedData = data.slice(startIndex, endIndex);

    const tableBody = document.getElementById('tableBody');
    tableBody.innerHTML = '';

    if (paginatedData.length === 0) {
        const row = document.createElement('tr');
        const cell = document.createElement('td');
        cell.colSpan = columns.length;
        cell.textContent = "No data available";
        row.appendChild(cell);
        tableBody.appendChild(row);
        return;
    }

    paginatedData.forEach(row => {
        const tableRow = document.createElement('tr');
        columns.forEach(column => {
            if (column.dontShow == true) return;
            const cell = document.createElement('td');
            cell.textContent = row[column.name];
            tableRow.appendChild(cell);
        });
        tableBody.appendChild(tableRow);
    });
}

function renderPaginationControls(data) {
    const totalPages = Math.ceil(data.length / rowsPerPage);
    const paginationContainer = document.getElementById('pagination');
    paginationContainer.innerHTML = '';

    let startPage = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
    let endPage = Math.min(totalPages, currentPage + Math.floor(MAX_PAGE / 2));

    for (let i = startPage; i <= endPage; i++) {
        const button = document.createElement('button');
        button.textContent = i;
        button.className = 'page-link';
        if (i === currentPage) {
            button.classList.add('active');
        }
        button.addEventListener('click', () => {
            currentPage = i;
            renderTablePage(currentPage, data);
            renderPaginationControls(data);
        });
        paginationContainer.appendChild(button);
    }
}

function createTableColumns() {
    const tableHeader = document.getElementById('table-head');
    tableHeader.innerHTML = '';

    columns.forEach((column, index) => {
        if (column.dontShow == true) return;

        const headerCell = document.createElement('th');
        headerCell.addEventListener('click', () => {
            if (event.target.tagName !== 'TH') return;
            sortTableByColumn(index);
            isAscending = !isAscending;
        });

        if (column.hasSearchBar || column.onlyTH) {
            headerCell.innerHTML = column.label;
            tableHeader.appendChild(headerCell);
            return;
        }
        headerCell.classList.add('dropdown');
        headerCell.innerHTML = `${column.label}<span class="dropdown-arrow">&#9660;</span>`;

        const dropdownContent = document.createElement('div');
        dropdownContent.classList.add('dropdown-content');

        const searchInput = document.createElement('input');
        searchInput.type = 'text';
        searchInput.placeholder = 'Search';
        searchInput.addEventListener('keyup', () => filterCheckboxes(searchInput));

        dropdownContent.appendChild(searchInput);

        const selectAllContainer = document.createElement('div');
        selectAllContainer.classList.add('select-all-div');
        const selectAllCheckbox = document.createElement('input');
        selectAllCheckbox.type = 'checkbox';
        selectAllCheckbox.addEventListener('change', () => {
            const checkboxes = dropdownContent.querySelectorAll('.checkboxes div:not([style*="display: none"]) input[type="checkbox"]');
            checkboxes.forEach(checkbox => checkbox.checked = selectAllCheckbox.checked);
            applyFilters(dropdownContent);
        });
        const selectAllLabel = document.createElement('label');
        selectAllLabel.textContent = 'Select All';
        selectAllContainer.appendChild(selectAllCheckbox);
        selectAllContainer.appendChild(selectAllLabel);

        dropdownContent.appendChild(selectAllContainer);

        const checkboxesContainer = document.createElement('div');
        checkboxesContainer.classList.add('checkboxes');
        dropdownContent.appendChild(checkboxesContainer);

        dropdownContent.appendChild(document.createElement('br'));
        headerCell.appendChild(dropdownContent);
        tableHeader.appendChild(headerCell);

        generateCheckboxesForColumn(dropdownContent, index);
    });
}

function createSearchInput() {
    columns.forEach((column, index) => {
        if (column.hasSearchBar) {
            const searchInput = document.createElement('input');
            searchInput.type = 'text';
            searchInput.placeholder = `Search ${column.label}`;
            searchInput.addEventListener('input', () => {
                applyFilters(null);
            });
            document.querySelector('.table-top').prepend(searchInput);
        }
    });
}

function generateCheckboxesForColumn(dropdownContent, columnIndex) {
    const checkboxesContainer = dropdownContent.querySelector('.checkboxes');
    checkboxesContainer.querySelectorAll("div").forEach((div) => {
        const checkbox = div.querySelector("input[type='checkbox']");
        if (!checkbox.checked) {
            div.remove();
        }
    });

    const uniqueValues = [...new Set(filteredData.map(row => row[columns[columnIndex].name].toString().trim()))];
    uniqueValues.sort();

    const fragment = document.createDocumentFragment();
        
    uniqueValues.forEach(value => {
        if (checkboxesContainer.querySelector(`input[value='${value}']`)) {
            return;
        }
        const checkboxContainer = document.createElement("div");
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.value = value;

        checkbox.addEventListener("change", () => checkbox.checked ? applyFilters(dropdownContent) : applyFilters(null));

        const label = document.createElement("label");
        label.textContent = value;

        checkboxContainer.appendChild(checkbox);
        checkboxContainer.appendChild(label);
        checkboxContainer.appendChild(document.createElement("br"));
        fragment.appendChild(checkboxContainer);

        checkboxesContainer.appendChild(fragment);
    });
}

function applyFilters(lastSelectedDropdown) {
    const selectedFilters = Array.from(document.querySelectorAll("th"))
        .map((column) => {
            if (column.classList.contains("dropdown")) {
                const checkboxesContainer = column.querySelector(".checkboxes");
                if (checkboxesContainer) {
                    return Array.from(checkboxesContainer.querySelectorAll("input[type='checkbox']:checked"))
                        .map(checkbox => checkbox.value);
                }
            }
            return [];
        });

    filteredData = baseData.filter(row => {
        const matchesFilters = selectedFilters.every((values, columnIndex) => {
            if (values.length === 0) return true;
            const columnName = columns[columnIndex].name;
            return values.includes(row[columnName]);
        });
        return matchesFilters;
    });

   
    columns.forEach((column, index) => {
        if (column.hasSearchBar || column.onlyTH || column.dontShow) return;

        const headerCell = document.querySelectorAll("th")[index];
        const dropdownContent = headerCell.querySelector(".dropdown-content");

        if (dropdownContent !== lastSelectedDropdown || selectedFilters.flat().length === 0) {
            generateCheckboxesForColumn(dropdownContent, index);
        }
    });

    currentPage = 1;
    renderTablePage(currentPage, filteredData);
    renderPaginationControls(filteredData);
    updateRowCounter(filteredData);
}

function updateRowCounter() {
    document.getElementById('rowCounter').textContent = `${filteredData.length} Rows Listed..`;
}

function setRowsPerPage(selectedButton) {
    const buttons = document.querySelector(".button-container").querySelectorAll('.button');
    buttons.forEach(button => button.classList.remove('active'));
    selectedButton.classList.add('active');
    rowsPerPage = parseInt(selectedButton.textContent);
    currentPage = 1;
    renderPaginationControls(filteredData);
    renderTablePage(currentPage, filteredData);
}

document.getElementById('reset-button').addEventListener('click', (event) => {
    event.preventDefault();
    document.querySelectorAll("input[type='checkbox']").forEach(checkbox => checkbox.checked = false);
    document.querySelectorAll('.hall-button').forEach(button => { if (button.innerText != "All Halls") button.classList.remove('active'); else button.classList.add('active'); });

    document.querySelectorAll("input[type='text']").forEach(input => input.value = '');
    filteredData = baseData;
    currentPage = 1;
    applyFilters();
});

document.getElementById('export-button').addEventListener('click', () => {
    event.preventDefault();
    const hdnInput = document.getElementById('hiddenField');
    var jsondata = JSON.stringify(filteredData);
    hdnInput.value = jsondata;
    document.getElementById('hiddenButton').click();
});

function filterCheckboxes(input) {
    const checkboxes = input.parentElement.querySelectorAll(".checkboxes input[type='checkbox']");
    checkboxes.forEach(checkbox => {
        const label = checkbox.nextElementSibling;
        if (label.textContent.toLowerCase().includes(input.value.toLowerCase())) {
            checkbox.parentElement.style.display = '';
        } else {
            checkbox.parentElement.style.display = 'none';
        }
    });
}

function initializeTable() {
    filteredData = baseData;
    createTableColumns();
    createSearchInput();
    applyFilters();
}
