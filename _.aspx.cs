
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;

namespace historicDatas
{
    public partial class _default : System.Web.UI.Page
    {
        private readonly string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sqlQuery = "SELECT * FROM fiziksel_historic";
                List<string> dates;
                Dictionary<string, Dictionary<string, List<int>>> datasets;

                GetAllDatasets(sqlQuery, out dates, out datasets);
            }
        }

        private void GetAllDatasets(string query, out List<string> dates, out Dictionary<string, Dictionary<string, List<int>>> datasets)
        {
            dates = new List<string>();
            datasets = new Dictionary<string, Dictionary<string, List<int>>>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProcessRow(reader, datasets, dates);
                        }
                    }
                }
            }
        }

        private void ProcessRow(SqlDataReader reader, Dictionary<string, Dictionary<string, List<int>>> datasets, List<string> dates)
        {
            dates.Add(reader[0].ToString());

            AddFixedValues(datasets, reader);

            ProcessDynamicValues(reader, datasets);
        }

        private void AddFixedValues(Dictionary<string, Dictionary<string, List<int>>> datasets, SqlDataReader reader)
        {
            AddValue(datasets, "Total CPU", "Total CPU", Convert.ToInt32(reader["Total CPU"])); // Adjust column name
            AddValue(datasets, "Total Memory", "Total Memory", Convert.ToInt32(reader["Total Memory"])); // Adjust column name
            AddValue(datasets, "Host Count", "Host Count", Convert.ToInt32(reader["Host Count"])); // Adjust column name
        }

        private void AddValue(Dictionary<string, Dictionary<string, List<int>>> datasets, string key, string columnName, int value)
        {
            if (!datasets.ContainsKey(key))
            {
                datasets[key] = new Dictionary<string, List<int>>();
            }

            if (!datasets[key].ContainsKey(columnName))
            {
                datasets[key][columnName] = new List<int>();
            }

            datasets[key][columnName].Add(value);
        }

        private void ProcessDynamicValues(SqlDataReader reader, Dictionary<string, Dictionary<string, List<int>>> datasets)
        {
            for (int i = 1; i < reader.FieldCount; i++) // Assuming the first column is the date
            {
                var columnName = reader.GetName(i);
                var value = reader.GetValue(i).ToString();
                ParseDynamicValues(value, columnName, datasets);
            }
        }

        private void ParseDynamicValues(string value, string columnName, Dictionary<string, Dictionary<string, List<int>>> datasets)
        {
            var entries = value.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var parts = entry.Split('|');

                if (parts.Length == 2 && int.TryParse(parts[1], out int number))
                {
                    var company = parts[0].Trim();
                    AddValue(datasets, company, columnName, number);
                }
            }
        }

    }
}
