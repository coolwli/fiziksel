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
            Show_List();

        }
        public void Show_List()
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT VMName, vCenter, VMNumCPU, VMMemoryCapacity, VMTotalStorage, VMPowerState, VMCluster, VMDataCenter, VMOwner, VMCreatedDate FROM vminfoVMs";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            rows.Add(row);
                        }
                    }
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string json = serializer.Serialize(rows);
            string script = $"<script> data = {json}; initializeTable(); screenName = 'vmscreen'; </script>";
            ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);

        }
    }
}
