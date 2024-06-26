const ROWS_PER_PAGE = 20;
const MAX_PAGE = 10;
var screenName = "";
let data = [];
let currentPage = 1;
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
            const checkboxes = selectAllCheckbox.closest(".dropdown-content").querySelectorAll("input[type='checkbox']:not([style*='display: none'])");
            const visibleCheckboxes = Array.from(checkboxes).filter(checkbox => checkbox.parentElement.style.display !== "none");
            visibleCheckboxes.forEach(checkbox => { checkbox.checked = selectAllCheckbox.checked; });
            filterTable();
        });
    });
    document.querySelectorAll(".dropdown-content").forEach((column) => {
        generateColumnCheckboxes(column);
    });
    renderPagination();
    updateCounter();
    sortDatas(0);
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

    const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]].toString().trim()))];
    values.sort(function (a, b) {
        if (!isNaN(a) && !isNaN(b)) {
            return ascending ? a - b : b - a;
        }
        else if (columnIndex == 9) {
            if (a == "-") return 1;
            if (b == "-") return -1;
            a = new Date(a);
            b = new Date(b);
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
    const checkedValuesLength = checkedValues.map(values => values.length);
    const sumOfArrayLengths = checkedValuesLength.reduce((total, length) => total + length, 0);
    console.log(sumOfArrayLengths);
    filteredData = data.filter(row => {
        return checkedValues.every((values, columnIndex) => {
            if (values.length === 0) return true;
            return values.includes(row[Object.keys(row)[columnIndex]].toString());
        });
    });
    columns.forEach((column) => {
        if (column !== lastSelectedColumn || sumOfArrayLengths == 0) {
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
            sortDatas(index);
        }
    });
});

document.getElementById('reset-button').addEventListener('click', () => {
    event.preventDefault();
    document.querySelectorAll("input[type='checkbox']").forEach((checkbox) => { checkbox.checked = false; });
    document.querySelectorAll("input[type='text']").forEach((input) => { input.value = ""; });
    ascending = true;
    filterTable();
    sortDatas(0);
});
        
