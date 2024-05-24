table {
    border-collapse: collapse;
    width: 97%;
    margin: auto;
    border-radius: 10px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    font-size: 9px;
}

th,
td {
    border: none;
    padding: 8px;
    text-align: left;
}

th {
    background-color: #f2f2f2;
    color: #333;
    font-weight: bold;
    cursor: pointer;
    width: auto
}

tr:nth-child(even) {
    background-color: #f9f9f9;
}

tr:hover {
    background-color: #efefef;
    cursor: pointer;
}

.dropdown-content {
    display: none;
    position: absolute;
    background-color: #f9f9f9;
    min-width: 160px;
    box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
    padding: 12px 16px;
    z-index: 1;
    max-height: 200px;
    overflow-y: auto;
    text-align: left;
}

.dropdown:hover .dropdown-content {
    display: block;
}

.dropdown-arrow {
    margin-left: 5px;
    font-size: 8px;
}

.table-top {
    width: 97%;
    display: flex;
    justify-content: space-between;
    margin: 8px auto 4px auto
}

button {
    color: white;
    border: none;
    cursor: pointer;
    font-size: 12px;
    transition: background-color 0.3s ease;
    outline: none;
}

#reset-button {
    background-color: black;
    padding: 5px 8px;
    margin-left: 10px;
}

    #reset-button:hover {
        background-color: #484848;
    }

#rowCounter {
    margin-right: auto;
    font-size: 10px;
}

.pagination {
    display: flex;
    justify-content: center;
    margin-top: 20px;
    margin-bottom: 20px;
}

.page-link {
    display: inline-block;
    padding: 10px;
    margin: 0 4px;
    text-decoration: none;
    cursor: pointer;
    color: #333;
    border: 1px solid #ddd;
    border-radius: 4px;
}

    .page-link.active {
        background-color: black;
        color: white;
    }
