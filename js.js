const ROWS_PER_PAGE = 20;
const MAX_PAGE = 10;
var screenName = "";
let data = [];

let currentPage = 1;
let checkedCheckboxes = [];
let ascending = true;
let filteredData = [];

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
                window.location.href = screenName+".aspx" + "?id=" + row[Object.keys(row)[0]];
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
    filteredData = data;
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
    updateCounter();
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
    renderPage(currentPage);
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
    document.getElementById('rowCounter').textContent = `${filteredData.length} Satır Listelendi..`;
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
