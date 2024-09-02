            document.getElementById('export-button').addEventListener('click', () => {
                event.preventDefault();
                const hdnInput = document.getElementById('hiddenField');
                const vmNames = filteredData.map(row => row.VMName);

                var jsondata = JSON.stringify(vmNames);
                hdnInput.value = jsondata;
                console.log(vmNames);
                document.getElementById('hiddenButton').click();
            });



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

            var csv = new StringBuilder();
            int batchSize = 2000; 
            for (int batchStart = 0; batchStart < vmNames.Length; batchStart += batchSize)
            {
                var batch = vmNames.Skip(batchStart).Take(batchSize).ToArray();

                var queryBuilder = new StringBuilder("SELECT * FROM vminfoVMs WHERE VMName IN (");
                for (int i = 0; i < batch.Length; i++)
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
                        for (int i = 0; i < batch.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@name" + i, batch[i]);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (batchStart == 0)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i > 0) csv.Append(";");
                                    csv.Append(reader.GetName(i));
                                }
                                csv.AppendLine();
                            }

                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i > 0) csv.Append(";");
                                    csv.Append(reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString());
                                }
                                csv.AppendLine();
                            }
                        }
                    }
                }
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF32;
            Response.AddHeader("content-disposition", "attachment;filename=vmpedia_vms.csv");
            Response.Write(csv.ToString());
            Response.End();
        }
