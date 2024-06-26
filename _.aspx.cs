using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace vminfo
{
    public partial class _default : System.Web.UI.Page
    {
        private string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowList();

        }

        public void ShowList()
        {
            DataTable vmInfos = new DataTable();

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

                }
            }
            string json = DataTableToJson(vmInfos);
            string script = $"<script> data = {json}; initializeTable(); screenName = 'vmscreen'; </script>";
            ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
            

        }

        private string DataTableToJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            return serializer.Serialize(rows);
        }
    }
}
