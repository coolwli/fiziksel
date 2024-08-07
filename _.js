<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <title>Cloud United</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns"></script>
    <style>
        body {
            font-family: Arial, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #f5f5f5;
        }

        .chart-container {
            width: 80%;
            height: 80%;
        }

        .charts {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        canvas {
            width: 100%;
            height: 100%;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: #ffffff;
        }

        .disk-panel {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .disk-canvas {
            width: 85%;
            height: 85%;
        }

        .dropdown-container {
            margin-bottom: 20px;
        }

        label {
            margin-right: 10px;
        }

        .panel-container {
            display: flex;
            flex-direction: column;
            gap: 10px;
            width: 250px;
        }

        .panel-group {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .panel {
            background-color: #eeeeee;
            padding: 10px;
            border-radius: 5px;
            cursor: pointer;
            text-align: center;
            border: 1px solid transparent;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .panel.active {
            background-color: #cccccc;
            font-weight: bold;
            border: 1px solid #aaa;
        }

        .panel-value {
            font-weight: bold;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server" style="width: 100%; height: 100%;">
        <p runat="server" id="Label"></p>
        <div class="dropdown-container">
            <label for="timeRange">Select Time Range: </label>
            <select id="timeRange" onchange="fetchData()">
                <option value="1">1 Day</option>
                <option value="7">7 Days</option>
                <option value="30" selected>30 Days</option>
                <option value="90">90 Days</option>
                <option value="360">1 Year</option>
            </select>
        </div>
        <div class="charts">
            <div>
                <canvas id="cpuChart"></canvas>
            </div>
            <div>
                <canvas id="memoryChart"></canvas>
            </div>
            <div class="disk-panel">
                <div class="panel-group" id="diskPanelContainer">
                    <!-- Disk panels will be dynamically inserted here -->
                </div>
                <div class="disk-canvas">
                    <canvas id="diskChart"></canvas>
                </div>
            </div>
        </div>

        <script>
            // Sample data
            const sampleDates = ["2024-07-08T00:00:00", "2024-07-09T00:00:00"];
            const sampleCpuData = [
                { x: "2024-07-08T00:00:00", y: 20 },
                { x: "2024-07-09T00:00:00", y: 30 }
            ];
            const sampleMemoryData = [
                { x: "2024-07-08T00:00:00", y: 40 },
                { x: "2024-07-09T00:00:00", y: 60 }
            ];
            const sampleDiskDataMap = {
                'disk1': [
                    { x: "2024-07-08T00:00:00", y: 50 },
                    { x: "2024-07-09T00:00:00", y: 70 }
                ],
                'disk2': [
                    { x: "2024-07-08T00:00:00", y: 60 },
                    { x: "2024-07-09T00:00:00", y: 80 }
                ]
            };

            const createChart = (ctx, data, dates, label, borderColor) => {
                const min = Math.min(...data.map(d => d.y));
                const max = Math.max(...data.map(d => d.y));
                const minIndex = data.findIndex(d => d.y === min);
                const maxIndex = data.findIndex(d => d.y === max);

                const backgroundColor = borderColor.replace('1)', '0.2)');

                return new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: dates,
                        datasets: [
                            {
                                label: `Minimum: ${min.toFixed(2)}%`,
                                data: [{ x: dates[minIndex], y: min }],
                                borderColor: 'red',
                                fill: true,
                                pointRadius: 6,
                                pointHoverRadius: 4,
                                pointBackgroundColor: 'red'
                            },
                            {
                                label: `Maximum: ${max.toFixed(2)}%`,
                                data: [{ x: dates[maxIndex], y: max }],
                                borderColor: 'black',
                                fill: true,
                                pointRadius: 6,
                                pointHoverRadius: 4,
                                pointBackgroundColor: 'black'
                            },
                            {
                                label: label,
                                data: data,
                                borderColor: borderColor,
                                backgroundColor: backgroundColor,
                                fill: true,
                                pointRadius: 0,
                                pointHoverRadius: 4,
                                borderWidth: 1,
                                tension: 0.1
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            title: { display: true, text: `${label} Usage` },
                            legend: { position: 'top' },
                            tooltip: {
                                backgroundColor: '#333',
                                titleColor: '#fff',
                                bodyColor: '#fff'
                            }
                        },
                        interaction: { intersect: false },
                        scales: {
                            x: {
                                type: 'time',
                                time: { unit: 'day' },
                                display: true,
                                title: { display: true, text: 'Date' },
                                grid: { color: '#e0e0e0' }
                            },
                            y: {
                                display: true,
                                title: { display: true, text: 'Value' },
                                suggestedMin: Math.max(0, min - 10),
                                suggestedMax: max + 10,
                                grid: { color: '#e0e0e0' }
                            }
                        }
                    }
                });
            };

            const processLargeData = (data, dates) => {
                if (data.length <= 250) {
                    return {
                        data: data.map((value, index) => ({ x: dates[index], y: value })),
                        dates
                    };
                }

                const step = Math.ceil(data.length / 250);
                let sum = 0;
                let count = 0;
                let sumDates = 0;
                const reducedData = [];
                const reducedDates = [];

                data.forEach((value, i) => {
                    sum += value;
                    sumDates += new Date(dates[i]).getTime();
                    count++;

                    if ((i + 1) % step === 0) {
                        const average = sum / count;
                        const averageDate = new Date(sumDates / count);
                        reducedData.push({ x: averageDate, y: average });
                        reducedDates.push(averageDate);
                        sum = 0;
                        sumDates = 0;
                        count = 0;
                    }
                });

                if (count > 0) {
                    const average = sum / count;
                    const averageDate = new Date(sumDates / count);
                    reducedData.push({ x: averageDate, y: average });
                    reducedDates.push(averageDate);
                }

                const min = Math.min(...data);
                const max = Math.max(...data);
                const minDate = new Date(dates[data.indexOf(min)]);
                const maxDate = new Date(dates[data.indexOf(max)]);

                if (!reducedData.some(d => d.x.getTime() === minDate.getTime())) {
                    reducedData.push({ x: minDate, y: min });
                    reducedDates.push(minDate);
                }
                if (!reducedData.some(d => d.x.getTime() === maxDate.getTime())) {
                    reducedData.push({ x: maxDate, y: max });
                    reducedDates.push(maxDate);
                }

                reducedData.sort((a, b) => a.x - b.x);
                reducedDates.sort((a, b) => a - b);

                return { dates: reducedDates, data: reducedData };
            };

            const updateDiskPanels = (diskLabels) => {
                const container = document.getElementById('diskPanelContainer');
                container.innerHTML = '';

                diskLabels.forEach(label => {
                    const averageUsage = calculateAverage(diskDataMap[label]);
                    const panel = document.createElement('div');
                    panel.className = 'panel';
                    panel.dataset.disk = label;

                    panel.innerHTML = `
                        Disk ${label} <span class="panel-value">${averageUsage}%</span>
                    `;

                    panel.addEventListener('click', () => handlePanelClick(label));
                    container.appendChild(panel);
                });
            };

            const fetchData = async () => {
                const range = parseInt(document.getElementById('timeRange').value);
                const dates = [...new Set([
                    ...sampleCpuData.map(d => d.x),
                    ...sampleMemoryData.map(d => d.x)
                ])];

                const processedCpuData = processLargeData(sampleCpuData.map(d => d.y), dates);
                const processedMemoryData = processLargeData(sampleMemoryData.map(d => d.y), dates);
                const processedDiskData = {};

                Object.keys(sampleDiskDataMap).forEach(label => {
                    processedDiskData[label] = processLargeData(
                        sampleDiskDataMap[label].map(d => d.y),
                        dates
                    ).data;
                });

                if (cpuChart) cpuChart.destroy();
                if (memoryChart) memoryChart.destroy();
                if (diskChart) diskChart.destroy();

                cpuChart = createChart(cpuCtx, processedCpuData.data, processedCpuData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
                memoryChart = createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');

                updateDiskPanels(Object.keys(sampleDiskDataMap));
                diskChart = createChart(diskCtx, processedDiskData[Object.keys(sampleDiskDataMap)[0]], processedCpuData.dates, `Disk ${Object.keys(sampleDiskDataMap)[0]} Usage`, 'rgba(255, 159, 64, 1)');
            };

            const updateDiskChart = (diskLabel) => {
                const processedDiskData = processLargeData(
                    sampleDiskDataMap[diskLabel].map(d => d.y),
                    sampleDates
                );
                if (diskChart) diskChart.destroy();

                diskChart = createChart(diskCtx, processedDiskData.data, processedDiskData.dates, `Disk ${diskLabel} Usage`, 'rgba(255, 159, 64, 1)');
            };

            const handlePanelClick = (diskLabel) => {
                document.querySelectorAll('.panel').forEach(panel => {
                    panel.classList.remove('active');
                });
                document.querySelector(`.panel[data-disk="${diskLabel}"]`).classList.add('active');
                updateDiskChart(diskLabel);
            };

            const calculateAverage = (data) => {
                if (!data.length) return 0;
                const sum = data.reduce((acc, cur) => acc + cur.y, 0);
                return (sum / data.length).toFixed(2);
            };

            const cpuCtx = document.getElementById('cpuChart').getContext('2d');
            const memoryCtx = document.getElementById('memoryChart').getContext('2d');
            const diskCtx = document.getElementById('diskChart').getContext('2d');

            window.onload = fetchData;
        </script>
    </form>
</body>

</html>
