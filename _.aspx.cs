using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

public partial class HierarchyTable : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string jsonData = GetJsonDataFromDatabase();
            Response.Write(jsonData);
        }
    }

    private string GetJsonDataFromDatabase()
    {
        string connectionString = "YourConnectionStringHere";
        string query = @"SELECT name, amount, created_at 
                         FROM your_table 
                         ORDER BY name, created_at DESC";

        var groupedData = new Dictionary<string, List<Dictionary<string, object>>>();
        var latestData = new List<Dictionary<string, object>>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["name"].ToString();
                int amount = Convert.ToInt32(reader["amount"]);
                DateTime createdAt = Convert.ToDateTime(reader["created_at"]);

                var rowData = new Dictionary<string, object>
                {
                    { "name", name },
                    { "amount", amount },
                    { "created_at", createdAt }
                };

                // Add to grouped data for historic purposes
                if (!groupedData.ContainsKey(name))
                    groupedData[name] = new List<Dictionary<string, object>>();

                groupedData[name].Add(rowData);

                // For latest entry, only keep the newest row per name
                if (!latestData.Exists(x => x["name"].ToString() == name))
                {
                    latestData.Add(rowData);
                }
            }
        }

        var jsonSerializer = new JavaScriptSerializer();
        return jsonSerializer.Serialize(new { latestData, historicData = groupedData });
    }
}
