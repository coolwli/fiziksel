private async Task<List<Dictionary<string, object>>> FetchDatabaseTableDataAsync()
        {
            var tableData = new List<Dictionary<string, object>>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    const string query = "SELECT * FROM VMCpuAlerts WHERE " +
                        "[VMName] IN (SELECT DISTINCT[VMName]" +
                        "FROM VMCpuAlerts " +
                        "WHERE LastEmailSent > DATEADD(DAY, -7, GETDATE()) " +
                        "OR(NeverSentEmail = 1 AND LastEmailSent > DATEADD(DAY, -90, GETDATE())))";

                    using (var command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                row["VMName"] = reader["VMName"] as string;
                                row["vCenterName"] = reader["vCenterName"] as string;
                                row["CpuPercentage"] = reader["CpuPercentage"];
                                row["NeverSentEmail"] = reader["NeverSentEmail"];
                                row["Date"] = reader["LastEmailSent"];

                                tableData.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return tableData;
        }














private void GenerateAlarmControls(List<Dictionary<string, object>> alarmData)
        {
            // Clear previous controls
            phAlarms.Controls.Clear();

            int index = 0;
            foreach (var item in alarmData)
            {
                // Create table row
                var tableRow = new HtmlGenericControl("tr");

                // VM Name cell
                var vmNameCell = new HtmlGenericControl("td");
                vmNameCell.InnerText = item["VMName"]?.ToString() ?? "";
                tableRow.Controls.Add(vmNameCell);

                // vCenter cell
                var vCenterCell = new HtmlGenericControl("td");
                vCenterCell.InnerText = item["vCenterName"]?.ToString() ?? "";
                tableRow.Controls.Add(vCenterCell);

                // CPU Percentage cell
                var cpuCell = new HtmlGenericControl("td");
                cpuCell.InnerText = $"{item["CpuPercentage"]}%";
                tableRow.Controls.Add(cpuCell);

                var dateCell = new HtmlGenericControl("td");
                dateCell.InnerText = item["Date"]?.ToString() ?? "";
                tableRow.Controls.Add(dateCell);

                // Action cell with dropdown
                var actionCell = new HtmlGenericControl("td");
                var ddl = new DropDownList();
                ddl.ID = $"durumddl_{index}";
                ddl.CssClass = "alarm-dropdown";
                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += durumddl_SelectedIndexChanged;

                // Add default option
                ddl.Items.Add(new ListItem("1 Hafta Boyunca Uyarma", "0"));
                ddl.Items.Add(new ListItem("Bu Sunucuyu Asla Uyarma", "1"));

                // Set selected value based on NeverSentEmail status
                string neverSentEmail = item["NeverSentEmail"]?.ToString() ?? "0";
                if (neverSentEmail == "True")
                {
                    ddl.SelectedValue = "1"; // Bu Sunucuyu Asla Uyarma
                }
                else
                {
                    ddl.SelectedValue = "0"; // Default selection
                }

                // Store VM info in dropdown attributes for later use
                ddl.Attributes["data-vmname"] = item["VMName"]?.ToString() ?? "";

                actionCell.Controls.Add(ddl);
                tableRow.Controls.Add(actionCell);

                phAlarms.Controls.Add(tableRow);
                index++;
            }









    
