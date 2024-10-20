<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="historicDatas._default" %>
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Fiziksel Historic Data</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns"></script>
    <style>
        .panel-container {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            justify-content: space-between;
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

        .table-container {
            width: 20%;
            padding-left: 20px;
        }

        canvas {
            width: 100%;
            height: 250px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
        }

        .table-container table {
            border-radius: 8px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        table th,
        table td {
            padding: 12px;
            border: none;
            text-align: center;
            transition: background-color 0.3s;
        }

        table th {
            background-color: #6c757d;
            color: white;
            font-weight: bold;
        }

        table tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        table tr:hover {
            background-color: #e9ecef;
        }

        .chart-container {
            margin-bottom: 20px;
        }

        .date-picker {
            margin: 10px;
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

        .date-picker button {
            padding: 8px 12px;
            border-radius: 4px;
            border: none;
            background-color: #007bff;
            color: white;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .date-picker button:hover {
            background-color: #0056b3;
        }
    </style>
</head>

<body>
    <div class="header">
        <div id="logo"></div>
        <div>
            <h1 class="baslik" id="baslik" runat="server">Fiziksel Historic</h1>
        </div>
    </div>
    <div class="date-picker">
        <label for="startDate">Start Date:</label>
        <input type="date" id="startDate">
        <label for="endDate">End Date:</label>
        <input type="date" id="endDate">
        <button id="updateButton">Update</button>
    </div>
    <div id="chart-tables" class="panel-container"></div>

    <script>
        let allDatasetGroups;
        let allAvailableDates;

        const generateDateList = (start, length) => {
            return Array.from({ length }, (_, i) => {
                const date = new Date(start);
                date.setDate(start.getDate() - i);
                return date.toISOString().split('T')[0];
            }).reverse();
        };

        const generateRandomColor = () => {
            const r = Math.floor(Math.random() * 255);
            const g = Math.floor(Math.random() * 255);
            const b = Math.floor(Math.random() * 255);
            return {
                borderColor: `rgba(${r}, ${g}, ${b}, 1)`,
                backgroundColor: `rgba(${r}, ${g}, ${b}, 0.2)`
            };
        };

        const createChartInstance = (ctx, datasets, dates, chartTitle) => {
            const chartData = {
                labels: dates,
                datasets: datasets.map(({ label, data }) => {
                    const colors = generateRandomColor();
                    return {
                        label,
                        data: data.map((y, i) => ({ x: dates[i], y })),
                        borderColor: colors.borderColor,
                        backgroundColor: colors.backgroundColor,
                        borderWidth: 2,
                        tension: 0.1,
                        pointRadius: 3,
                        fill: false
                    };
                })
            };

            const chartOptions = {
                responsive: true,
                plugins: {
                    title: { display: true, text: chartTitle },
                    tooltip: { mode: 'index', intersect: false }
                },
                scales: {
                    x: {
                        type: 'time',
                        time: { unit: 'day' },
                        title: { display: true, text: 'Date' }
                    },
                    y: {
                        title: { display: true, text: 'Value' }
                    }
                }
            };

            return new Chart(ctx, { type: 'line', data: chartData, options: chartOptions });
        };

        const createDatasetTable = (datasets) => {
            const rows = datasets.map(({ label, data }) => `
                <tr>
                    <td>${label}</td>
                    <td>${data[0]}</td>
                    <td>${data[data.length - 1]}</td>
                </tr>`).join('');

            return `
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>First Day</th>
                            <th>Last Day</th>
                        </tr>
                    </thead>
                    <tbody>${rows}</tbody>
                </table>`;
        };

        const initializeChartsAndTables = (datasetGroups, dates) => {
            const chartTablesContainer = document.getElementById('chart-tables');
            chartTablesContainer.innerHTML = '';

            datasetGroups.forEach((datasetGroup, index) => {
                const wrapper = document.createElement('div');
                wrapper.classList.add('chart-table-wrapper');

                const panel = document.createElement('div');
                panel.classList.add('panel');

                const chartContainer = document.createElement('div');
                chartContainer.classList.add('chart-container');
                const tableContainer = document.createElement('div');
                tableContainer.classList.add('table-container');

                chartContainer.innerHTML = `<canvas id="chart${index}"></canvas>`;
                tableContainer.innerHTML = createDatasetTable(datasetGroup.datasets);

                panel.appendChild(chartContainer);
                wrapper.appendChild(panel);
                wrapper.appendChild(tableContainer);
                chartTablesContainer.appendChild(wrapper);

                const ctx = document.getElementById(`chart${index}`).getContext('2d');
                createChartInstance(ctx, datasetGroup.datasets, dates, datasetGroup.title);
            });
        };

        const formatDate = (date) => {
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            return `${yyyy}-${mm}-${dd}`;
        };

        const fetchInitialData = () => {
            allDatasetGroups = Object.entries(datasets).map(([title, dataset]) => ({
                title,
                datasets: Object.entries(dataset).map(([label, data]) => ({ label, data }))
            }));

            allAvailableDates = dates.map(date => new Date(date));
            allDatasetGroups = allDatasetGroups.map(group => ({
                ...group,
                datasets: group.datasets.sort((a, b) => {
                    const sumA = a.data.reduce((acc, val) => acc + val, 0);
                    const sumB = b.data.reduce((acc, val) => acc + val, 0);
                    return sumB - sumA;
                })
            }));
            initializeChartsAndTables(allDatasetGroups, allAvailableDates);

            document.getElementById('startDate').value = formatDate(allAvailableDates[0]);
            document.getElementById('endDate').value = formatDate(allAvailableDates[allAvailableDates.length - 1]);
        };

        document.getElementById('updateButton').addEventListener('click', () => {
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

        fetchInitialData();
    </script>
</body>

</html>


</body>

</html>
