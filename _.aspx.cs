protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue; // Maksimum JSON uzunluğunu ayarla
                var tableData = js.Deserialize<List<string[]>>(jsonData);

                if (tableData.Count == 0)
                {
                    Response.Write("No data available.");
                    return;
                }

                // Kolon başlıklarını belirle
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Host,User");

                // Her bir satırı CSV formatına dönüştür
                foreach (var row in tableData)
                {
                    if (row.Length >= 2)
                    {
                        csv.AppendLine($"{row[0]},{row[1]}");
                    }
                    else
                    {
                        csv.AppendLine($"{row[0]},"); // User bilgisi yoksa boş bırak
                    }
                }

                // CSV'yi kullanıcıya sun
                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", "attachment;filename=vmsData.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }
