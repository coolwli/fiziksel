using System;
using System.Web.UI.HtmlControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace CloudUnited
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string excelPath = @"F:/Ali/CloudUnited/Datas/CombinedData.xlsx";
            
            // Tek bir bağlantı kur
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open(excelPath);
            
            // Her bir worksheet'i ayrı ayrı yükle
            Excel.Worksheet wsMembers = wb.Worksheets["Members"];
            Excel.Worksheet wsDirections = wb.Worksheets["Directions"];
            Excel.Worksheet wsServices = wb.Worksheets["Services"];
            
            // Verileri yükle
            loadMembers(wsMembers);
            loadServices(wsServices);
            loadDirections(wsDirections);
            
            // Workbook'u kapat ve Excel uygulamasını kapat
            wb.Close(false);
            excelApp.Quit();
            
            // Kaynakları serbest bırak
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wsMembers);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wsDirections);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wsServices);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

        void loadMembers(Excel.Worksheet ws)
        {
            string cities = "cities=[";
            string members = "members=[";
            Excel.Range range = ws.UsedRange;

            for (int col = 1; col <= range.Columns.Count; col++)
            {
                string colText = range.Cells[1, col].Text;
                cities += "'" + colText + "'";
                cities += ",";
                members += "[";
                for (int i = 2; i <= range.Rows.Count; i++)
                {
                    if (range.Cells[i, col].Text == "") break;
                    members += "'" + range.Cells[i, col].Text + "'";
                    members += ",";
                }
                members = members.Substring(0, members.Length - 1);
                members += "] ";
                members += ",";
            }
            cities = cities.Substring(0, cities.Length - 1);
            cities += "] ";
            members = members.Substring(0, members.Length - 1);
            members += "] ";

            string script = $"<script>{members}; {cities}; showAll();</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", script);
        }

        void loadDirections(Excel.Worksheet ws)
        {
            Excel.Range range = ws.UsedRange;
            for (int i = 2; i <= range.Rows.Count; i++)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "direction-panel");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.ID = range.Cells[i, 3].Text;
                h3.InnerText = range.Cells[i, 1].Text;
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.InnerText = range.Cells[i, 2].Text;
                div.Controls.Add(h3);
                div.Controls.Add(p);
                directions.Controls.Add(div);
            }
        }

        void loadServices(Excel.Worksheet ws)
        {
            Excel.Range range = ws.UsedRange;
            for (int i = 2; i <= range.Rows.Count; i++)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "service-panel");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.ID = range.Cells[i, 3].Text;
                h3.InnerText = range.Cells[i, 1].Text;
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.InnerText = range.Cells[i, 2].Text;
                div.Controls.Add(h3);
                div.Controls.Add(p);
                services.Controls.Add(div);
            }
        }
    }
}
