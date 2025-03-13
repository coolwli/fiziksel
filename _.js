
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

    const selectedHalls = Array.from(document.querySelectorAll('#hall-buttons .active'))
        .map(button => button.innerText.split(' ')[1]);
    
    if (selectedHalls.toString() != "Halls" && selectedHalls.length != Array.from(document.querySelectorAll('#hall-buttons .active')).length) {
        filteredData = baseData.filter(row => {
            for (let columnIndex = 0; columnIndex < selectedFilters.length; columnIndex++) {
                const values = selectedFilters[columnIndex];
                if (values.length === 0) continue;  

                const columnName = columns[columnIndex].name;
                if (!values.includes(row[columnName])) {
                    return false; 
                }
            }
            if (!selectedHalls.includes(row[1])) {
                return false; 
            }
            return true; 
        });
    }
    else{
        filteredData = baseData.filter(row => {
            for (let columnIndex = 0; columnIndex < selectedFilters.length; columnIndex++) {
                const values = selectedFilters[columnIndex];
                if (values.length === 0) continue; 

                const columnName = columns[columnIndex].name;
                if (!values.includes(row[columnName])) {
                    return false; 
                }
            }
            return true; 
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
