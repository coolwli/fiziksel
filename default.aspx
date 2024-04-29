 HtmlGenericControl divChart = new HtmlGenericControl("div");
                divChart.Attributes["class"] = "chart-container";

                Chart chart = new Chart();
                chart.Width = 300;
                chart.Height = 300;
                chart.ChartAreas.Add(new ChartArea());
                chart.Titles.Add(column.Key + " Grafiği");

                Series series = new Series();
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = false;

                foreach (var valueCount in column.Value)
                {
                    DataPoint point = new DataPoint();

                    point.SetValueY(valueCount.Value);
                    point.LegendText = $"{valueCount.Key} ({valueCount.Value})";
                    series.Points.Add(point);
                }

                chart.Series.Add(series);

                Legend legend = new Legend();
                legend.Docking = Docking.Bottom;
                chart.Legends.Add(legend);

                divChart.Controls.Add(chart);

                chartsContainer.Controls.Add(divChart);
