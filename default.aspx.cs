using System;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;


namespace vminfo
{
    public partial class _default : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            showList();
        }
        public void showList()
        {

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE VMInfos2 SET VMOwner = NULL WHERE VMOwner ='' ";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"SELECT VMName,vCenter,VMNumCPU,VMMemoryCapacity,VMTotalDisk,VMPowerState,VMCluster,VMDataCenter,VMOwner,VMCreatedDate FROM VMInfos2 ORDER BY CASE WHEN VMOwner IS NULL THEN 1 ELSE 0 END, VMName ASC";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            cmd.CommandText = "SELECT CreatedDate,VMName FROM VMsExtras";
            cmd.ExecuteNonQuery();
            DataTable dtt = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd);
            da2.Fill(dtt);


            if (tableBody != null)
            {
                int count = 1;
                foreach (DataRow row in dt.Rows)
                {
                    HtmlTableRow newRow = new HtmlTableRow();
                    for (int j = 0; j < 10; j++)
                    {
                        HtmlTableCell newCell = new HtmlTableCell();
                        newCell.InnerText = row[j].ToString();
                        if (j == 9)
                        {
                            var foundRow = dtt.AsEnumerable().ToList().FindIndex(frow => frow[1].ToString() == row[0].ToString());
                            if (foundRow != -1)
                            {
                                newCell.InnerText = dtt.Rows[foundRow][0].ToString();
                            }
                        }
                        newRow.Cells.Add(newCell);
                        newRow.Style["display"] = "none";
                    }
                    tableBody.Controls.Add(newRow);
                    count++;
                    if (count > 100) newRow.Attributes["class"] = "in-filter";

                }

            }
        }
    }

}
