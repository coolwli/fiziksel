const MAX_PAGE = 10;
let ROWS_PER_PAGE = 10;
let CURRENT_PAGE = 1;
let data = [];
let ascending = true;
let filteredData = [];



function sortDatas(index) {
    filteredData.sort(function (a, b) {
        let aValue = a[Object.keys(a)[index]];
        let bValue = b[Object.keys(b)[index]];


        if (!isNaN(aValue) && !isNaN(bValue)) {
            return ascending ? aValue - bValue : bValue - aValue;
        }
        else if (index === 9) {
            if (aValue == "-") return 1;
            if (bValue == "-") return -1;
            aValue = new Date(aValue.trim());
            bValue = new Date(bValue.trim());
            return ascending ? aValue - bValue : bValue - aValue;
        }
        return ascending ? aValue.trim().localeCompare(bValue.trim()) : bValue.trim().localeCompare(aValue.trim());
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
    document.querySelectorAll(".filter-panel").forEach((column) => {
        generateColumnCheckboxes(column);
    });
    renderPagination();
    updateCounter();
    sortDatas(0);
}

function getColumnIndex(columnName) {
    var contentTable=document.getElementById("contentTable");
    return Array.from(contentTable.rows[0].cells).findIndex(cell => cell.innerText.trim() === columnName);
}

function generateColumnCheckboxes(dropdownContent) {
    const columnIndex = Array.from(document.querySelectorAll(".dropdown-content")).indexOf(dropdownContent);
    const checkboxesDiv = dropdownContent.querySelector(".checkbox-panel");

    checkboxesDiv.querySelectorAll("div").forEach((div) => {
        const checkbox = div.querySelector("input[type='checkbox']");
        if (!checkbox.checked) {
            div.remove();
        }
    });

    const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]].toString().trim()))];
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
        if (checkbox.checked) {
            return;
        }
        checkbox.closest('.checkbox-content').parentElement.removeChild(checkbox.closest('.checkbox-content'));
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
    CURRENT_PAGE = 1;
    Array.from(document.querySelectorAll('h5')).forEach(h5 => h5.classList.remove('filtered'));


    if (labels.length === 0) {
        tableRows.forEach(row => row.classList.add('visible'));
        lastChangedColumn = null;

    }
    else {
        if (isBack) {
            tableRows.forEach(row => row.classList.add('visible'));
        }

        labels.forEach(function (label) {
            const columnName = label.closest(".panel") ? label.closest(".panel").querySelector("h5") : label.closest(".extra-panel").querySelector("h5");
            if (columnName.classList.contains('filtered')) {
                tableRows.forEach(row => {
                    if (row.classList.contains('visible')) return;
                    const cellValue = row.cells[getColumnIndex(columnName.innerText)].innerText;
                    if (cellValue == label.innerText) row.classList.add('visible');
                    else row.classList.remove('visible');

                });
            }

            else {
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
            labels = [...labels, label];
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
    console.log(searchText);
    filterCheckboxes(panel, searchText);
}

function filterCheckboxes(panel, searchText) {
    const checkboxes = panel.querySelector('.checkbox-panel').querySelectorAll('.checkbox-content');
    const lowerCaseSearchText = searchText.toLowerCase();

    checkboxes.forEach(function (checkbox) {
        const label = checkbox.querySelector('label');
        const labelText = label.innerText.toLowerCase();
        const cont = checkbox.closest('.checkbox-content');
        cont.style.display = labelText.includes(lowerCaseSearchText) ? "block" : "none";
    });
}

function handleDropdownChange() {
    const dropdown = document.getElementById('column-dropdown');
    const extraCbs = Array.from(dropdown.closest(".extra-panel").querySelectorAll('input[type="checkbox"]:checked')).map(checkbox => checkbox);
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
    }
    else {
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
        icon = row.querySelectorAll('td')[0];
        icon.classList.add('edit-icon');
        icon.addEventListener('click', function () {
            editRow(row.querySelectorAll('td')[1].innerText);
        });
    });
}

document.getElementById('add-button').addEventListener('click', function () {
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

function selectRow(selectedButton) {
    const buttons = document.querySelector(".button-container").querySelectorAll('.button');
    buttons.forEach(button => button.classList.remove('active'));
    selectedButton.classList.add('active');
    ROWS_PER_PAGE = selectedButton.textContent;
    CURRENT_PAGE = 1;
    displayRows();

}
