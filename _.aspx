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
            document.addEventListener('DOMContentLoaded', () => {
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
                                    label: 'Maximum: %' + max,
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
                                    suggestedMin: Math.max(0, Math.min(...data.map(d => d.y)) - 10),
                                    suggestedMax: Math.max(...data.map(d => d.y)) + 10
                                }
                            }
                        },
                    };

                    return new Chart(ctx, config);
                };

                const fetchData = async () => {
                    // API'den veri çekme simülasyonu
                    for (let i = 0; i < 5500; i++) {
                        const date = new Date();
                        date.setDate(date.getDate() + i);
                        dates.push(date);
                        cpuDatas.push(Math.random() * 100 + 10);
                        memoryDatas.push(Math.random() * 30 + 20);
                    }

                    // Veriyi işle
                    const processedCPUData = processLargeData(cpuDatas, dates);
                    const processedMemoryData = processLargeData(memoryDatas, dates);

                    // Processed dates should be the same for both datasets, ensuring synchronization
                    createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
                    createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
                };

                const processLargeData = (data, dates) => {
                    if (data.length <= 250) {
                        return {
                            data: data.map((value, index) => ({
                                x: dates[index],
                                y: value
                            })),
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
                            reducedData.push({
                                x: averageDate,
                                y: average
                            });
                            reducedDates.push(averageDate);
                            sum = 0;
                            sumDates = 0;
                            count = 0;
                        }
                    }

                    // Handle any remaining data after the loop
                    if (count > 0) {
                        const average = sum / count;
                        const averageDate = new Date(sumDates / count);
                        reducedData.push({
                            x: averageDate,
                            y: average
                        });
                        reducedDates.push(averageDate);
                    }

                    // Handle min and max values explicitly
                    const min = Math.min(...data);
                    const max = Math.max(...data);
                    const minIndex = data.indexOf(min);
                    const maxIndex = data.indexOf(max);

                    // Remove old min/max data if they exist
                    const minDate = dates[minIndex];
                    const maxDate = dates[maxIndex];

                    // Ensure min/max data is not duplicated
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

                    // Sort by date to ensure the order is correct
                    reducedData.sort((a, b) => a.x - b.x);
                    reducedDates.sort((a, b) => a - b);

                    return { dates: reducedDates, data: reducedData };
                };

                // Get context after DOM is loaded
                const cpuCtx = document.getElementById('cpuChart').getContext('2d');
                const memoryCtx = document.getElementById('memoryChart').getContext('2d');
                let dates = [];
                let cpuDatas = [];
                let memoryDatas = [];

                fetchData();
            });
        </script>

    </form>
</body>

</html>
