<!DOCTYPE html>
<html lang="tr">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <title>Basit Test Sayfası</title>
        <style>
            * {
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }

            body {
                font-family: Arial, sans-serif;
                background: #f5f5f5;
            }

            .table-container {
                max-width: 1000px;
                margin: 20px auto;
                background: white;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                padding: 20px;
            }
            .table-top {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 10px 0;
                border-bottom: 1px solid #ddd;
            }
            button {
                background: #3498db;
                color: white;
                border: none;
                padding: 10px 15px;
                border-radius: 5px;
                cursor: pointer;
                margin-right: 10px;
            }

            button:hover {
                background: #2980b9;
            }

            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 10px;
            }

            th,
            td {
                padding: 12px;
                text-align: left;
                border-bottom: 1px solid #ddd;
                position: relative;
            }

            th {
                background: #f8f9fa;
                font-weight: bold;
                cursor: pointer;
            }

            th:hover {
                background: #e9ecef;
            }

            tr:hover {
                background: #f5f5f5;
            }

            th:hover .resize-handle {
                display: block;
                background: #ccc; /* isteğe bağlı: görünürlüğü artırmak için */
            }

            .resize-handle {
                display: none;
                position: absolute;
                right: 0;
                top: 50%;
                transform: translateY(-50%);
                height: 60%;
                width: 3px;
                background-color: rgba(0, 0, 0, 0.15);
                border-radius: 2px;
                cursor: col-resize;
                transition: background-color 0.3s ease;
            }

            .dropdown-content {
                position: absolute;
                top: 100%;
                left: 0;
                background: white;
                border: 1px solid #ddd;
                border-radius: 5px;
                padding: 10px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                display: none;
                z-index: 100;
                min-width: 200px;
            }

            th:hover .dropdown-content {
                display: block;
            }

            .dropdown-content input {
                width: 100%;
                padding: 5px;
                border: 1px solid #ddd;
                border-radius: 3px;
                margin-bottom: 10px;
            }

            /* Column visibility menu */
            .columns-menu {
                position: relative;
            }
            .columns-button {
                background: #6c5ce7;
                color: #fff;
            }
            .columns-dropdown {
                position: absolute;
                right: 0;
                top: calc(100% + 6px);
                min-width: 220px;
                background: #fff;
                border: 1px solid #ddd;
                border-radius: 6px;
                box-shadow: 0 6px 24px rgba(0, 0, 0, 0.12);
                padding: 10px;
                display: none;
                z-index: 1000;
            }
            .columns-dropdown.open {
                display: block;
            }
            .columns-dropdown .row {
                display: flex;
                align-items: center;
                gap: 8px;
                padding: 4px 0;
            }
            .columns-dropdown .actions {
                display: flex;
                justify-content: space-between;
                gap: 8px;
                margin-bottom: 8px;
            }
        </style>
    </head>
    <body>
        <div id="table-top" class="table-top">
            <p id="rowCounter" class="rowCounter">Toplam 0 satır</p>
            <div class="button-container">
                <button class="button" onclick="setRowsPerPage(this)">10</button>
                <button class="button active" onclick="setRowsPerPage(this)">20</button>
                <button class="button" onclick="setRowsPerPage(this)">50</button>
            </div>
            <div id="actions-right" style="display: flex; gap: 8px; align-items: center">
                <!-- Column visibility dropdown will be injected here -->
                <button id="reset-button" title="Tüm filtreleri temizle">Sıfırla</button>
                <button id="export-button" title="Filtrelenmiş veriyi dışa aktar">CSV Dışa Aktar</button>
            </div>
        </div>
        <div id="table-container">
            <table>
                <thead id="table-head"></thead>
                <tbody id="tableBody"></tbody>
            </table>
        </div>
        <div id="pagination" class="pagination"></div>
        <script src="temp.js"></script>
        <script>
            const data = [
                { id: "1", name: "Ahmet", surname: "Yılmaz", value: "30" },
                { id: "2", name: "Mehmet", surname: "Demir", value: "25" },
                { id: "3", name: "Ayşe", surname: "Kara", value: "28" },
            ];

            const columns = [
                { name: "id", label: "ID", onlyTH: true },
                { name: "name", label: "Name", hasSearchBar: true },
                { name: "surname", label: "Surname" },
                { name: "value", label: "Value" },
            ];
            const sortableColumnTypes = {
                numericColumns: [0, 3],
                dateColumns: [],
                textwNumColumns: [],
            };
            initializeTable(data);
        </script>
    </body>
</html>
