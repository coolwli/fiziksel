
    const fetchAndProcessData = async (days) => {
        const endDate = new Date();
        const startDate = new Date();
        startDate.setDate(endDate.getDate() - days);

        const formattedStartDate = startDate.toISOString();
        const formattedEndDate = endDate.toISOString();

        const cpuDataUrl = `/api/metrics?metric=cpu&start=${formattedStartDate}&end=${formattedEndDate}`;
        const memoryDataUrl = `/api/metrics?metric=memory&start=${formattedStartDate}&end=${formattedEndDate}`;

        const [cpuResponse, memoryResponse] = await Promise.all([
            fetch(cpuDataUrl),
            fetch(memoryDataUrl)
        ]);

        const [cpuData, memoryData] = await Promise.all([
            cpuResponse.json(),
            memoryResponse.json()
        ]);

        return { cpuData, memoryData };
    };

    const updateCharts = async () => {
        const days = document.getElementById('timeRange').value;

        const { cpuData, memoryData } = await fetchAndProcessData(days);

        const dates = cpuData.dates;

        const processedCPUData = processLargeData(cpuData.values, dates);
        const processedMemoryData = processLargeData(memoryData.values, dates);

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
            sumDates += new Date(dates[i]).getTime();
            count++;

            if ((i + 1) % step === 0) {
                const average = sum / count;
                const averageDate = new Date(sumDates / count);
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

        if (count > 0) {
            const average = sum / count;
            const averageDate = new Date(sumDates / count);
            reducedData.push({
                x: averageDate,
                y: average
            });
            reducedDates.push(averageDate);
        }

        const min = Math.min(...data);
        const max = Math.max(...data);
        const minIndex = data.indexOf(min);
        const maxIndex = data.indexOf(max);

        const minDate = new Date(dates[minIndex]);
        const maxDate = new Date(dates[maxIndex]);

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























<form id="form1" runat="server">
    <div class="header">
        <h1 class="baslik" id="baslik" runat="server">Host Informations</h1>
    </div>
    <div class="container">
        <!-- Diğer içerikler burada -->
        <div>
            <label for="timeRange">Select Time Range:</label>
            <select id="timeRange" onchange="updateCharts()">
                <option value="1">1 Day</option>
                <option value="7">7 Days</option>
                <option value="30">30 Days</option>
                <option value="90">90 Days</option>
                <option value="365">365 Days</option>
            </select>
        </div>
        <div class="columnRight" id="performanceColumn" style="display:none;">
            <div class="chart-panel">
                <canvas id="cpuChart"></canvas>
                <canvas id="memoryChart"></canvas>
            </div>
        </div>
    </div>
    <footer>
        <p class="footer">© 2024 - Cloud United Team</p>
    </footer>
</form>
