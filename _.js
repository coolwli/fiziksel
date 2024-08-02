<script>
    // URL Parametresinden ID'yi Alma
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get("id");
    document.getElementById("baslik").textContent = id;

    // Tablo Sıralama Fonksiyonu
    function sortTable(columnIndex, column) {
        const table = column.closest('table');
        const rows = Array.from(table.rows).slice(1);
        let switching = true;
        let dir = "asc";

        while (switching) {
            switching = false;
            for (let i = 0; i < rows.length - 1; i++) {
                let shouldSwitch = false;
                let xValue = rows[i].getElementsByTagName("td")[columnIndex].innerText;
                let yValue = rows[i + 1].getElementsByTagName("td")[columnIndex].innerText;

                if (!isNaN(xValue) && !isNaN(yValue)) {
                    xValue = parseFloat(xValue);
                    yValue = parseFloat(yValue);
                }
                if ((dir === "asc" && xValue > yValue) || (dir === "desc" && xValue < yValue)) {
                    shouldSwitch = true;
                    break;
                }
            }
            if (shouldSwitch) {
                [rows[i], rows[i + 1]] = [rows[i + 1], rows[i]];
                switching = true;
            } else if (dir === "asc") {
                dir = "desc";
                switching = true;
            }
        }
        rows.forEach(row => table.appendChild(row));
    }

    // Sekme Açma Fonksiyonu
    function openTab(event, panelId, button) {
        event.preventDefault();
        const tabContent = document.getElementsByClassName("columnRight");
        const tabButtons = document.getElementsByClassName("tab-button");

        Array.from(tabContent).forEach(content => content.style.display = "none");
        Array.from(tabButtons).forEach(btn => btn.classList.remove("active"));

        document.getElementById(panelId).style.display = "flex";
        button.classList.add("active");
    }

    // Host ve Cluster Navigasyonu
    document.getElementById("host").addEventListener("click", function () {
        window.location.href = `hostscreen.aspx?id=${this.textContent}`;
    });
    document.getElementById("cluster").addEventListener("click", function () {
        window.location.href = `clusterscreen.aspx?id=${this.textContent}`;
    });
</script>


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
                        label: `Minimum: ${min}`,
                        data: [{ x: dates[minIndex], y: min }],
                        borderColor: 'red',
                        fill: true,
                        pointRadius: 6,
                        pointHoverRadius: 4,
                        pointBackgroundColor: 'red'
                    },
                    {
                        label: `Maximum: ${max}`,
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
                        time: { unit: 'day' },
                        display: true,
                        title: { display: true, text: 'Date' }
                    },
                    y: {
                        display: true,
                        title: { display: true, text: 'Value' },
                        suggestedMin: Math.max(0, min - 10),
                        suggestedMax: max + 10
                    }
                }
            }
        };

        return new Chart(ctx, config);
    };

    const processLargeData = (data, dates) => {
        const maxDataPoints = 350;
        if (data.length <= maxDataPoints) {
            return { data: data.map((value, index) => ({ x: dates[index], y: value })), dates: dates };
        }

        const step = Math.ceil(data.length / maxDataPoints);
        const reducedData = [];
        const reducedDates = [];

        for (let i = 0; i < data.length; i += step) {
            const chunk = data.slice(i, i + step);
            const avgValue = chunk.reduce((acc, curr) => acc + curr, 0) / chunk.length;
            const avgDate = new Date(dates.slice(i, i + step).reduce((acc, curr) => acc + new Date(curr).getTime(), 0) / chunk.length);

            reducedData.push({ x: avgDate, y: avgValue });
            reducedDates.push(avgDate);
        }

        // Min ve max değerleri indirgenmiş verilere ekleme
        const minValues = [];
        const maxValues = [];
        for (let i = 0; i < data.length; i++) {
            if (data[i] === Math.min(...data)) {
                minValues.push({ x: new Date(dates[i]), y: data[i] });
            }
            if (data[i] === Math.max(...data)) {
                maxValues.push({ x: new Date(dates[i]), y: data[i] });
            }
        }

        // Min ve max değerleri indirgenmiş verilere ekleyelim ve tekrar sıraya koyalım
        reducedData.push(...minValues, ...maxValues);
        reducedData.sort((a, b) => a.x - b.x);
        reducedDates.sort((a, b) => a - b);

        return { data: reducedData, dates: reducedDates };
    };

    const fetchData = () => {
        const timeRange = parseInt(document.getElementById('timeRange').value);
        const filteredDates = dates.filter(dateString => new Date() - new Date(dateString) <= (timeRange * 24 * 60 * 60 * 1000));
        const filteredCpuData = cpuDatas.slice(-filteredDates.length);
        const filteredMemoryData = memoryDatas.slice(-filteredDates.length);
        const processedCPUData = processLargeData(filteredCpuData, filteredDates);
        const processedMemoryData = processLargeData(filteredMemoryData, filteredDates);

        if (cpuChart) cpuChart.destroy();
        if (memoryChart) memoryChart.destroy();

        cpuChart = createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
        memoryChart = createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
    };

    const cpuCtx = document.getElementById('cpuChart').getContext('2d');
    const memoryCtx = document.getElementById('memoryChart').getContext('2d');
    let cpuChart, memoryChart;
</script>






