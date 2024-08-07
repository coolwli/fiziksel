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
                    sumDates +=new Date(dates[i]).getTime(); // Add date as time in milliseconds
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
                const minDate = new Date(dates[minIndex]);
                const maxDate = new Date(dates[maxIndex]);

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
