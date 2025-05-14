<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CPUModelHistoric._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Historical Data</title>

    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/table-style.css" />

    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
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
        #page-selector{
            margin: 40px auto 20px auto;
        }

        #page-selector a{
            cursor:pointer;
        }

        canvas {
            width: 100%;
            height: 250px;
        }

        .container{
            display:flex;
        }

        .chart-container {
            width: 58%;     
            margin: 0px auto;
            display:block;
            max-height: 600px;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }
        .all{
            width: 100%;     
            flex-direction: row;
            margin: 20px auto;
            max-height:500px;
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
            margin:auto;
        }
        table {
            width: 40%;
            border-collapse: collapse;
            background-color: #fff;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            border-radius: 8px;
            overflow: hidden;
            font-size:16px;
            margin-left:auto;
            height: fit-content;
            display:none;
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
        <div class="date-picker" >
            <label for="startDate">Start Date:</label>
            <input type="date" id="startDate">
            <label for="endDate">End Date:</label>
            <input type="date" id="endDate">
            <button id="updateButton">Update</button>
        </div>
        <div id="page-selector">
            <a class="active" onclick="setTable(this)">All</a>
            <a onclick="setTable(this)">Pendik</a>
            <a onclick="setTable(this)">Ankara</a>
        </div>

        <div class="container">
            <table id="hierarchy-table">
              <thead>
                <tr>
                  <th style="width: 40%">Ad</th>
                  <th style="width: 12%">Toplam Core</th>
                  <th style="width: 12%">Intel CPU Count</th>
                  <th style="width: 12%">AMD CPU Count</th>
                  <th style="width: 12%">Intel CPU Core</th>
                  <th style="width: 12%">AMD CPU Core</th>
                </tr>
              </thead>
              <tbody></tbody>
            </table>
            <div class="chart-container all" id="chart-container">
              <canvas class="panel" id="model-chart"></canvas>
              <canvas class="panel" id="core-chart"></canvas>
            </div>
        </div>


        <script>
            const tbody = document.querySelector("#hierarchy-table tbody");

            
            function setTable(nameelement) {
                names= document.querySelectorAll("#page-selector a");
                names.forEach((name) => {
                    name.classList.remove("active");
                });
                nameelement.classList.add("active");

                let name = nameelement.innerText;
                if (name == "All") {
                    document.getElementById("chart-container").classList.add("all");
                    document.getElementById("hierarchy-table").style.display = "none";

                }
                else if(name == "Ankara"){
                    document.getElementById("chart-container").classList.remove("all");
                    document.getElementById("hierarchy-table").style.display = "block";

                }
                else{
                    document.getElementById("chart-container").classList.remove("all");
                    document.getElementById("hierarchy-table").style.display = "block";

                }
            }
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
                    intel_core: item.intel_core,
                    amd_core: item.amd_core,
                    tarih: new Date(item.tarih),
                    }))
                .sort((a,b) => a.tarih - b.tarih);
                // Intel ve AMD sayıları ve CPU Core için verileri al
                const intelCounts = chartData.map((item) => item.intel_count);
                const amdCounts = chartData.map((item) => item.amd_count);
                const labels = chartData.map((item) =>
                item.tarih.toLocaleDateString()
                ); // Date formatına çeviriyoruz
                const intelCores = chartData.map((item) => item.intel_core);
                const amdCores = chartData.map((item) => item.amd_core);

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
                    plugins: {
                        title: {
                            display: true, text: 'CPU Model Counts'},
                            tooltip: {
                            mode: 'index', intersect: false
                        }
                    },
                    scales: {
                        x: {
                            time: { unit: 'day' },
                            title: { display: true, text: 'Date' }
                        },
                        y: {
                            beginAtZero: true,
                            title: { display: true, text: 'Value' }
                        }
                    }
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
                        label: "Intel Core(s)",
                        data: intelCores,
                        borderColor: "rgba(110, 0, 255, 1)", 
                        backgroundColor: "rgba(75, 192, 192, 0.2)",
                        fill: false,
                        borderWidth: 2,
                        tension: 0.4, 
                    },
                    {
                        label: "AMD Core(s)",
                        data: amdCores,
                        borderColor: "rgba(40, 167, 69, 1)", // AMD için renk
                        backgroundColor: "rgba(40, 167, 69, 0.2)",
                        fill: false, // Çizgi grafiği, dolgu istemiyoruz
                        borderWidth: 2,
                        tension: 0.4, // Eğriliği artırmak için
                    },
                    ],
                },
                options: {
                    responsive: true,
                    plugins: {
                        title: { display: true, text: 'CPU Core Size'},
                        tooltip: { mode: 'index', intersect: false}
                    },
                    scales: {
                        x: {
                            time: { unit: 'day' },
                            title: { display: true, text: 'Date' }
                        },
                        y: {
                        beginAtZero: true, 
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
                tdCpuCore.textContent = item.amd_core+item.intel_core;
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

                const intelCore = document.createElement("td");
                intelCore.textContent = item.intel_core;
                  tr.appendChild(intelCore);
                const amdCore = document.createElement("td");
                amdCore.textContent = item.amd_core;
                  tr.appendChild(amdCore);

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
                window.scrollTo({top:0,behavior:'smooth'});
            }

            document.getElementById("updateButton").addEventListener("click", (event) => {
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
        </script>
    </form>
</body>
</html>
