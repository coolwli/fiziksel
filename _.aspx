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
