private async Task<Dictionary<string, Dictionary<string, object>>> FetchDatabaseTableDataAsync()
{
    var groupedData = new Dictionary<string, Dictionary<string, object>>();

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            const string query = @"
                SELECT VMName, vCenterName, NeverSentEmail, CpuPercentage, LastEmailSent
                FROM VMCpuAlerts 
                WHERE VMName IN (
                    SELECT DISTINCT VMName
                    FROM VMCpuAlerts
                    WHERE LastEmailSent > DATEADD(DAY, -7, GETDATE()) 
                    OR (NeverSentEmail = 1 AND LastEmailSent > DATEADD(DAY, -90, GETDATE()))
                )
                ORDER BY VMName, LastEmailSent DESC";

            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var vmName = reader["VMName"] as string;

                        if (!groupedData.ContainsKey(vmName))
                        {
                            groupedData[vmName] = new Dictionary<string, object>
                            {
                                ["vCenterName"] = reader["vCenterName"] as string,
                                ["NeverSentEmail"] = (bool)reader["NeverSentEmail"],
                                ["Alerts"] = new List<Dictionary<string, object>>() // iç liste
                            };
                        }

                        var alertsList = groupedData[vmName]["Alerts"] as List<Dictionary<string, object>>;

                        alertsList.Add(new Dictionary<string, object>
                        {
                            ["CpuPercentage"] = reader["CpuPercentage"],
                            ["Date"] = reader["LastEmailSent"]
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        LogError(ex);
    }

    return groupedData;
}









private void GenerateAlarmControls(Dictionary<string, Dictionary<string, object>> alarmData)
{
    // Clear previous controls
    phAlarms.Controls.Clear();

    int index = 0;
    foreach (var kvp in alarmData)
    {
        string vmName = kvp.Key;
        var data = kvp.Value;

        // Main row
        var mainRow = new HtmlGenericControl("tr");
        mainRow.Attributes["class"] = "main-row";

        // VM Name cell
        var vmNameCell = new HtmlGenericControl("td");
        vmNameCell.InnerText = vmName;
        mainRow.Controls.Add(vmNameCell);

        // vCenter cell
        var vCenterCell = new HtmlGenericControl("td");
        vCenterCell.InnerText = data["vCenterName"]?.ToString() ?? "";
        mainRow.Controls.Add(vCenterCell);

        // Last CPU (latest alert)
        var alerts = data["Alerts"] as List<Dictionary<string, object>>;
        var latestAlert = alerts?.FirstOrDefault();

        var cpuCell = new HtmlGenericControl("td");
        cpuCell.InnerText = latestAlert != null ? $"{latestAlert["CpuPercentage"]}%" : "-";
        mainRow.Controls.Add(cpuCell);

        var dateCell = new HtmlGenericControl("td");
        dateCell.InnerText = latestAlert != null ? latestAlert["Date"]?.ToString() : "-";
        mainRow.Controls.Add(dateCell);

        // Dropdown cell
        var actionCell = new HtmlGenericControl("td");
        var ddl = new DropDownList();
        ddl.ID = $"durumddl_{index}";
        ddl.CssClass = "alarm-dropdown";
        ddl.AutoPostBack = true;
        ddl.SelectedIndexChanged += durumddl_SelectedIndexChanged;

        ddl.Items.Add(new ListItem("1 Hafta Boyunca Uyarma", "0"));
        ddl.Items.Add(new ListItem("Bu Sunucuyu Asla Uyarma", "1"));

        string neverSentEmail = data["NeverSentEmail"]?.ToString() ?? "0";
        ddl.SelectedValue = neverSentEmail == "True" ? "1" : "0";
        ddl.Attributes["data-vmname"] = vmName;

        actionCell.Controls.Add(ddl);
        mainRow.Controls.Add(actionCell);

        phAlarms.Controls.Add(mainRow);

        // Hidden details row
        foreach (var alert in alerts.Skip(1)) // ilk alert zaten yukarda gösterildi
        {
            var alertRow = new HtmlGenericControl("tr");
            alertRow.Attributes["class"] = "alert-detail-row";
            alertRow.Style["display"] = "none"; // varsayılan olarak gizli

            // Empty cells to align under VM
            for (int i = 0; i < 2; i++)
            {
                alertRow.Controls.Add(new HtmlGenericControl("td")); // boş hücreler
            }

            var cpuDetailCell = new HtmlGenericControl("td");
            cpuDetailCell.InnerText = $"{alert["CpuPercentage"]}%";
            alertRow.Controls.Add(cpuDetailCell);

            var dateDetailCell = new HtmlGenericControl("td");
            dateDetailCell.InnerText = alert["Date"]?.ToString();
            alertRow.Controls.Add(dateDetailCell);

            // Empty cell for dropdown
            alertRow.Controls.Add(new HtmlGenericControl("td"));

            phAlarms.Controls.Add(alertRow);
        }

        index++;
    }
}


<script>
    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".main-row").forEach(function (row) {
            row.style.cursor = "pointer";
            row.addEventListener("click", function () {
                let next = row.nextElementSibling;
                while (next && next.classList.contains("alert-detail-row")) {
                    next.style.display = next.style.display === "none" ? "table-row" : "none";
                    next = next.nextElementSibling;
                }
            });
        });
    });
</script>








    
