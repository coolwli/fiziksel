function getColumnIndex(columnName) {
    const contentTable = document.getElementById("contentTable");
    return Array.from(contentTable.rows[0].cells).findIndex(cell => cell.innerText.trim() === columnName);
}

function showContentTable(column,tableId) {
    const columnIndex = getColumnIndex(column);
    const table = document.getElementById(tableId);

    table.innerHTML = "";
    const dataCounts = filteredData.reduce((counts, row) => {
        const cell = row[Object.keys(row)[columnIndex]].trim();
        counts[cell] = (counts[cell] || 0) + 1;
        return counts;
    }, {});

    Object.entries(dataCounts).forEach(([data, count]) => {
        const row = table.insertRow();
        const cell = row.insertCell(0);
        const countCell = row.insertCell(1);
        cell.textContent = data;
        countCell.textContent = count;
    });
}

function setRowsPerPage(selectedButton) {
    const buttons = document.querySelector(".button-container").querySelectorAll('.button');
    buttons.forEach(button => button.classList.remove('active'));
    selectedButton.classList.add('active');
    ROWS_PER_PAGE = parseInt(selectedButton.textContent);
    CURRENT_PAGE = 1;
    renderPagination();
    renderPage(CURRENT_PAGE);
}

function searchCheckboxes(searchInput) {
    const filter = searchInput.value.toUpperCase();
    const checkboxesDiv = searchInput.parentElement.querySelector(".checkbox-panel");
    const divs = checkboxesDiv.getElementsByTagName("div");
    for (let i = 0; i < divs.length; i++) {
        const label = divs[i].getElementsByTagName("label")[0];
        const txtValue = label.textContent || label.innerText;
        divs[i].style.display = txtValue.toUpperCase().indexOf(filter) > -1 ? "" : "none";
    }
}

function calculateTotalValues(columnIndex) {
    return filteredData.reduce((total, row) => total + (!isNaN(row[Object.keys(row)[columnIndex]]) ? Number(row[Object.keys(row)[columnIndex]]) : 0), 0);
}

function setCounters() {
    document.getElementById('row-count').textContent = filteredData.length +" Envanter Listelendi.";
    document.getElementById('soket-count').textContent = calculateTotalValues(6);
    document.getElementById('core-count').textContent = calculateTotalValues(7);
    document.getElementById('cpu-count').textContent = calculateTotalValues(8);
    document.getElementById('memory-count').textContent = calculateTotalValues(9);

}

function handleDropdownChange() {
    const selectedColumn = document.getElementById('column-dropdown').value;
    document.querySelector('.dropdown-panel h5').textContent = selectedColumn;
    const input = document.querySelector('.dropdown-panel input[type="text"]');
    const checkboxesDiv = document.querySelector('.dropdown-panel .checkbox-panel');
    checkboxesDiv.innerHTML = '';
    input.style.display = selectedColumn === "Select Extra Column" ? "none" : "block";
    filterTable();
}

function addOptionsDropdown() {
    const dropdown = document.getElementById('column-dropdown');
    dropdown.innerHTML = '';
    const firstOption = document.createElement('option');
    firstOption.value = "Select Extra Column";
    firstOption.textContent = "Select Extra Column";
    dropdown.appendChild(firstOption);

    const contentTable = document.getElementById('contentTable');
    contentTable.querySelectorAll('th').forEach(column => {
        const columnName = column.innerText.trim();
        const excludedColumns = ["Server", "Vendor", "Model", "Kapsam-BBVA Metrics", "Enviroment", "OS", "HWType", "Company", "DC Location", "Responsible Group","Notes"];
        if (!excludedColumns.includes(columnName)) {
            const option = document.createElement('option');
            option.value = columnName;
            option.textContent = columnName;
            dropdown.appendChild(option);
        }
    });
}


function page_load() {
    document.querySelectorAll("th").forEach((th, index) => {
        th.addEventListener('click', () => {
            ascending = !ascending;
            sortDatas(index);
        });
    });
    document.getElementById('reset-button').addEventListener('click', (event) => {
        event.preventDefault();
        document.querySelectorAll("input[type='checkbox']").forEach(checkbox => checkbox.checked = false);
        document.querySelectorAll("input[type='text']").forEach(input => input.value = "");
        ascending = true;
        filterTable();
        sortDatas(0);
    });
    document.getElementById('export-button').addEventListener('click', (event) => {
        event.preventDefault();
        const hdnInput = document.getElementById('hiddenField');
        var jsondata = JSON.stringify(filteredData);
        hdnInput.value = jsondata;
        console.log(hdnInput.value);

        hdnInput.value = jsondata;
        document.getElementById('hiddenButton').click();
    });
    document.getElementById('column-dropdown').addEventListener('change', handleDropdownChange);

    document.getElementById('server-search').addEventListener('keyup', function () {
        filterTable();
        /*
        searchedData = filteredData.filter(row => row.Server.toString().trim().toUpperCase().includes(this.value.toUpperCase()));
        renderPage(CURRENT_PAGE, searchedData);*/
    });
    document.querySelectorAll('.search-input').forEach(input => {
        input.addEventListener('keyup', () => {
            searchCheckboxes(input);
        });
    });
    showContentTable("Vendor", "vendor-table");
    showContentTable("HWType", "hwtype-table");

}
