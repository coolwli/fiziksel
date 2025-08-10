const MAX_PAGE = 10;
let rowsPerPage = 20;
let baseData = [];
let filteredData = [];
let currentPage = 1;
let isAscending = true;
let filename = "vmpedia.csv";
let initialHiddenColumns = [];

function sortTableByColumn(columnIndex) {
    filteredData.sort((rowA, rowB) => {
        const key = columns[columnIndex].name;
        let aVal = rowA[key].toString().trim();
        let bVal = rowB[key].toString().trim();

        if (sortableColumnTypes.numericColumns.includes(columnIndex)) {
            aVal = isNaN(aVal) ? 0 : parseFloat(aVal);
            bVal = isNaN(bVal) ? 0 : parseFloat(bVal);
            return isAscending ? aVal - bVal : bVal - aVal;
        }

        if (sortableColumnTypes.dateColumns.includes(columnIndex)) {
            aVal = new Date(aVal).getTime() || 0;
            bVal = new Date(bVal).getTime() || 0;
            return isAscending ? aVal - bVal : bVal - aVal;
        }

        if (sortableColumnTypes.textwNumColumns.includes(columnIndex)) {
            aVal = aVal.split(" ")[0];
            bVal = bVal.split(" ")[0];
            return isAscending ? aVal - bVal : bVal - aVal;
        }

        return isAscending ? aVal.localeCompare(bVal) : bVal.localeCompare(aVal);
    });
}

function dynamicSort(values, columnIndex) {
    return values.sort((a, b) => {
        let aVal = a.toString().trim();
        let bVal = b.toString().trim();

        if (sortableColumnTypes.numericColumns.includes(columnIndex)) {
            aVal = isNaN(aVal) ? 0 : parseFloat(aVal);
            bVal = isNaN(bVal) ? 0 : parseFloat(bVal);
            return aVal - bVal;
        }

        if (sortableColumnTypes.dateColumns.includes(columnIndex)) {
            aVal = new Date(aVal).getTime() || 0;
            bVal = new Date(bVal).getTime() || 0;
            return aVal - bVal;
        }

        if (sortableColumnTypes.textwNumColumns.includes(columnIndex)) {
            aVal = aVal.split(" ")[0];
            bVal = bVal.split(" ")[0];
            return aVal - bVal;
        }

        return aVal.localeCompare(bVal);
    });
}

function renderTablePage(pageNumber, data) {
    const startIndex = (pageNumber - 1) * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    const pageRows = data.slice(startIndex, endIndex);

    const tbody = document.getElementById("tableBody");
    tbody.innerHTML = "";

    if (pageRows.length === 0) {
        const tr = document.createElement("tr");
        const td = document.createElement("td");
        td.colSpan = columns.length;
        td.textContent = "No data available";
        tr.appendChild(td);
        tbody.appendChild(tr);
        return;
    }

    pageRows.forEach((row) => {
        const tr = document.createElement("tr");
        columns.forEach((col, colIndex) => {
            if (col.dontShow) return;
            const td = document.createElement("td");
            td.dataset.colIndex = colIndex;
            td.textContent = row[col.name];
            tr.appendChild(td);
        });
        tbody.appendChild(tr);
    });
}

function renderPaginationControls(data) {
    const totalPages = Math.ceil(data.length / rowsPerPage);
    const container = document.getElementById("pagination");
    container.innerHTML = "";

    const startPage = Math.max(1, currentPage - Math.floor(MAX_PAGE / 2));
    const endPage = Math.min(totalPages, currentPage + Math.floor(MAX_PAGE / 2));

    for (let page = startPage; page <= endPage; page++) {
        const btn = document.createElement("button");
        btn.textContent = page;
        btn.className = "page-link";
        if (page === currentPage) btn.classList.add("active");
        btn.addEventListener("click", () => {
            currentPage = page;
            renderTablePage(currentPage, data);
            renderPaginationControls(data);
        });
        container.appendChild(btn);
    }
}

function createTableColumns() {
    const thead = document.getElementById("table-head");
    thead.innerHTML = "";

    columns.forEach((col, index) => {
        if (col.dontShow) return;

        const th = document.createElement("th");
        th.dataset.colIndex = index;
        th.addEventListener("click", (e) => {
            if (e.target.tagName !== "TH") return;
            sortTableByColumn(index);
            currentPage = 1;
            renderTablePage(currentPage, filteredData);
            renderPaginationControls(filteredData);
            updateRowCounter(filteredData);
            isAscending = !isAscending;
        });

        if (col.hasSearchBar || col.onlyTH) {
            th.innerHTML = col.label;
            const handle = document.createElement("span");
            handle.classList.add("resize-handle");
            th.appendChild(handle);
            thead.appendChild(th);
            return;
        }

        th.classList.add("dropdown");
        th.innerHTML = `${col.label}<span class="dropdown-arrow">&#9660;</span>`;

        const dropdown = document.createElement("div");
        dropdown.classList.add("dropdown-content");

        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Search";
        input.addEventListener("keyup", () => filterCheckboxes(input));
        dropdown.appendChild(input);

        const selectAllDiv = document.createElement("div");
        selectAllDiv.classList.add("select-all-div");
        const selectAllCb = document.createElement("input");
        selectAllCb.type = "checkbox";
        selectAllCb.addEventListener("change", () => {
            const boxes = dropdown.querySelectorAll(
                '.checkboxes div:not([style*="display: none"]) input[type="checkbox"]'
            );
            boxes.forEach((cb) => (cb.checked = selectAllCb.checked));
            applyFilters(dropdown);
        });
        const selectAllLbl = document.createElement("label");
        selectAllLbl.textContent = "Select All";
        selectAllDiv.appendChild(selectAllCb);
        selectAllDiv.appendChild(selectAllLbl);
        dropdown.appendChild(selectAllDiv);

        const list = document.createElement("div");
        list.classList.add("checkboxes");
        dropdown.appendChild(list);

        dropdown.appendChild(document.createElement("br"));
        th.appendChild(dropdown);
        const handle = document.createElement("span");
        handle.classList.add("resize-handle");
        th.appendChild(handle);
        thead.appendChild(th);

        generateCheckboxesForColumn(dropdown, index);
    });

    buildOrUpdateColumnsVisibilityMenu();
}

function makeResizable() {
    const headers = document.querySelectorAll("th");
    headers.forEach((header) => {
        const handle = header.querySelector(".resize-handle");
        if (!handle) return;
        handle.addEventListener("mousedown", (e) => {
            const startX = e.pageX;
            const startWidth = header.offsetWidth;
            const onMove = (ev) => {
                const newWidth = startWidth + (ev.pageX - startX);
                if (newWidth > 50) header.style.width = `${newWidth}px`;
            };
            const onUp = () => {
                document.removeEventListener("mousemove", onMove);
                document.removeEventListener("mouseup", onUp);
            };
            document.addEventListener("mousemove", onMove);
            document.addEventListener("mouseup", onUp);
        });
    });
}

function filterCheckboxes(input) {
    const checkboxes = input.parentElement.querySelectorAll(".checkboxes input[type='checkbox']");
    const term = input.value.toLowerCase();
    checkboxes.forEach((cb) => {
        const label = cb.nextElementSibling;
        cb.parentElement.style.display = label.textContent.toLowerCase().includes(term) ? "" : "none";
    });
}

function generateCheckboxesForColumn(dropdown, columnIndex) {
    const list = dropdown.querySelector(".checkboxes");
    list.querySelectorAll("div").forEach((div) => {
        const cb = div.querySelector("input[type='checkbox']");
        if (!cb.checked) div.remove();
    });

    const values = getUniqueValues(columnIndex);
    const frag = document.createDocumentFragment();
    values.forEach((val) => {
        if (list.querySelector(`input[value='${val}']`)) return;
        const row = document.createElement("div");
        const cb = document.createElement("input");
        cb.type = "checkbox";
        cb.value = val;
        cb.addEventListener("change", () => applyFilters(dropdown));
        const label = document.createElement("label");
        label.textContent = val;
        row.appendChild(cb);
        row.appendChild(label);
        row.appendChild(document.createElement("br"));
        frag.appendChild(row);
    });
    list.appendChild(frag);
}

function getUniqueValues(columnIndex) {
    const values = [...new Set(filteredData.map((row) => row[columns[columnIndex].name].toString().trim()))];
    return dynamicSort(values, columnIndex);
}

function applyFilters(lastSelectedDropdown) {
    const selectedFilters = Array.from({ length: columns.length }, () => []);
    document.querySelectorAll("th.dropdown").forEach((header) => {
        const idx = Number(header.dataset.colIndex);
        const list = header.querySelector(".checkboxes");
        if (list) {
            selectedFilters[idx] = Array.from(list.querySelectorAll("input[type='checkbox']:checked")).map(
                (cb) => cb.value
            );
        }
    });

    const searchInputs = {};
    columns.forEach((col) => {
        if (!col.hasSearchBar) return;
        const input = Array.from(document.querySelectorAll(".table-top input[type='text']")).find(
            (el) => el.placeholder === `Search ${col.label}`
        );
        if (input && input.value.trim() !== "") searchInputs[col.name] = input.value.trim().toLowerCase();
    });

    filteredData = baseData.filter((row) => {
        const checkboxMatch = selectedFilters.every((values, idx) => {
            if (values.length === 0) return true;
            const key = columns[idx].name;
            return values.includes(row[key].toString());
        });
        const searchMatch = Object.entries(searchInputs).every(([key, term]) => {
            return row[key] && row[key].toString().toLowerCase().includes(term);
        });
        return checkboxMatch && searchMatch;
    });

    columns.forEach((col, idx) => {
        if (col.hasSearchBar || col.onlyTH || col.dontShow) return;
        const header = Array.from(document.querySelectorAll("th")).find((th) => Number(th.dataset.colIndex) === idx);
        if (!header) return;
        const dropdown = header.querySelector(".dropdown-content");
        if (dropdown !== lastSelectedDropdown || selectedFilters.flat().length === 0) {
            generateCheckboxesForColumn(dropdown, idx);
        }
    });

    currentPage = 1;
    renderTablePage(currentPage, filteredData);
    renderPaginationControls(filteredData);
    updateRowCounter(filteredData);
}

function updateRowCounter() {
    document.getElementById("rowCounter").textContent = `Toplam ${filteredData.length} satır`;
}

function setRowsPerPage(selectedButton) {
    const buttons = document.querySelector(".button-container").querySelectorAll(".button");
    buttons.forEach((btn) => btn.classList.remove("active"));
    selectedButton.classList.add("active");
    rowsPerPage = parseInt(selectedButton.textContent);
    currentPage = 1;
    renderPaginationControls(filteredData);
    renderTablePage(currentPage, filteredData);
}

function initializeTable(data) {
    if (Array.isArray(data)) baseData = data;
    filteredData = baseData;
    initialHiddenColumns = columns.map((c) => !!c.dontShow);
    createTableColumns();
    makeResizable();
    createSearchInput();
    applyFilters();
    sortTableByColumn(0);
    renderTablePage(currentPage, filteredData);
}

function createSearchInput() {
    columns.forEach((col) => {
        if (col.hasSearchBar) {
            const input = document.createElement("input");
            input.type = "text";
            input.placeholder = `Search ${col.label}`;
            input.addEventListener("input", () => applyFilters(null));
            document.querySelector(".table-top").prepend(input);
        }
    });
}

document.getElementById("reset-button").addEventListener("click", (e) => {
    e.preventDefault();
    if (initialHiddenColumns && initialHiddenColumns.length === columns.length) {
        columns.forEach((c, i) => (c.dontShow = initialHiddenColumns[i]));
    } else {
        columns.forEach((c) => (c.dontShow = false));
    }
    document.querySelectorAll(".table-top input[type='text']").forEach((el) => (el.value = ""));
    filteredData = baseData;
    currentPage = 1;
    createTableColumns();
    renderTablePage(currentPage, filteredData);
    renderPaginationControls(filteredData);
    updateRowCounter(filteredData);
    makeResizable();
});

document.getElementById("export-button").addEventListener("click", (e) => {
    e.preventDefault();
    exportTableToCSV(filename);
});

function exportTableToCSV(filename) {
    if (!filteredData.length) return;
    const visibleColumns = columns.filter((c) => !c.dontShow);
    if (!visibleColumns.length) return;
    const escapeCSV = (val) => {
        const s = val == null ? "" : String(val);
        return s.includes(",") || s.includes("\n") || s.includes('"') ? `"${s.replace(/"/g, '""')}"` : s;
    };
    const headerRow = visibleColumns.map((c) => escapeCSV(c.label || c.name)).join(",");
    const dataRows = filteredData.map((row) => visibleColumns.map((c) => escapeCSV(row[c.name])).join(",")).join("\n");
    const bom = "\uFEFF";
    const csvContent = bom + headerRow + "\n" + dataRows;
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function buildOrUpdateColumnsVisibilityMenu() {
    const actionsRight = document.getElementById("actions-right");
    let menuWrap = document.getElementById("columns-menu");
    if (!menuWrap) {
        menuWrap = document.createElement("div");
        menuWrap.id = "columns-menu";
        menuWrap.className = "columns-menu";
        const btn = document.createElement("button");
        btn.type = "button";
        btn.className = "button columns-button";
        btn.textContent = "Sütunlar";
        btn.addEventListener("click", () => {
            dropdown.classList.toggle("open");
        });
        const dropdown = document.createElement("div");
        dropdown.className = "columns-dropdown";
        document.addEventListener("click", (e) => {
            if (!menuWrap.contains(e.target)) dropdown.classList.remove("open");
        });
        menuWrap.appendChild(btn);
        menuWrap.appendChild(dropdown);
        actionsRight.prepend(menuWrap);
    }

    const dropdown = menuWrap.querySelector(".columns-dropdown");
    dropdown.innerHTML = "";
    columns.forEach((col, idx) => {
        const row = document.createElement("div");
        row.className = "row";
        const cb = document.createElement("input");
        cb.type = "checkbox";
        cb.id = `col-vis-${idx}`;
        cb.checked = !col.dontShow;
        const label = document.createElement("label");
        label.setAttribute("for", cb.id);
        label.textContent = col.label || col.name;
        cb.addEventListener("change", () => {
            col.dontShow = !cb.checked;
            createTableColumns();
            renderTablePage(currentPage, filteredData);
            makeResizable();
        });
        row.appendChild(cb);
        row.appendChild(label);
        dropdown.appendChild(row);
    });
}
