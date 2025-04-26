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
        canvas {
            width: 100%;
            height: 250px;
        }

        .container{
            display:flex;
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
            font-size:16px;
            margin-left:auto;
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
    <form id="form1" runat="server">
        <div class="header">
            <a href="/">cloudunited</a>
            <span id="page-name">CPU Model Historic View</span>
        </div>
        <div class="date-picker">
            <label for="startDate">Start Date:</label>
            <input type="date" id="startDate">
            <label for="endDate">End Date:</label>
            <input type="date" id="endDate">
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
            <div id="chart" class="chart-container"></div>
        </div>


        <script>
            const tbody = document.querySelector("#hierarchy-table tbody");
            let tableData = [];

            function initialize() {
                tableData = buildHierarchy(getLatestData(datasets));
                renderTable(tableData)


                document.getElementById('startDate').value = formatDate(allDates[0]);
                document.getElementById('endDate').value = formatDate(allDates[allAvailableDates.length - 1]);
            }
            const formatDate = (date) => {
                const yyyy = date.getFullYear();
                const mm = String(date.getMonth() + 1).padStart(2, '0');
                const dd = String(date.getDate()).padStart(2, '0');
                return `${yyyy}-${mm}-${dd}`;
            };
            function getLatestData(data){
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

            function buildHierarchy(data){
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

            document.getElementById('updateButton').addEventListener('click', () => {
                event.preventDefault();
                const startDate = new Date(document.getElementById('startDate').value);
                const endDate = new Date(document.getElementById('endDate').value);

                if (startDate && endDate && startDate <= endDate) {
                    const filteredDates = allAvailableDates.filter(date => {
                        const currentDate = new Date(date);
                        return currentDate >= startDate && currentDate <= endDate;
                    });

                    const filteredDatasetGroups = allDatasetGroups.map(group => ({
                        title: group.title,
                        datasets: group.datasets.map(dataset => {
                            const startIndex = allAvailableDates.indexOf(filteredDates[0]);
                            const endIndex = allAvailableDates.indexOf(filteredDates[filteredDates.length - 1]);
                            return {
                                label: dataset.label,
                                data: dataset.data.slice(startIndex, endIndex + 1)
                            };
                        })
                    }));

                    initializeChartsAndTables(filteredDatasetGroups, filteredDates);
                } else {
                    alert("Please select a valid date range.");
                }
            });
        </script>
    </form>
</body>
</html>
