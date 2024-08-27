let filteredDates;
const cpuCtx = document.getElementById('cpuChart').getContext('2d');
const memoryCtx = document.getElementById('memoryChart').getContext('2d');

let cpuChart, memoryChart;

const createChart = (ctx, data, dates, label, borderColor) => {
    ctx.canvas.style.display = "block";

    const min = Math.min(...data.map(d => d.y));
    const max = Math.max(...data.map(d => d.y));

    const minIndex = data.findIndex(d => d.y === min);
    const maxIndex = data.findIndex(d => d.y === max);

    const backgroundColor = borderColor.substring(0, 19) + " 0.2)";
    const config = {
        type: 'line',
        data: {
            labels: dates,
            datasets: [
                {
                    label: `Minimum: %${min}`,
                    data: [{ x: dates[minIndex], y: min }],
                    borderColor: 'red',
                    fill: true,
                    pointRadius: 6,
                    pointHoverRadius: 4,
                    pointBackgroundColor: 'red'
                },
                {
                    label: `Maximum: %${max}`,
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
                    tension: 0.01
                }
            ]
        },
        options: {
            responsive: true,
            animation: { duration: 0 },
            plugins: {
                title: {
                    display: true,
                    text: `${label} Average %`
                }
            },
            interaction: {
                intersect: false
            },
            scales: {
                x: {
                    type: 'time',
                    time: { unit: 'day' },
                    display: true,
                    title: { display: true, text: 'Date' }
                },
                y: {
                    display: true,
                    title: { display: true, text: 'Value' },
                    suggestedMin: Math.max(0, min - min * 0.1),
                    suggestedMax: max + max * 0.1
                }
            }
        }
    };

    return new Chart(ctx, config);
};

const processLargeData = (data, dates, numExtremePoints = 10, maxPoints = 300) => {
    if (data.length <= maxPoints) {
        return {
            data: data.map((value, index) => ({
                x: new Date(dates[index]),
                y: value
            })),
            dates: dates.map(date => new Date(date))
        };
    }

    const step = Math.ceil(data.length / maxPoints);
    const reducedData = [];
    const reducedDates = [];
    let sum = 0, count = 0, sumDates = 0;

    for (let i = 0; i < data.length; i++) {
        sum += data[i];
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
    }

    if (count > 0) {
        const average = sum / count;
        const averageDate = new Date(sumDates / count);
        reducedData.push({ x: averageDate, y: average });
        reducedDates.push(averageDate);
    }

    const addExtremePoints = (data, dates, numPoints, comparator) => {
        const extremePoints = [];
        for (let i = 0; i < data.length; i++) {
            if (extremePoints.length < numPoints) {
                extremePoints.push({ x: new Date(dates[i]), y: data[i] });
                extremePoints.sort(comparator);
            } else if (comparator({ y: data[i] }, extremePoints[numPoints - 1])) {
                extremePoints[numPoints - 1] = { x: new Date(dates[i]), y: data[i] };
                extremePoints.sort(comparator);
            }
        }
        return extremePoints;
    };

    const minPoints = addExtremePoints(data, dates, numExtremePoints, (a, b) => a.y - b.y);
    const maxPoints = addExtremePoints(data, dates, numExtremePoints, (a, b) => b.y - a.y);

    minPoints.forEach(point => {
        if (!reducedData.some(d => d.x.getTime() === point.x.getTime())) {
            reducedData.push(point);
            reducedDates.push(point.x);
        }
    });

    maxPoints.forEach(point => {
        if (!reducedData.some(d => d.x.getTime() === point.x.getTime())) {
            reducedData.push(point);
            reducedDates.push(point.x);
        }
    });

    reducedData.sort((a, b) => a.x - b.x);
    reducedDates.sort((a, b) => a - b);

    // Fill in missing dates with zero values
    const completeData = [];
    const completeDates = [];
    const dateSet = new Set(reducedDates.map(date => date.toISOString()));

    let currentDate = new Date(reducedDates[0]);
    while (currentDate <= new Date(reducedDates[reducedDates.length - 1])) {
        const isoDate = currentDate.toISOString();
        if (!dateSet.has(isoDate)) {
            completeData.push({ x: currentDate, y: 0 });
        } else {
            const index = reducedDates.findIndex(date => date.toISOString() === isoDate);
            completeData.push(reducedData[index]);
        }
        completeDates.push(currentDate);
        currentDate.setDate(currentDate.getDate() + 1);
    }

    return {
        dates: completeDates,
        data: completeData
    };
};

function fetchData() {
    const timeRange = parseInt(document.getElementById('timeRange').value, 10);
    filteredDates = dates.filter(dateString => {
        return new Date() - new Date(dateString) <= (timeRange * 24 * 60 * 60 * 1000);
    });

    const filteredCpuData = cpuDatas.slice(-filteredDates.length);
    const filteredMemoryData = memDatas.slice(-filteredDates.length);

    const processedCPUData = processLargeData(filteredCpuData, filteredDates);
    const processedMemoryData = processLargeData(filteredMemoryData, filteredDates);

    if (cpuChart) cpuChart.destroy();
    if (memoryChart) memoryChart.destroy();

    cpuCtx.canvas.style.display = "none";
    memoryCtx.canvas.style.display = "none";
    if (processedCPUData.dates.length > 0)
        cpuChart = createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(84, 175, 228, 1)');
    if (processedMemoryData.dates.length > 0)
        memoryChart = createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
}
