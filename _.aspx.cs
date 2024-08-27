using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text;

namespace vmpedia
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
                    cmd.CommandText = "SELECT VMName, vCenter, VMNumCPU, VMMemoryCapacity, VMTotalStorage, VMPowerState, VMCluster, VMDataCenter,VMGuestOS, VMOwner, VMCreatedDate FROM vminfoVMs";

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
        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue; 
                string[] vmNames = js.Deserialize<string[]>(jsonData);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM vminfoVMs WHERE VMName=";

                        //add here
                    }
                }
                StringBuilder csv = new StringBuilder();


                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=vmsData.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }

    }
}
