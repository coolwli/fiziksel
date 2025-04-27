<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Historical Data</title>

    <style>
      #page-name {
        color: #eaeb00;
      }
      .chart-container {
        margin-bottom: 20px;
      }
      .table-container {
        width: 20%;
        padding-left: 20px;
        max-height: 570px;
        overflow: auto;
      }
      .date-picker {
        margin: 70px 10px 20px;
        display: flex;
        gap: 10px;
        padding-bottom: 10px;
        border-bottom: 1px solid #ced4da;
      }

      .date-picker label {
        font-weight: bold;
        align-self: center;
        color: #495057;
      }

      .date-picker input {
        padding: 8px;
        border-radius: 4px;
        border: 1px solid #ced4da;
        width: 150px;
        transition: border-color 0.3s;
        background-color: #ffffff;
      }

      .date-picker input:focus {
        border-color: #007bff;
        outline: none;
      }

      .date-picker button:hover {
        background-color: #0056b3;
      }
      canvas {
        width: 100%;
        height: 250px;
      }

      .container {
        display: flex;
      }

      .chart-container {
        background-color: #f2f2f2;
        width: 58%;
        margin: 0px auto;
        max-height: 500px;
      }

      .chart-table-wrapper {
        display: flex;
        width: 100%;
      }

      .panel {
        background-color: #ffffff;
        border-radius: 8px;
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        padding: 15px;
        width: 80%;
        min-width: 300px;
      }
      table {
        width: 40%;
        border-collapse: collapse;
        background-color: #fff;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        border-radius: 8px;
        overflow: hidden;
        font-size: 16px;
        margin-left: auto;
      }

      thead {
        background-color: #343a40;
        color: white;
      }

      th,
      td {
        padding: 12px 8px;
        text-align: left;
      }

      tbody tr {
        cursor: pointer;
        transition: background-color 0.2s ease;
      }

      .indent-0 {
        background-color: #f6f6f6;
      }

      .indent-1 {
        background-color: #f6f6f6;
      }

      .indent-2 {
        background-color: #eaeaea;
      }
      .indent-3 {
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
        padding-left: 10px;
      }

      .indent-2 td:first-child {
        padding-left: 40px;
      }
      .indent-3 td:first-child {
        padding-left: 100px;
      }
    </style>
  </head>
  <body>
    <form id="form1" runat="server">
      <div class="header">
        <a href="/">cloudunited</a>
        <span id="page-name">CPU Model Historic View</span>
      </div>
      <div class="date-picker">
        <label for="startDate">Start Date:</label>
        <input type="date" id="startDate" />
        <label for="endDate">End Date:</label>
        <input type="date" id="endDate" />
        <button id="updateButton">Update</button>
      </div>
      <div class="container">
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
        <div class="chart-container">
          <canvas id="model-chart"></canvas>
          <canvas id="core-chart"></canvas>
        </div>
      </div>

      <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
      <script src="https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns"></script>
      <script src="temp.js"></script>

      <script>
        const tbody = document.querySelector("#hierarchy-table tbody");

        const formatDate = (date) => {
          const yyyy = date.getFullYear();
          const mm = String(date.getMonth() + 1).padStart(2, "0");
          const dd = String(date.getDate()).padStart(2, "0");
          return `${yyyy}-${mm}-${dd}`;
        };

        function initialize(data) {
          const uniqueDates = [
            ...new Set(
              data.map((d) => {
                const date = new Date(d.tarih);
                return date.toDateString();
              })
            ),
          ]
            .map((d) => new Date(d))
            .sort((a, b) => b - a);

          document.getElementById("startDate").value = formatDate(
            uniqueDates.at(-1)
          );
          document.getElementById("endDate").value = formatDate(uniqueDates[0]);
          generateDate(data);
        }

        function buildTree(data) {
          let map = {};
          let roots = [];

          data.forEach((item) => {
            map[item.name] = { ...item, children: [] };
          });
          data.forEach((item) => {
            if (item.parent) {
              map[item.parent].children.push(map[item.name]);
            } else {
              roots.push(map[item.name]);
            }
          });

          return roots;
        }

        function generateDate(data) {
          if (data.length === 0) {
            alert("No data available for the selected date range.");
            return;
          }
          const latestDate = data.reduce((latest, item) => {
            const latestDate = new Date(latest);
            const currentDate = new Date(item.tarih);
            return currentDate.setHours(0, 0, 0, 0) >
              latestDate.setHours(0, 0, 0, 0)
              ? item.tarih
              : latest;
          }, data[0].tarih);

          const latestData = data.filter((item) => item.tarih === latestDate);
          const tabelData = buildTree(latestData);
          renderTable(tabelData[0]);
          document.querySelectorAll("tbody tr")[0].click();
        }

        function updateChart(clickedRow) {
          const clickedName = clickedRow.dataset.name;
          let startDate = new Date(document.getElementById("startDate").value);
          let endDate = new Date(document.getElementById("endDate").value);

          // Veriyi filtreleme
          const chartData = datasets
            .filter((item) => {
              let d = new Date(item.tarih);
              d = new Date(d.getFullYear(), d.getMonth(), d.getDate());
              startDate = new Date(
                startDate.getFullYear(),
                startDate.getMonth(),
                startDate.getDate()
              );
              endDate = new Date(
                endDate.getFullYear(),
                endDate.getMonth(),
                endDate.getDate()
              );
              return (
                item.name === clickedName && d >= startDate && d <= endDate
              );
            })
            .map((item) => ({
              intel_count: item.intel_count,
              amd_count: item.amd_count,
              cpu_core: item.cpu_core,
              tarih: new Date(item.tarih),
            }));

          // Intel ve AMD sayıları ve CPU Core için verileri al
          const intelCounts = chartData.map((item) => item.intel_count);
          const amdCounts = chartData.map((item) => item.amd_count);
          const labels = chartData.map((item) =>
            item.tarih.toLocaleDateString()
          ); // Date formatına çeviriyoruz
          const cpuCores = chartData.map((item) => item.cpu_core);

          // Intel ve AMD için Line Chart
          const ctx = document.getElementById("model-chart").getContext("2d");
          const coreCtx = document
            .getElementById("core-chart")
            .getContext("2d");

          // Eğer bir chart varsa, önceki chart'ı yok et
          if (window.intelAmdChart) {
            window.intelAmdChart.destroy();
          }

          // Intel ve AMD sayıları için line chart oluşturma
          window.intelAmdChart = new Chart(ctx, {
            type: "line", // Line chart tipi
            data: {
              labels: labels,
              datasets: [
                {
                  label: "Intel Count",
                  data: intelCounts,
                  borderColor: "rgba(54, 162, 235, 1)", // Intel için renk
                  backgroundColor: "rgba(54, 162, 235, 0.2)",
                  fill: false, // Çizgi grafiği, dolgu istemiyoruz
                  borderWidth: 2,
                  tension: 0.4, // Eğriliği artırmak için
                },
                {
                  label: "AMD Count",
                  data: amdCounts,
                  borderColor: "rgba(255, 99, 132, 1)", // AMD için renk
                  backgroundColor: "rgba(255, 99, 132, 0.2)",
                  fill: false, // Çizgi grafiği, dolgu istemiyoruz
                  borderWidth: 2,
                  tension: 0.4, // Eğriliği artırmak için
                },
              ],
            },
            options: {
              responsive: true,
              scales: {
                y: {
                  beginAtZero: true, // Y ekseni sıfırdan başlasın
                },
              },
            },
          });

          // CPU Core için Line Chart
          if (window.cpuCoreChart) {
            window.cpuCoreChart.destroy();
          }

          window.cpuCoreChart = new Chart(coreCtx, {
            type: "line", // Line chart tipi
            data: {
              labels: labels,
              datasets: [
                {
                  label: "CPU Core Count",
                  data: cpuCores,
                  borderColor: "rgba(75, 192, 192, 1)", // CPU Core için renk
                  backgroundColor: "rgba(75, 192, 192, 0.2)",
                  fill: false, // Çizgi grafiği, dolgu istemiyoruz
                  borderWidth: 2,
                  tension: 0.4, // Eğriliği artırmak için
                },
              ],
            },
            options: {
              responsive: true,
              scales: {
                y: {
                  beginAtZero: true, // Y ekseni sıfırdan başlasın
                },
              },
            },
          });
        }

        function renderTable(root) {
          tbody.innerHTML = "";

          function renderRow(item, level, parent) {
            const tr = document.createElement("tr");
            tr.classList.add("indent-" + level);
            tr.dataset.name = item.name;
            tr.dataset.parent = parent;
            tr.dataset.level = level;
            tr.style.display = level > 1 ? "none" : "table-row";

            const tdName = document.createElement("td");
            tdName.textContent = item.name;
            tr.appendChild(tdName);

            const tdCpuCore = document.createElement("td");
            tdCpuCore.textContent = item.cpu_core;
            tr.appendChild(tdCpuCore);

            const tdIntelCount = document.createElement("td");
            tdIntelCount.textContent = item.intel_count;
            tr.appendChild(tdIntelCount);

            const tdAmdCount = document.createElement("td");
            tdAmdCount.textContent = item.amd_count;
            tr.appendChild(tdAmdCount);
            tr.addEventListener("click", () => {
              handleRowClick(tr);
            });

            tbody.appendChild(tr);

            if (item.children.length > 0) {
              item.children.forEach((child) =>
                renderRow(child, level + 1, item.name)
              );
            }
          }

          renderRow(root, 0, null);
        }

        function handleRowClick(clickedRow) {
          tbody.querySelectorAll("tr").forEach((row) => {
            const isChildOfClicked =
              row.dataset.parent === clickedRow.dataset.name;
            const isParent = row.dataset.name === clickedRow.dataset.parent;
            const isHaveSameParent =
              row.dataset.parent === clickedRow.dataset.parent;
            row.classList.remove("active");
            row.style.display =
              isChildOfClicked ||
              isHaveSameParent ||
              row.dataset.level < 2 ||
              isParent
                ? "table-row"
                : "none";
          });

          clickedRow.classList.add("active");
          updateChart(clickedRow);
        }

        document
          .getElementById("updateButton")
          .addEventListener("click", (event) => {
            event.preventDefault();
            let startDate = new Date(
              document.getElementById("startDate").value
            );
            let endDate = new Date(document.getElementById("endDate").value);

            if (startDate && endDate && startDate <= endDate) {
              const filtered = datasets.filter((item) => {
                let d = new Date(item.tarih);
                d = new Date(d.getFullYear(), d.getMonth(), d.getDate());
                startDate = new Date(
                  startDate.getFullYear(),
                  startDate.getMonth(),
                  startDate.getDate()
                );
                endDate = new Date(
                  endDate.getFullYear(),
                  endDate.getMonth(),
                  endDate.getDate()
                );

                return d >= startDate && d <= endDate;
              });
              generateDate(filtered);
            } else {
              alert("Please select a valid date range.");
            }
          });
        initialize(datasets);
      </script>
    </form>
  </body>
</html>
