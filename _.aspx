<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="blank_page._default" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Cloud United</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <script src="chart.js"></script>
    <script src="chartjs-adapter-date-fns.js"></script>
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
    <form id="form1" runat="server" style="width: 100%;height: 100%;">
        <p runat="server" id="Label"></p>
        <div class="chart-container">
            <canvas id="cpuChart"></canvas>
            <canvas id="memoryChart"></canvas>
        </div>
        <script>
            const createChart = (ctx, data, label, borderColor) => {
                const min = Math.min(...data);
                const max = Math.max(...data);

                const minIndex = data.indexOf(min);
                const maxIndex = data.indexOf(max);
                const config = {
                    type: 'line',
                    data: {
                        labels: dates,
                        datasets: [
                            {
                                label: 'Minimum',
                                data: [{
                                    x: dates[minIndex],
                                    y: min
                                }],
                                borderColor: 'red',
                                fill: true,
                                pointRadius: 6,
                                pointHoverRadius: 4,
                                pointBackgroundColor: 'red'
                            }, {
                                label: 'Maximum',
                                data: [{
                                    x: dates[maxIndex],
                                    y: max
                                }],
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
                                suggestedMin: (min - 10) < 0 ? 0 : min-10,
                                suggestedMax: max + 10
                            }
                        }
                    },
                };

                return new Chart(ctx, config);
            };

            const cpuCtx = document.getElementById('cpuChart').getContext('2d');
            const memoryCtx = document.getElementById('memoryChart').getContext('2d');
            let dates = [];
            let cpuDatas = [];
            let memoryDatas= [];

            console.log(dates.length);
            function startCharts() {
                createChart(cpuCtx, cpuDatas, 'CPU Usage', 'rgba(75, 192, 192, 1)');
                createChart(memoryCtx, memoryDatas, 'Memory Usage', 'rgba(153, 102, 255, 1)');
            }

        </script>
    </form>
</body>
</html>
