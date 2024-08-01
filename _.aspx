<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <title>Cloud United</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns"></script>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            height: 100vh;
            margin: 0;
            background-color: #e0f2f1;
        }

        .chart-container {
            height: 90%;
            width: 90%;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .dropdown-container {
            display: flex;
            justify-content: flex-end;
            background: #fff;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        label {
            font-size: 16px;
            color: #333;
            margin-right: 10px;
        }

        select {
            border-radius: 4px;
            border: 1px solid #ccc;
            background-color: #f9f9f9;
            font-size: 14px;
            outline: none;
            width: 120px;
            transition: border-color 0.3s;
            height: 25px;
        }

        select:focus {
            border-color: #007bff;
        }

        .divider {
            width: 100%;
            height: 1px;
            background-color: #ddd;
            margin-top: 10px;
        }

        canvas {
            border-radius: 8px;
            background-color: #fff;
            border: 1px solid #ddd;
            max-width: 100%;
            max-height: 100%;
            box-sizing: border-box;
        }
    </style>
</head>

<body>
    <div class="chart-container">
        <div class="dropdown-container">
            <label for="timeRange">Select Time Range: </label>
            <select id="timeRange" onchange="updateCharts()">
                <option value="1">1 Day</option>
                <option value="7">7 Days</option>
                <option value="30">30 Days</option>
                <option value="90">90 Days</option>
                <option value="360" selected>1 Year</option>
            </select>
        </div>
        <canvas id="cpuChart"></canvas>
        <canvas id="memoryChart"></canvas>
    </div>
    <script>
        const createChart = (ctx, data, dates, label, borderColor) => {
            const min = Math.min(...data.map(d => d.y));
            const max = Math.max(...data.map(d => d.y));

            const minIndex = data.findIndex(d => d.y === min);
            const maxIndex = data.findIndex(d => d.y === max);

            const config = {
                type: 'line',
                data: {
                    labels: dates,
                    datasets: [
                        {
                            label: 'Minimum: %' + min,
                            data: [{ x: dates[minIndex], y: min }],
                            borderColor: 'red',
                            fill: true,
                            pointRadius: 6,
                            pointHoverRadius: 4,
                            pointBackgroundColor: 'red'
                        },
                        {
                            label: 'Maximum: %' + max,
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
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            fill: true,
                            pointRadius: 0,
                            pointHoverRadius: 4,
                            borderWidth: 1,
                            tension: 0.01
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        title: {
                            display: true,
                            text: `Chart.js Line Chart - ${label}`
                        }
                    },
                    interaction: {
                        intersect: false
                    },
                    scales: {
                        x: {
                            type: 'time',
                            time: {
                                unit: 'day'
                            },
                            display: true,
                            title: {
                                display: true,
                                text: 'Date'
                            }
                        },
                        y: {
                            display: true,
                            title: {
                                display: true,
                                text: 'Value'
                            },
                            suggestedMin: Math.max(0, min - 10),
                            suggestedMax: max + 10
                        }
                    }
                }
            };

            return new Chart(ctx, config);
        };

        const processLargeData = (data, dates) => {
            if (data.length <= 250) {
                return {
                    data: data.map((value, index) => ({ x: dates[index], y: value })),
                    dates: dates
                };
            }

            const reducedData = [];
            const reducedDates = [];
            const step = Math.ceil(data.length / 250);
            let sum = 0;
            let count = 0;
            let sumDates = 0;

            for (let i = 0; i < data.length; i++) {
                sum += data[i];
                sumDates += dates[i].getTime(); // Add date as time in milliseconds
                count++;

                if ((i + 1) % step === 0) {
                    const average = sum / count;
                    const averageDate = new Date(sumDates / count); // Calculate average date
                    reducedData.push({ x: averageDate, y: average });
                    reducedDates.push(averageDate);
                    sum = 0;
                    sumDates = 0;
                    count = 0;
                }
            }

            if (count > 0) {
                const average = sum / count;
                const averageDate = new Date(sumDates / count);
                reducedData.push({ x: averageDate, y: average });
                reducedDates.push(averageDate);
            }

            const min = Math.min(...data);
            const max = Math.max(...data);
            const minIndex = data.indexOf(min);
            const maxIndex = data.indexOf(max);
            const minDate = dates[minIndex];
            const maxDate = dates[maxIndex];
            const isMinInReducedData = reducedData.some(d => d.x.getTime() === minDate.getTime());
            const isMaxInReducedData = reducedData.some(d => d.x.getTime() === maxDate.getTime());

            if (!isMinInReducedData) {
                reducedData.push({ x: minDate, y: min });
                reducedDates.push(minDate);
            }
            if (!isMaxInReducedData) {
                reducedData.push({ x: maxDate, y: max });
                reducedDates.push(maxDate);
            }

            reducedData.sort((a, b) => a.x - b.x);
            reducedDates.sort((a, b) => a - b);

            return { dates: reducedDates, data: reducedData };
        };

        const fetchData = async () => {
            for (let i = 0; i < 365; i++) {
                const date = new Date();
                date.setDate(date.getDate() - (365 - i));
                dates.push(date);
                cpuDatas.push(Math.random() * 20 + 10);
                memoryDatas.push(Math.random() * 30 + 20);
            }

            updateCharts();
        };

        const updateCharts = () => {
            const timeRange = parseInt(document.getElementById('timeRange').value);
            const filteredDates = dates.slice(-timeRange);
            const filteredCpuData = cpuDatas.slice(-timeRange);
            const filteredMemoryData = memoryDatas.slice(-timeRange);

            const processedCPUData = processLargeData(filteredCpuData, filteredDates);
            const processedMemoryData = processLargeData(filteredMemoryData, filteredDates);

            if (cpuChart) cpuChart.destroy();
            if (memoryChart) memoryChart.destroy();

            cpuChart = createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
            memoryChart = createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
        };

        const cpuCtx = document.getElementById('cpuChart').getContext('2d');
        const memoryCtx = document.getElementById('memoryChart').getContext('2d');
        const dates = [];
        const cpuDatas = [];
        const memoryDatas = [];
        let cpuChart, memoryChart;

        fetchData();
    </script>

</body>

</html>
