const values = [...new Set(filteredData.map(row => row[Object.keys(row)[columnIndex]].trim()))];
