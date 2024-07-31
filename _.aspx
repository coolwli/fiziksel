        private void getVMs(string[] hostNames)
        {
            foreach (string name in hostNames)
            {
                string queryString = $"SELECT * FROM vminfoVMs WHERE VMName = '{name}'";
                using (SqlCommand cmd = new SqlCommand(queryString, con))
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateTable(vmsTable, reader["VMName"].ToString(), reader["VMOwner"].ToString(), reader["VMPowerState"].ToString(), reader["VMTotalStorage"].ToString());
                        }
                        reader.Close();

                    }
                }
            }
        }

        private void PopulateTable(HtmlTable table, params string[] columns)
        {
            var columnData = columns.Select(c => c.Split('~')).ToArray();
            int rowCount = columnData[0].Length;
            for (int i = 0; i < rowCount; i++)
            {
                if (columnData[0][i].Length == 0)
                    return;
                var row = new HtmlTableRow();
                for (int j = 0; j < columnData.Length; j++)
                {
                    row.Cells.Add(new HtmlTableCell { InnerText = columnData[j][i] });
                }
                table.Rows.Add(row);
            }
        }
