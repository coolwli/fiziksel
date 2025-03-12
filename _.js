
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

    const searchInput = document.querySelector('.table-top input[type="text"]');
    const searchTerm = searchInput ? searchInput.value.toLowerCase() : '';

    const searchColumnName = searchInput ? columns[columns.findIndex(column => column.hasSearchBar)].name : null;

    filteredData = baseData.filter(row => {
        const matchesSearchTerm = searchInput ? row[searchColumnName].toLowerCase().includes(searchTerm):true;

        const matchesFilters = selectedFilters.every((values, columnIndex) => {
            if (values.length === 0) return true;
            const columnName = columns[columnIndex].name;
            return values.includes(row[columnName]);
        });
        return matchesSearchTerm && matchesFilters;
    });

    const selectedHalls = Array.from(document.querySelectorAll('#hall-buttons .active')).map(button => button.innerText.split(' ')[1]);
    if (selectedHalls.toString() != "Halls") {
        filteredData = filteredData.filter(row => {
            return selectedHalls.includes(row[1]);
        });
    }
    columns.forEach((column, index) => {
        if (column.hasSearchBar || column.onlyTH) return;

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
