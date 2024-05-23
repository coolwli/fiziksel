function sortDatas(index) {
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

        if (!isNaN(aValue) && !isNaN(bValue)) {
            return ascending ? aValue - bValue : bValue - aValue;
        }

        return ascending ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
    });
    currentPage = 1;
    renderPage(currentPage);
}
