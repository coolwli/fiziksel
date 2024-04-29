using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;

namespace fizkselArayuz
{
    public partial class pichart : System.Web.UI.Page
    {
        Dictionary<string, Dictionary<string, int>> columnUniqueValues;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Response.Write("");
            }
            else
                Response.Write(Request["hdnTestControl"]);
        }

        private void WriteChartToDiv()
        {
            foreach (var column in columnUniqueValues)
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

                    // Dilimin boyutunu kontrol edip etiketi gösterme kararını alın
                    if (ShouldShowLabel((double)valueCount.Value / column.Value.Values.Sum()))
                    {
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
            // İstenilen eşik değeri burada belirleyin
            double threshold = 0.1; // Örneğin, dilimin yüzde 10'unun altındaysa etiketi gösterme
            return value >= threshold;
        }

        protected void btnFoo_Click(object sender, EventArgs e)
        {
            string hiddenControlValue = Request["hdnTestControl"];
            string[] vs = hiddenControlValue.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            columnUniqueValues = new Dictionary<string, Dictionary<string, int>>();

            foreach (string value in vs)
            {
                if (value.Length == 0) continue;

                string columName = value.Split('~')[0];
                string[] values = value.Split('~')[1].Split('%');

                columnUniqueValues.Add(columName, new Dictionary<string, int>());
                foreach (string opp in values)
                {
                    if (opp.Length == 0) continue;
                    string key = opp.Split('!')[0];
                    int count = Convert.ToInt32(opp.Split('!')[1]);
                    columnUniqueValues[columName].Add(key, count);
                }
            }

            WriteChartToDiv();
        }
    }
}
