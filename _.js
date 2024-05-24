function sortDatas(index, ascending) {
    filteredData.sort(function (a, b) {
        let aValue = a[Object.keys(a)[index]];
        let bValue = b[Object.keys(b)[index]];

        // Check if values are dates (in MM/DD/YYYY format)
        const datePattern = /^\d{2}\/\d{2}\/\d{4}$/;

        if (datePattern.test(aValue) && datePattern.test(bValue)) {
            aValue = new Date(aValue);
            bValue = new Date(bValue);
            return ascending ? aValue - bValue : bValue - aValue;
        }

        // Check if values are numbers
        if (!isNaN(aValue) && !isNaN(bValue)) {
            aValue = parseFloat(aValue);
            bValue = parseFloat(bValue);
            return ascending ? aValue - bValue : bValue - aValue;
        }

        // Default to string comparison
        return ascending ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
    });
    currentPage = 1;
    renderPage(currentPage);
}
