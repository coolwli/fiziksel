private void GetVMs(string[] hostNames)
{
    if (hostNames == null || hostNames.Length == 0)
        return;

    var query = "SELECT * FROM vminfoVMs WHERE VMName IN (@VMNames)";
    using (var con = new SqlConnection("YourConnectionString")) // Ensure you replace "YourConnectionString" with your actual connection string
    {
        con.Open();

        // Using parameterized query to prevent SQL Injection
        using (var cmd = new SqlCommand(query, con))
        {
            var parameter = new SqlParameter("@VMNames", SqlDbType.VarChar);
            parameter.Value = string.Join(",", hostNames); // Assuming VMNames are a comma-separated list. Adjust if necessary.
            cmd.Parameters.Add(parameter);

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var vmsData = new List<string[]>();

                    while (reader.Read())
                    {
                        vmsData.Add(new string[]
                        {
                            reader["VMName"].ToString(),
                            reader["VMOwner"].ToString(),
                            reader["VMPowerState"].ToString(),
                            reader["VMTotalStorage"].ToString()
                        });
                    }

                    // Populate table after fetching all data
                    PopulateTable(vmsTable, vmsData);
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it accordingly
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

private void PopulateTable(HtmlTable table, List<string[]> rowsData)
{
    foreach (var rowData in rowsData)
    {
        var row = new HtmlTableRow();
        foreach (var cellData in rowData)
        {
            row.Cells.Add(new HtmlTableCell { InnerText = cellData });
        }
        table.Rows.Add(row);
    }
}
