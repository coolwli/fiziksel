using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace vminfo
{
    public partial class _default : System.Web.UI.Page
    {
        private readonly string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowList();
            }
        }

        public void ShowList()
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

            string jsArray = "data = [";
            foreach (DataRow row in vmInfos.Rows)
            {
                jsArray += "{";
                jsArray += $"Name: '{row["VMName"]}',";
                jsArray += $"vCenter: '{row["vCenter"]}',";
                jsArray += $"CPU: '{row["VMNumCPU"]}',";
                jsArray += $"Memory: '{row["VMMemoryCapacity"]}',";
                jsArray += $"TotalDisk: '{row["VMTotalDisk"]}',";
                jsArray += $"PowerState: '{row["VMPowerState"]}',";
                jsArray += $"Cluster: '{row["VMCluster"]}',";
                jsArray += $"DataCenter: '{row["VMDataCenter"]}',";
                jsArray += $"Owner: '{row["VMOwner"]}',";
                jsArray += $"CreatedDate: '{row["VMCreatedDate"]}',";
                jsArray += "},";
            }
            jsArray = jsArray.TrimEnd(',');
            jsArray += "]; initializeTable();";

            ClientScript.RegisterStartupScript(this.GetType(), "initializeData", jsArray, true);
        }
    }
}
