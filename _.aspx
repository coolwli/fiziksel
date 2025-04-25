<!DOCTYPE html>
<html lang="tr">
  <head>
    <meta charset="UTF-8" />
    <title>Dinamik Hiyerarşik Tablo</title>
    <style>
      body {
        font-family: "Segoe UI", sans-serif;
        background-color: #f8f9fa;
        margin: 40px;
        color: #333;
      }

      h1 {
        text-align: center;
        margin-bottom: 20px;
      }

      table {
        width: 100%;
        border-collapse: collapse;
        background-color: #fff;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        border-radius: 8px;
        overflow: hidden;
      }

      thead {
        background-color: #343a40;
        color: white;
      }

      th,
      td {
        padding: 12px 16px;
        text-align: left;
      }

      tbody tr {
        cursor: pointer;
        transition: background-color 0.2s ease;
      }

      .details-panel {
        margin-top: 30px;
        padding: 16px;
        border-radius: 8px;
        background-color: #ffffff;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        display: block;
      }

      .details-title {
        font-weight: bold;
        font-size: 18px;
        margin-bottom: 8px;
        color: #007b8a;
      }

      .details-content {
        font-size: 16px;
      }

      .indent-0 {
        background-color: #e9ecef;
      }
      .indent-1 {
        background-color: #f8f9fa;
      }
      .indent-2 {
        background-color: #e9ecef;
      }
      tbody tr:hover {
        background-color: #ccd6f0;
      }
      .active {
        background-color: #b3c7f9 !important;
        font-weight: bold;
      }

      .indent-0 td:first-child {
        padding-left: 10px;
      }
      .indent-1 td:first-child {
        padding-left: 40px;
      }
      .indent-2 td:first-child {
        padding-left: 100px;
      }
    </style>
  </head>
  <body>
    <h1>Dinamik Hiyerarşik Tablo</h1>

    <table id="hierarchy-table">
      <thead>
        <tr id="table-header"></tr>
      </thead>
      <tbody></tbody>
    </table>

    <div id="details" class="details-panel">
      <div class="details-title" id="details-title">Detay</div>
      <div class="details-content" id="details-content">
        Tıklanan satıra ait açıklama burada görünecek.
      </div>
    </div>

    <script>
      const tableHeadRow = document.getElementById("table-header");
      const tableBody = document.querySelector("#hierarchy-table tbody");
      const detailTitle = document.getElementById("details-title");
      const detailContent = document.getElementById("details-content");

      const tableData = [
        {
          name: "Toplam",
          amount: 100,
          type: "Genel",
          children: [
            {
              name: "A",
              amount: 30,
              type: "Kategori",
              children: [
                { name: "A1", amount: 15, type: "Alt Kategori" },
                { name: "A2", amount: 2, type: "Alt Kategori" },
                { name: "A3", amount: 13, type: "Alt Kategori" },
              ],
            },
            {
              name: "B",
              amount: 70,
              type: "Kategori",
              children: [
                { name: "B1", amount: 40, type: "Alt Kategori" },
                { name: "B2", amount: 30, type: "Alt Kategori" },
              ],
            },
          ],
        },
      ];

      function extractHeaders(obj, headerSet = new Set()) {
        Object.keys(obj).forEach((key) => {
          if (key !== "children") headerSet.add(key);
          if (key === "children" && Array.isArray(obj.children)) {
            obj.children.forEach((child) => extractHeaders(child, headerSet));
          }
        });
        return Array.from(headerSet);
      }

      const columnHeaders = extractHeaders(tableData[0]);
      columnHeaders.forEach((key) => {
        const th = document.createElement("th");
        th.textContent = key.charAt(0).toUpperCase() + key.slice(1);
        tableHeadRow.appendChild(th);
      });

      function createTableRow(item, depth = 0) {
        const row = document.createElement("tr");
        row.classList.add(`indent-${depth}`);

        columnHeaders.forEach((key) => {
          const cell = document.createElement("td");
          cell.textContent = item[key] !== undefined ? item[key] : "";
          row.appendChild(cell);
        });

        row.addEventListener("click", () => {
          document
            .querySelectorAll("tbody tr")
            .forEach((r) => r.classList.remove("active"));
          row.classList.add("active");

          detailTitle.textContent = `Detay: ${item.name || "Bilinmiyor"}`;
          detailContent.textContent = columnHeaders
            .map((key) => `${key}: ${item[key] ?? "-"}`)
            .join("\n");
        });

        tableBody.appendChild(row);

        if (Array.isArray(item.children)) {
          item.children.forEach((child) => createTableRow(child, depth + 1));
        }
      }

      tableData.forEach((item) => createTableRow(item));

      if (tableBody.rows.length > 0) {
        tableBody.querySelector("tr").click();
      }
    </script>
  </body>
</html>
