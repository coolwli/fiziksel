using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace HallAyrimliVMLists
{
    public partial class _default : System.Web.UI.Page
    {
        private const string CsvFilesPath = @"\\gbnas02\GTSunucuOrtakAlan";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<string[]> allRows = LoadCsvFiles(CsvFilesPath);
                string json = ConvertToJson(allRows);

                // Initialize data for JavaScript
                string script = $"<script>baseData = {json}; initializeTable();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
            }
            catch (IOException ioEx)
            {
                // Handle file I/O exceptions
                Response.Write($"File I/O Error: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Response.Write($"An error occurred: {ex.Message}");
            }
        }

        private List<string[]> LoadCsvFiles(string folderPath)
        {
            List<string[]> allRows = new List<string[]>();

            // Get all CSV files in the folder
            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

            foreach (var csvFilePath in csvFiles)
            {
                allRows.AddRange(ReadCsvFile(csvFilePath));
            }

            return allRows;
        }

        private IEnumerable<string[]> ReadCsvFile(string filePath)
        {
            List<string[]> rows = new List<string[]>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Skip the header row
                    reader.ReadLine();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            // Parse each row and clean the data
                            string[] columns = line.Split(',')
                                .Select(col => col.Replace("\"", "").Trim())
                                .Select(col => string.IsNullOrWhiteSpace(col) ? "" : col)
                                .ToArray();

                            rows.Add(columns);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new IOException($"Error reading file {filePath}: {ex.Message}", ex);
            }

            return rows;
        }

        private string ConvertToJson(List<string[]> data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            return serializer.Serialize(data);
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var tableData = DeserializeJson(jsonData);

                    if (tableData.Count == 0)
                    {
                        Response.Write("No data available.");
                        return;
                    }

                    string csvContent = GenerateCsvContent(tableData);

                    // Send CSV file to user
                    SendCsvToClient(csvContent);
                }
                catch (Exception ex)
                {
                    Response.Write($"An error occurred: {ex.Message}");
                }
            }
        }

        private List<string[]> DeserializeJson(string jsonData)
        {
            JavaScriptSerializer js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return js.Deserialize<List<string[]>>(jsonData);
        }

        private string GenerateCsvContent(List<string[]> tableData)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Name;ID;Created Date;Cluster;vCenter;User Name");

            foreach (var row in tableData)
            {
                csv.AppendLine(string.Join(";", row));
            }

            return csv.ToString();
        }

        private void SendCsvToClient(string csvContent)
        {
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=createdVMs.csv");
            Response.Write(csvContent);
            Response.End();
        }
    }
}
