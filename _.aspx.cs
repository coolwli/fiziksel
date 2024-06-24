using System;
using System.Web.UI.HtmlControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace CloudUnited
{
    public partial class _default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            string membersPath = @"F:/Ali/CloudUnited/Datas/Members.xlsx";
            string directionsPath = @"F:/Ali/CloudUnited/Datas/Directions.xlsx";
            string servicesPath = @"F:/Ali/CloudUnited/Datas/Services.xlsx";

            loadMembers(membersPath);
            loadServices(servicesPath);
            loadDirections(directionsPath);

        }

        void loadMembers(string path)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open(path);
            Excel.Worksheet ws = wb.Worksheets[1];
            string cities = "cities=[";
            string members = "members=[";
            Excel.Range range = ws.UsedRange;

            for (int col = 1; col <= range.Rows[1].Cells.Count; col++)
            {
                string colText = range.Rows[1].Cells[col].Text;
                cities += "'" + colText + "'";
                cities += ",";
                members += "[";
                for (int i = 2; i <= range.Rows.Count; i++)
                {
                    if (range.Rows[i].Cells[col].Text == "") break;
                    members += "'" + range.Rows[i].Cells[col].Text + "'";
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

            wb.Close(false);
            excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

        void loadDirections(string path)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open(path);
            Excel.Worksheet ws = wb.Worksheets[1];
            Excel.Range range = ws.UsedRange;
            for (int i =2; i <= range.Rows.Count; i++)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "direction-panel");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.ID = range.Rows[i].Cells[3].Text;
                h3.InnerText= range.Rows[i].Cells[1].Text;
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.InnerText = range.Rows[i].Cells[2].Text;
                div.Controls.Add(h3);
                div.Controls.Add(p);
                directions.Controls.Add(div);
            }

            wb.Close(false);
            excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);


        }

        void loadServices(string path)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open(path);
            Excel.Worksheet ws = wb.Worksheets[1];
            Excel.Range range = ws.UsedRange;
            for (int i = 2; i <= range.Rows.Count; i++)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "service-panel");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.ID = range.Rows[i].Cells[3].Text;
                h3.InnerText = range.Rows[i].Cells[1].Text;
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.InnerText = range.Rows[i].Cells[2].Text;
                div.Controls.Add(h3);
                div.Controls.Add(p);
                services.Controls.Add(div);
            }

            wb.Close(false);
            excelApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

        }


    }

}
