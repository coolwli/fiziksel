using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;

namespace WebApplication1
{
    public partial class FileName : System.Web.UI.Page
    {
        Dictionary<string, Dictionary<string, int>> columnUniqueValuesCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            columnUniqueValuesCount = new Dictionary<string, Dictionary<string, int>>();

            for (int rowIndex = 1; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                HtmlTableRow row = dataTable.Rows[rowIndex];
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    string columnName = dataTable.Rows[0].Cells[i].InnerText;
                    string cellValue = row.Cells[i].InnerText;

                    if (!columnUniqueValuesCount.ContainsKey(columnName))
                    {
                        columnUniqueValuesCount[columnName] = new Dictionary<string, int>();
                    }

                    if (!columnUniqueValuesCount[columnName].ContainsKey(cellValue))
                    {
                        columnUniqueValuesCount[columnName][cellValue] = 1;
                    }
                    else
                    {
                        columnUniqueValuesCount[columnName][cellValue]++;
                    }
                }
            }
            foreach (var column in columnUniqueValuesCount)
            {
                HtmlGenericControl divChart = new HtmlGenericControl("div");
                divChart.Attributes["class"] = "chart-container";

                Chart chart = new Chart();
                chart.Width = 400;
                chart.Height = 400;
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

                    if (ShouldShowLabel((double)valueCount.Value / column.Value.Values.Sum()))
                    {
                        point.Label = valueCount.Key;
                        point.IsValueShownAsLabel = true;
                    }
                    else
                    {
                        point.IsValueShownAsLabel = false;
                    }

                    series.Points.Add(point);
                }

                chart.Series.Add(series);

                Legend legend = new Legend();
                legend.Docking = Docking.Right;
                chart.Legends.Add(legend);

                divChart.Controls.Add(chart);

                chartsContainer.Controls.Add(divChart);
            }

        }

        private bool ShouldShowLabel(double value)
        {
            double threshold = 0.5; 
            return value >= threshold;
        }

        protected void btnCreateCharts_Click(object sender, EventArgs e)
        {

            CreateExcelFile();
        }
        private void CreateExcelFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Charts");

                int startRow = 1;

                foreach (var column in columnUniqueValuesCount)
                {
                    int startColumn = 1;
                    var pieChart = worksheet.Drawings.AddChart("PieChart_" + column.Key, eChartType.Pie);
                    pieChart.Border.LineStyle = eLineStyle.Solid;
                    pieChart.SetPosition(startRow - 1, 0, startColumn + 2, 0);
                    pieChart.SetSize(400, 400);
                    pieChart.Title.Text=column.Key;


                    worksheet.Cells[startRow, startColumn].Value = column.Key;
                    startRow++;
                    int dataStartRow = startRow ;
                    foreach (var valueCount in column.Value)
                    {
                        worksheet.Cells[startRow, startColumn].Value = valueCount.Key;
                        worksheet.Cells[startRow, startColumn + 1].Value = valueCount.Value;
                        startRow++;
                    }

                    var chartDataRange = worksheet.Cells[dataStartRow, startColumn + 1, dataStartRow+column.Value.Count-1, startColumn + 1];
                    var chartLabelsRange = worksheet.Cells[dataStartRow, startColumn, dataStartRow +column.Value.Count-1, startColumn];

                    // Add chart series
                    pieChart.Series.Add(chartDataRange, chartLabelsRange);

                    startRow = startRow + 20;
                }

                // Save the Excel package
                string filePath = Server.MapPath("~/Charts.xlsx");
                excelPackage.SaveAs(new FileInfo(filePath));

                // Send the file to the browser
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Charts.xlsx");
                Response.TransmitFile(filePath);
                Response.End();
            }
        }


    }

}

