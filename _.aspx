<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Grafik Görselleştirme</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns@latest"></script>
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

        canvas {
            max-width: 100%;
            max-height: 100%;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: #fff;
        }

        .chart-container {
            width: 80%;
            height: 80%;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }
    </style>
</head>

<body>
    <div class="chart-container">
        <canvas id="cpuChart"></canvas>
        <canvas id="memoryChart"></canvas>
    </div>
    <script>
        const labels = [
            '2024-06-29 16:59:59', '2024-06-29 19:59:59', '2024-06-29 22:59:59',
            '2024-06-30 01:59:59', '2024-06-30 04:59:59', '2024-06-30 07:59:59',
            '2024-06-30 10:59:59', '2024-06-30 13:59:59', '2024-06-30 16:59:59',
            '2024-06-30 19:59:59', '2024-06-30 22:59:59', '2024-07-01 01:59:59',
            '2024-07-01 04:59:59', '2024-07-01 07:59:59', '2024-07-01 10:59:59',
            '2024-07-01 13:59:59', '2024-07-01 16:59:59', '2024-07-01 19:59:59',
            '2024-07-01 22:59:59', '2024-07-02 01:59:59', '2024-07-02 04:59:59',
            '2024-07-02 07:59:59', '2024-07-02 10:59:59', '2024-07-02 13:59:59',
            '2024-07-02 16:59:59', '2024-07-02 19:59:59', '2024-07-02 22:59:59',
            '2024-07-03 01:59:59', '2024-07-03 04:59:59', '2024-07-03 07:59:59',
            '2024-07-03 10:59:59', '2024-07-03 13:59:59', '2024-07-03 16:59:59',
            '2024-07-03 19:59:59', '2024-07-03 22:59:59', '2024-07-04 01:59:59',
            '2024-07-04 04:59:59', '2024-07-04 07:59:59', '2024-07-04 10:59:59',
            '2024-07-04 13:59:59', '2024-07-04 16:59:59', '2024-07-04 19:59:59',
            '2024-07-04 22:59:59', '2024-07-05 01:59:59', '2024-07-05 04:59:59',
            '2024-07-05 07:59:59', '2024-07-05 10:59:59', '2024-07-05 13:59:59',
            '2024-07-05 16:59:59', '2024-07-05 19:59:59', '2024-07-05 22:59:59',
            '2024-07-06 01:59:59', '2024-07-06 04:59:59', '2024-07-06 07:59:59',
            '2024-07-06 10:59:59', '2024-07-06 13:59:59', '2024-07-06 16:59:59',
            '2024-07-06 19:59:59', '2024-07-06 22:59:59', '2024-07-07 01:59:59',
            '2024-07-07 04:59:59', '2024-07-07 07:59:59', '2024-07-07 10:59:59',
            '2024-07-07 13:59:59', '2024-07-07 16:59:59', '2024-07-07 19:59:59',
            '2024-07-07 22:59:59', '2024-07-08 01:59:59', '2024-07-08 04:59:59',
            '2024-07-08 07:59:59', '2024-07-08 10:59:59', '2024-07-08 13:59:59',
            '2024-07-08 16:59:59', '2024-07-08 19:59:59', '2024-07-08 22:59:59',
            '2024-07-09 01:59:59', '2024-07-09 04:59:59', '2024-07-09 07:59:59',
            '2024-07-09 10:59:59', '2024-07-09 13:59:59', '2024-07-09 16:59:59',
            '2024-07-09 19:59:59', '2024-07-09 22:59:59', '2024-07-10 01:59:59',
            '2024-07-10 04:59:59', '2024-07-10 07:59:59', '2024-07-10 10:59:59',
            '2024-07-10 13:59:59', '2024-07-10 16:59:59', '2024-07-10 19:59:59',
            '2024-07-10 22:59:59', '2024-07-11 01:59:59', '2024-07-11 04:59:59',
            '2024-07-11 07:59:59', '2024-07-11 10:59:59', '2024-07-11 13:59:59',
            '2024-07-11 16:59:59', '2024-07-11 19:59:59', '2024-07-11 22:59:59',
            '2024-07-12 01:59:59', '2024-07-12 04:59:59', '2024-07-12 07:59:59',
            '2024-07-12 10:59:59', '2024-07-12 13:59:59', '2024-07-12 16:59:59',
            '2024-07-12 19:59:59', '2024-07-12 22:59:59', '2024-07-13 01:59:59',
            '2024-07-13 04:59:59', '2024-07-13 07:59:59', '2024-07-13 10:59:59',
            '2024-07-13 13:59:59', '2024-07-13 16:59:59', '2024-07-13 19:59:59',
            '2024-07-13 22:59:59', '2024-07-14 01:59:59', '2024-07-14 04:59:59',
            '2024-07-14 07:59:59', '2024-07-14 10:59:59', '2024-07-14 13:59:59',
            '2024-07-14 16:59:59', '2024-07-14 19:59:59', '2024-07-14 22:59:59',
            '2024-07-15 01:59:59', '2024-07-15 04:59:59', '2024-07-15 07:59:59',
            '2024-07-15 10:59:59', '2024-07-15 13:59:59', '2024-07-15 16:59:59',
            '2024-07-15 19:59:59', '2024-07-15 22:59:59', '2024-07-16 01:59:59'
        ];

        const createChart = (ctx, data, label, borderColor) => {
            const min = Math.min(...data);
            const max = Math.max(...data);

            const minIndex = data.indexOf(min);
            const maxIndex = data.indexOf(max);

            const config = {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Minimum',
                            data: [{
                                x: labels[minIndex],
                                y: min
                            }],
                            borderColor: 'red',
                            fill: true,
                            pointRadius: 6,
                            pointHoverRadius: 7,
                            pointBackgroundColor: 'red'
                        }, {
                            label: 'Maximum',
                            data: [{
                                x: labels[maxIndex],
                                y: max
                            }],
                            borderColor: 'black',
                            fill: true,
                            pointRadius: 6,
                            pointHoverRadius: 7,
                            pointBackgroundColor: 'black'
                        },
                        {
                            label: label,
                            data: data,
                            borderColor: borderColor,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            fill: true,
                            pointRadius: 0,
                            pointHoverRadius: 7,
                            tension: 0.1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        title: {
                            display: true,
                            text: `Chart.js Line Chart - ${label}`
                        },
                    },
                    interaction: {
                        intersect: false,
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
                            suggestedMin: min - 10,
                            suggestedMax: max + 10
                        }
                    }
                },
            };

            return new Chart(ctx, config);
        };

        const cpuCtx = document.getElementById('cpuChart').getContext('2d');
        const memoryCtx = document.getElementById('memoryChart').getContext('2d');

        cpuDatas = [];
        memoryDatas = [];
        for (let i = 0; i < labels.length; i++) {
            cpuDatas.push(Math.floor(Math.random() * 20 + 10));
            memoryDatas.push(Math.floor(Math.random() * 50));
        }

        createChart(cpuCtx, cpuDatas, 'CPU Usage', 'rgba(75, 192, 192, 1)');
        createChart(memoryCtx, memoryDatas, 'Memory Usage', 'rgba(153, 102, 255, 1)');
    </script>
</body>

</html>
