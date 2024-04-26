using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
//using OfficeOpenXml;
//using OfficeOpenXml.Drawing;
//using OfficeOpenXml.Drawing.Chart;

namespace fizkselArayuz
{
    public partial class pichart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string scriptCode = "var veri =JSON.parse(localStorage.getItem('dizi'))";
            ClientScript.RegisterStartupScript(this.GetType(), "LocalDataScript", scriptCode, true);
            string veri = Page.ClientScript.GetWebResourceUrl(this.GetType(), "veri");
            Response.Write(HiddenField1.Value+" dc " +veri);
        }



        private void WriteChartToDiv(Dictionary<string, int> dict)
        {
            Chart chart = new Chart();
            chart.Width = 400;
            chart.Height = 300;

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.ChartType = SeriesChartType.Pie;

            foreach (var kvp in dict)
            {
                DataPoint point = new DataPoint();
                point.SetValueY(kvp.Value);
                point.Label = $"{kvp.Key} ({kvp.Value})"; // Include count in the label
                series.Points.Add(point);
            }

            chart.Series.Add(series);

            Legend legend = new Legend();
            legend.Docking = Docking.Bottom;
            chart.Legends.Add(legend);

            using (MemoryStream ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                string imageBase64 = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                HtmlGenericControl nevDiv = new HtmlGenericControl("div");
                nevDiv.InnerHtml = $"<img src='{imageBase64}' alt='Chart' />";
                
                chartContainer.Controls.Add(nevDiv);
            }
        }

    }
}
