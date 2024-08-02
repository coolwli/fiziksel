        <script>
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get("id");

            const baslik = document.getElementById("baslik");
            baslik.textContent = id;
       
            function sortTable(columnIndex, column) {
                var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
                table = column.closest('table');
                switching = true;
                dir = "asc";
                rows = Array.from(table.rows).slice(1);
    
                while (switching) {
                    switching = false;

                    for (i = 0; i < rows.length - 1; i++) {
                        shouldSwitch = false;
                        xValue = rows[i].getElementsByTagName("td")[columnIndex].innerText;
                        yValue = rows[i + 1].getElementsByTagName("td")[columnIndex].innerText;
            
                        if (!isNaN(xValue) && !isNaN(yValue)) {
                            xValue = parseFloat(xValue);
                            yValue = parseFloat(yValue);
                        }
                        if (dir == "asc") {
                            if (xValue > yValue) {
                                shouldSwitch = true;
                                break;
                            }
                        } else if (dir == "desc") {
                            if (xValue < yValue) {
                                shouldSwitch = true;
                                break;
                            }
                        }
                    }
                    if (shouldSwitch) {
                        [rows[i], rows[i + 1]] = [rows[i + 1], rows[i]];
                        switching = true;
                        switchcount++;
                    } else {
                        if (switchcount == 0 && dir == "asc") {
                            dir = "desc";
                            switching = true;
                        }
                    }
                }

                for (i = 0; i < rows.length; i++) {
                    table.appendChild(rows[i]);
                }
            }

            function openTab(panelId, button,performanceButton) {
                event.preventDefault();
                var i, tabcontent, tabbuttons;
                tabcontent = document.getElementsByClassName("columnRight");
                tabbuttons = document.getElementsByClassName("tab-button");

                for (i = 0; i < tabcontent.length; i++) {
                    tabcontent[i].style.display = "none";
                }
                for (i = 0; i < tabbuttons.length; i++) {
                    tabbuttons[i].classList.remove("active");
                }

                document.getElementById(panelId).style.display = "flex";
                button.classList.add("active");
                
            }
            

            document.getElementById("host").addEventListener("click", function () {
                window.location.href = "hostscreen.aspx?id=" + document.getElementById("host").textContent;
            });
            document.getElementById("cluster").addEventListener("click", function () {
                window.location.href = "clusterscreen.aspx?id=" + document.getElementById("cluster").textContent;
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
                if (data.length <= 350) {
                    return {
                        data: data.map((value, index) => ({ x: dates[index], y: value })),
                        dates: dates
                    };
                }

                const reducedData = [];
                const reducedDates = [];
                const step = Math.ceil(data.length / 350);
                let sum = 0;
                let count = 0;
                let sumDates = 0;
                var date;

                for (let i = 0; i < data.length; i++) {
                    sum += data[i];
                    date = new Date(dates[i])
                    sumDates += date.getTime(); 
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


            const fetchData = () => {
                const timeRange = parseInt(document.getElementById('timeRange').value);
                const filteredDates = dates.filter(dateString => {
                    return new Date() -new Date(dateString) <=(timeRange * 24 *60*60*1000)
                });
                const filteredCpuData = cpuDatas.slice(-filteredDates.length);
                console.log(dates.length, filteredDates.length, filteredCpuData.length);
                const filteredMemoryData = memoryDatas.slice(-filteredDates.length);
                const processedCPUData = processLargeData(filteredCpuData, filteredDates);
                const processedMemoryData = processLargeData(filteredMemoryData, filteredDates);
                console.log(new Date(filteredDates[filteredDates.length - 1]).getDay()-1);

                if (cpuChart) cpuChart.destroy();
                if (memoryChart) memoryChart.destroy();

                cpuChart = createChart(cpuCtx, processedCPUData.data, processedCPUData.dates, 'CPU Usage', 'rgba(75, 192, 192, 1)');
                memoryChart = createChart(memoryCtx, processedMemoryData.data, processedMemoryData.dates, 'Memory Usage', 'rgba(153, 102, 255, 1)');
            };

            const cpuCtx = document.getElementById('cpuChart').getContext('2d');
            const memoryCtx = document.getElementById('memoryChart').getContext('2d');

            let cpuChart, memoryChart;

        </script>
