<!DOCTYPE html>
<html lang="tr">
  <head>
    <meta charset="UTF-8" />
    <title>Dinamik Hiyerarşik Tablo</title>
    <style>
      body {
        font-family: "Segoe UI", sans-serif;
        background-color: #f4f6f8;
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
        background-color: #f6f6f6;
      }

      .indent-1 {
        background-color: #eaeaea;
      }

      .indent-2 {
        background-color: #dedede;
      }

      tbody tr:hover {
        background-color: #cfcfcf;
      }

      tbody tr.active {
        background-color: #bdbdbd !important;
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
        <tr>
          <th style="width: 55%">Ad</th>
          <th style="width: 15%">Intel CPU</th>
          <th style="width: 15%">AMD CPU</th>
          <th style="width: 15%">Toplam Core</th>
        </tr>
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
      const baseData = [];

      const getLatestData = (data) => {
        const map = {};
        data.forEach((item) => {
          if (
            !map[item.name] ||
            new Date(item.tarih) > new Date(map[item.name].tarih)
          ) {
            map[item.name] = item;
          }
        });
        return Object.values(map);
      };

      const buildHierarchy = (data) => {
        const result = [];
        const total = { intel: 0, amd: 0, core: 0 };

        const grouped = {};
        data.forEach(
          ({ name, vcenter, location, intel_count, amd_count, cpu_core }) => {
            grouped[location] = grouped[location] || {};
            grouped[location][vcenter] = grouped[location][vcenter] || [];
            grouped[location][vcenter].push({
              name,
              intel: intel_count,
              amd: amd_count,
              core: cpu_core,
            });
          }
        );

        for (const [location, vcenters] of Object.entries(grouped)) {
          let locIntel = 0,
            locAmd = 0,
            locCore = 0;

          for (const [vcenter, servers] of Object.entries(vcenters)) {
            let vcIntel = 0,
              vcAmd = 0,
              vcCore = 0;

            servers.forEach(({ name, intel, amd, core }) => {
              result.push({
                name,
                intel,
                amd,
                core,
                level: 2,
                parent: vcenter,
              });
              vcIntel += intel;
              vcAmd += amd;
              vcCore += core;
            });

            result.push({
              name: vcenter,
              intel: vcIntel,
              amd: vcAmd,
              core: vcCore,
              level: 1,
              parent: location,
            });
            locIntel += vcIntel;
            locAmd += vcAmd;
            locCore += vcCore;
          }

          result.push({
            name: location,
            intel: locIntel,
            amd: locAmd,
            core: locCore,
            level: 0,
            parent: "TÜMÜ",
          });
          total.intel += locIntel;
          total.amd += locAmd;
          total.core += locCore;
        }

        result.push({ name: "TÜMÜ", ...total, level: 0, parent: null });
        return result.reverse();
      };

      const tbody = document.querySelector("#hierarchy-table tbody");
      const detailsTitle = document.getElementById("details-title");
      const detailsContent = document.getElementById("details-content");

      function renderTable(rows) {
        tbody.innerHTML = "";

        rows.forEach((row) => {
          const tr = document.createElement("tr");
          tr.className = `indent-${row.level}`;
          tr.dataset.level = row.level;
          tr.dataset.parent = row.parent;
          tr.dataset.name = row.name;
          tr.style.display = row.level <= 1 ? "table-row" : "none";

          tr.innerHTML = `
          <td>${row.name}</td>
          <td>${row.intel}</td>
          <td>${row.amd}</td>
          <td>${row.core}</td>
        `;

          tr.addEventListener("click", () => handleRowClick(tr, rows));
          tbody.appendChild(tr);
        });
      }

      function handleRowClick(clickedRow, rows) {
        const clickedName = clickedRow.dataset.name;

        tbody.querySelectorAll("tr").forEach((row) => {
          const isChildOfClicked = row.dataset.parent === clickedName;
          const isSameLevel = row.dataset.parent === clickedRow.dataset.parent;

          row.classList.remove("active");
          row.style.display =
            isChildOfClicked || isSameLevel || row.dataset.level < 2
              ? "table-row"
              : "none";
        });

        clickedRow.classList.add("active");
        updateDetails(clickedRow);
      }

      const uniqueData = getLatestData(baseData);
      const tableData = buildHierarchy(uniqueData);

      renderTable(tableData);
      document.querySelector("tbody tr").click();

      //when a row is clicked
      function updateDetails(row) {
        const name = row.cells[0].innerText;
        const intel = row.cells[1].innerText;
        const amd = row.cells[2].innerText;
        const core = row.cells[3].innerText;

        detailsTitle.textContent = `${name} Detayları`;
        detailsContent.innerHTML = `
              <ul>
                <li><strong>Intel CPU:</strong> ${intel}</li>
                <li><strong>AMD CPU:</strong> ${amd}</li>
                <li><strong>Toplam Core:</strong> ${core}</li>
              </ul>
            `;
      }
    </script>
  </body>
</html>
