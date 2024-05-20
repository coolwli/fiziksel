using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace vminfo
{
    public partial class _default : System.Web.UI.Page
    {
        private readonly string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            
            ShowListAsync();
            
        }

        public void ShowListAsync()
        {
            DataTable vmInfos = new DataTable();
            DataTable vmExtras = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT VMName,vCenter,VMNumCPU,VMMemoryCapacity,VMTotalDisk,VMPowerState,VMCluster,VMDataCenter,VMOwner,VMCreatedDate FROM VMInfos2";
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(vmInfos);
                    }

                    cmd.CommandText = "SELECT CreatedDate,VMName FROM VMsExtras";
                    using (SqlDataAdapter da2 = new SqlDataAdapter(cmd))
                    {
                        da2.Fill(vmExtras);
                    }
                }
            }

            if (tableBody != null)
            {
                int count = 1;
                foreach (DataRow row in vmInfos.Rows)
                {
                    HtmlTableRow newRow = new HtmlTableRow();
                    for (int j = 0; j < 10; j++)
                    {
                        HtmlTableCell newCell = new HtmlTableCell
                        {
                            InnerText = row[j].ToString()
                        };

                        if (j == 9)
                        {
                            var foundRow = vmExtras.AsEnumerable().FirstOrDefault(frow => frow.Field<string>("VMName") == row.Field<string>("VMName"));
                            if (foundRow != null)
                            {
                                newCell.InnerText = foundRow["CreatedDate"].ToString();
                            }
                        }

                        newRow.Cells.Add(newCell);
                        newRow.Style["display"] = "none";
                        newRow.Attributes["class"] = "in-filter";

                    }

                    tableBody.Controls.Add(newRow);
                    count++;

                }
            }
        }
    }
}
