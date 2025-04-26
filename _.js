const createChartInstance = (ctx, datasets, dates, chartTitle) => {
                    const chartData = {
                        labels: dates,
                        datasets: datasets.map(({ label, data }) => {
                            const colors = generateRandomColor();
                            return {
                                label,
                                data: data.map((y, i) => ({ x: dates[i], y })),
                                borderColor: colors.borderColor,
                                backgroundColor: colors.backgroundColor,
                                borderWidth: 2,
                                tension: 0.1,
                                pointRadius: 3,
                                fill: false
                            };
                        })
                    };

                    const chartOptions = {
                        responsive: true,
                        plugins: {
                            title: { display: true, text: chartTitle },
                            tooltip: { mode: 'index', intersect: false }
                        },
                        scales: {
                            x: {
                                type: 'time',
                                time: { unit: 'day' },
                                title: { display: true, text: 'Date' }
                            },
                            y: {
                                title: { display: true, text: 'Value' }
                            }
                        }
                    };

                    return new Chart(ctx, { type: 'line', data: chartData, options: chartOptions });
                };
