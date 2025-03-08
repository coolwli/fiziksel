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
        protected void Page_Load(object sender, EventArgs e)
        {

            string csvFilesPath = @"\\gbnas02\GTSunucuOrtakAlan";

            try
            {
                List<string[]> rows = new List<string[]>();

                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    string line;

                    reader.ReadLine();

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] columns = line.Split(',')
                                .Select(col => col.Replace("\"", "").Trim())
                                .Select(col => string.IsNullOrWhiteSpace(col) ? "" : col)
                                .ToArray();
                            rows.Add(columns);
                        }
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue
                };
                string json = serializer.Serialize(rows);

                string script = $"<script>baseData = {json}; initializeTable();</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
        }

        protected void hiddenButton_Click(object sender, EventArgs e)
        {
            string jsonData = hiddenField.Value;

            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer
                    {
                        MaxJsonLength = int.MaxValue
                    };

                    var tableData = js.Deserialize<List<string[]>>(jsonData);

                    if (tableData.Count == 0)
                    {
                        Response.Write("No data available.");
                        return;
                    }

                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("Name;ID;Created Date;Cluster;vCenter;User Name");
                    foreach (var row in tableData)
                    {
                        csv.AppendLine($"{row[0]};{row[1]};{row[2]};{row[3]};{row[4]};{row[5]}");
                    }

                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", "attachment;filename=createdVMs.csv");
                    Response.Write(csv.ToString());
                    Response.End();
                }
                catch (Exception ex)
                {
                    Response.Write("Hata: " + ex.Message);
                }
            }
        }
    }
}
