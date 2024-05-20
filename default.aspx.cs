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

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await ShowListAsync();
            }
        }

        public async Task ShowListAsync()
        {
            DataTable vmInfos = new DataTable();
            DataTable vmExtras = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    // Update VMOwner to NULL if it's empty
                    cmd.CommandText = "UPDATE VMInfos2 SET VMOwner = NULL WHERE VMOwner ='' ";
                    await cmd.ExecuteNonQueryAsync();

                    // Select VMInfos2 data
                    cmd.CommandText = "SELECT VMName,vCenter,VMNumCPU,VMMemoryCapacity,VMTotalDisk,VMPowerState,VMCluster,VMDataCenter,VMOwner,VMCreatedDate FROM VMInfos2 ORDER BY CASE WHEN VMOwner IS NULL THEN 1 ELSE 0 END, VMName ASC";
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(vmInfos);
                    }

                    // Select VMsExtras data
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
                                newCell.InnerText = foundRow.Field<DateTime>("CreatedDate").ToString();
                            }
                        }

                        newRow.Cells.Add(newCell);
                        newRow.Style["display"] = "none";
                    }

                    tableBody.Controls.Add(newRow);
                    count++;
                    if (count > 100)
                    {
                        newRow.Attributes["class"] = "in-filter";
                    }
                }
            }
        }
    }
}
