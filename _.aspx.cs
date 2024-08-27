using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace vmpedia
{
    public partial class _default : System.Web.UI.Page
    {
        private readonly string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Show_List();
            }
        }

        public void Show_List()
        {
            var rows = new List<Dictionary<string, object>>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand("SELECT VMName, vCenter, VMNumCPU, VMMemoryCapacity, VMTotalStorage, VMPowerState, VMCluster, VMDataCenter, VMGuestOS, VMOwner, VMCreatedDate FROM vminfoVMs", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            rows.Add(row);
                        }
                    }
                }
            }

            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            var json = serializer.Serialize(rows);
            var script = $"<script>data = {json}; initializeTable(); screenName = 'vmscreen';</script>";
            ClientScript.RegisterStartupScript(GetType(), "initializeData", script);
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            var jsonData = hiddenField.Value;

            if (string.IsNullOrEmpty(jsonData))
            {
                Response.Write("No data received.");
                Response.End();
                return;
            }

            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            var vmNames = serializer.Deserialize<string[]>(jsonData);

            if (vmNames.Length == 0)
            {
                Response.Write("No VM names provided.");
                Response.End();
                return;
            }

            var queryBuilder = new StringBuilder("SELECT * FROM vminfoVMs WHERE VMName IN (");
            for (int i = 0; i < vmNames.Length; i++)
            {
                if (i > 0) queryBuilder.Append(", ");
                queryBuilder.Append("@name" + i);
            }
            queryBuilder.Append(")");

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand(queryBuilder.ToString(), con))
                {
                    for (int i = 0; i < vmNames.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@name" + i, vmNames[i]);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        var csv = new StringBuilder();

                        // Append the header row
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i > 0) csv.Append(",");
                            csv.Append(reader.GetName(i));
                        }
                        csv.AppendLine();

                        // Append data rows
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (i > 0) csv.Append(",");
                                csv.Append(reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString());
                            }
                            csv.AppendLine();
                        }

                        Response.Clear();
                        Response.ContentType = "text/csv";
                        Response.AddHeader("content-disposition", "attachment;filename=vmsData.csv");
                        Response.Write(csv.ToString());
                        Response.End();
                    }
                }
            }
        }
    }
}
