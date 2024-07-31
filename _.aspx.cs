private void GetVMs(string[] hostNames)
{
    if (hostNames == null || hostNames.Length == 0)
        return;

    var query = "SELECT * FROM vminfoVMs WHERE VMName IN (@VMNames)";
    using (var con = new SqlConnection("YourConnectionString")) // Ensure you replace "YourConnectionString" with your actual connection string
    {
        con.Open();

        // Prepare the list of VM names, each enclosed in single quotes
        var formattedHostNames = hostNames.Select(name => $"'{name}'").ToArray();
        var vmNamesList = string.Join(",", formattedHostNames);

        // Using parameterized query to prevent SQL Injection
        using (var cmd = new SqlCommand(query.Replace("@VMNames", vmNamesList), con))
        {
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var vmsDataList = new List<string[]>();

                    while (reader.Read())
                    {
                        vmsDataList.Add(new string[]
                        {
                            reader["VMName"].ToString(),
                            reader["VMOwner"].ToString(),
                            reader["VMPowerState"].ToString(),
                            reader["VMTotalStorage"].ToString()
                        });
                    }

                    // Convert list to array before populating the table
                    var vmsData = vmsDataList.ToArray();
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

private void PopulateTable(HtmlTable table, string[][] rowsData)
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
