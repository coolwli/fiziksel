using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace windows_users
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderPath = @"F:\Ali\windows users";

            var csvFiles = Directory.GetFiles(folderPath, "*.csv");

            if (csvFiles.Length == 0)
            {
                Response.Write("csv bulunamadı");
                return;
            }

            string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            try
            {
                using (StreamReader reader = new StreamReader(latestFile))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        if (columns.Length > 0)
                        {
                            string[] cell = columns[0].Split('+');
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            for (int i = 0; i < cell.Length ; i++)
                            {
                                row.Add(cell[0],cell[1]);
                            }
                            rows.Add(row);
                        }
                    }
                }

                Response.Write(rows.Count+"-");
                Response.Write(rows[0].Count);
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
        }
    }
}
