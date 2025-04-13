let filteredDates = [];
let cpuChart, memoryChart, diskChart;

const cpuCtx = document.getElementById('cpuChart').getContext('2d');
const memoryCtx = document.getElementById('memoryChart').getContext('2d');
const diskCtx = document.getElementById('diskChart').getContext('2d');

let diskDataMap = {};

// 📊 Chart Oluşturucu
function createChart(ctx, data, dates, label, borderColor) {
    ctx.canvas.style.display = "block";

    const yValues = data.map(d => d.y);
    const min = Math.min(...yValues);
    const max = Math.max(...yValues);

    const findPoint = (value) => {
        const index = yValues.indexOf(value);
        return { x: dates[index], y: value };
    };

    const config = {
        type: 'line',
        data: {
            labels: dates,
            datasets: [
                {
                    label: `Min: %${min}`,
                    data: [findPoint(min)],
                    borderColor: 'red',
                    pointRadius: 6,
                    pointBackgroundColor: 'red',
                    fill: true
                },
                {
                    label: `Max: %${max}`,
                    data: [findPoint(max)],
                    borderColor: 'black',
                    pointRadius: 6,
                    pointBackgroundColor: 'black',
                    fill: true
                },
                {
                    label,
                    data,
                    borderColor,
                    backgroundColor: borderColor.replace("1)", "0.2)"),
                    pointRadius: 0,
                    borderWidth: 1,
                    tension: 0.01,
                    fill: true
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
            interaction: { intersect: false },
            scales: {
                x: {
                    type: 'time',
                    time: { unit: 'day' },
                    title: { display: true, text: 'Date' }
                },
                y: {
                    title: { display: true, text: 'Value' },
                    suggestedMin: Math.max(0, min * 0.9),
                    suggestedMax: max * 1.1
                }
            }
        }
    };

    return new Chart(ctx, config);
}

// 🔍 Büyük Veri İşleme
function processLargeData(data, dates, extremeCount = 10, maxPoints = 300) {
    if (data.length <= maxPoints) {
        return {
            data: data.map((y, i) => ({ x: new Date(dates[i]), y })),
            dates: dates.map(d => new Date(d))
        };
    }

    const reduced = [];
    const reducedDates = [];
    const step = Math.ceil(data.length / maxPoints);

    for (let i = 0; i < data.length; i += step) {
        const chunk = data.slice(i, i + step);
        const avg = chunk.reduce((a, b) => a + b, 0) / chunk.length;
        const timeAvg = chunk.map((_, j) => new Date(dates[i + j]).getTime())
                             .reduce((a, b) => a + b, 0) / chunk.length;

        const dateAvg = new Date(timeAvg);
        reduced.push({ x: dateAvg, y: avg });
        reducedDates.push(dateAvg);
    }

    const addExtremes = (compareFn) => {
        return data.map((y, i) => ({ x: new Date(dates[i]), y }))
                   .sort(compareFn)
                   .slice(0, extremeCount);
    };

    [...addExtremes((a, b) => a.y - b.y), ...addExtremes((a, b) => b.y - a.y)]
        .forEach(point => {
            if (!reduced.some(d => d.x.getTime() === point.x.getTime())) {
                reduced.push(point);
                reducedDates.push(point.x);
            }
        });

    reduced.sort((a, b) => a.x - b.x);
    return {
        data: reduced,
        dates: reduced.map(d => d.x)
    };
}

// 📈 CPU ve Bellek Grafikleri Çiz
function fetchData() {
    initializeProperties();
    const range = parseInt(document.getElementById('timeRange').value);
    filteredDates = dates.filter(date => new Date() - new Date(date) <= range * 86400000);

    const processAndDraw = (ctx, chart, data, label, color) => {
        const filtered = data.slice(-filteredDates.length);
        const processed = processLargeData(filtered, filteredDates);
        if (chart) chart.destroy();
        ctx.canvas.style.display = "none";
        if (processed.dates.length)
            return createChart(ctx, processed.data, processed.dates, label, color);
    };

    cpuChart = processAndDraw(cpuCtx, cpuChart, cpuDatas, 'CPU Usage', 'rgba(84, 175, 228, 1)');
    memoryChart = processAndDraw(memoryCtx, memoryChart, memDatas, 'Memory Usage', 'rgba(153, 102, 255, 1)');
}

// 💾 Disk Paneli Güncelleme
function updateDiskPanels(diskLabels) {
    const container = document.getElementById('diskPanelContainer');
    container.style.display = "";
    container.innerHTML = '';

    diskLabels.forEach(label => {
        const panel = document.createElement('div');
        panel.className = 'disk-panel';
        panel.dataset.disk = label;
        panel.innerHTML = `${label}: ${calculateAverage(diskDataMap[label])}%`;
        panel.onclick = () => handlePanelClick(label);
        container.appendChild(panel);
    });
}

// 🟠 Disk Grafiğini Güncelle
function updateDiskChart(label) {
    const diskData = diskDataMap[label]?.slice(-filteredDates.length);
    if (!diskData) return;

    const processed = processLargeData(diskData, filteredDates);
    if (diskChart) diskChart.destroy();
    diskCtx.canvas.style.display = "none";

    if (processed.dates.length)
        diskChart = createChart(diskCtx, processed.data, processed.dates, `Disk (${label}) Usage`, 'rgba(255, 159, 64, 1)');
}

// 📌 Disk Paneli Seçildiğinde
function handlePanelClick(label) {
    document.querySelectorAll('.disk-panel').forEach(p => p.classList.remove('active'));
    const selected = document.querySelector(`.disk-panel[data-disk="${label}"]`);
    if (selected) selected.classList.add('active');
    updateDiskChart(label);
}

// 📊 Ortalama Hesapla
function calculateAverage(data) {
    const sum = data.reduce((acc, val) => acc + val, 0);
    return (sum / data.length).toFixed(2);
}

// 🔄 Disk Verisini Yenile
function fetchDisk() {
    fetchData();
    const labels = Object.keys(diskDataMap).sort();
    updateDiskPanels(labels);
    if (labels.length > 0) handlePanelClick(labels[0]);
}
