using System;
using System.Net;
using System.Xml;
using System.Text;
using System.Linq;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Net.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace vminfo
{
    public partial class clusterscreen : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        public string hostName;
        protected void Page_Load(object sender, EventArgs e)
        {

            hostName = Request.QueryString["id"];
            if (string.IsNullOrEmpty(hostName))
            {
                DisplayHostNameError();
                return;
            }

            InitializeDatabaseConnection();

            if (!IsPostBack)
            {
                ShowHost();
            }
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = @"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True";
            con = new SqlConnection(connectionString);
        }


        private void DisplayHostNameError()
        {
            form1.InnerHtml = "";
            Response.Write("ID Bulunamadı");
        }

        private void ShowHost()
        {
            string query = "SELECT * FROM vminfoClusters WHERE ClusterName = @name";
            con.Open();
            using (var command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@name", hostName);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PopulateDetails(reader);
                    }
                    else
                    {
                        DisplayHostNameError();
                    }
                }
            }
            con.Close();
        }



        private void PopulateDetails(SqlDataReader reader)
        {
            vcenter.InnerText = reader["vCenter"].ToString();
            datacenter.InnerText = reader["DataCenter"].ToString();
            hae.InnerText = reader["ClusterHAE"].ToString();
            drs.InnerText = reader["ClusterDRSEnabled"].ToString();
            vmotion.InnerText = Convert.ToInt32(reader["ClusterNumVmotions"]).ToString("N0");
            hostcount.InnerText = reader["ClusterNumHosts"].ToString();
            cpucore.InnerText = reader["ClusterNumCPUCores"].ToString();
            vmcount.InnerText = reader["TotalVMCount"].ToString();
            lasttime.InnerText = reader["LastWriteTime"].ToString();
            totalDisk.InnerText = "Total Disk " + Convert.ToDouble(reader["ClusterTotalStorage"]).ToString("N2") + " GB";
            freeDisk.InnerHtml = "Free Disk: <strong>" + Convert.ToDouble(reader["ClusterFreeStorage"]).ToString("N2") + "GB </strong >";
            double usedStorage = (Convert.ToDouble(reader["ClusterTotalStorage"]) - Convert.ToDouble(reader["ClusterFreeStorage"]));
            usedDisk.InnerHtml = "Used Disk: <strong>" + usedStorage.ToString("N2") + "GB </strong >";
            usedPercentage.Style["width"] = (usedStorage / (Convert.ToDouble(reader["ClusterTotalStorage"])) * 100).ToString() + "%";
            usedBar.InnerText = (usedStorage / (Convert.ToDouble(reader["ClusterTotalStorage"])) * 100).ToString("F2") + "%";
            totalCpu.InnerText = "Total CPU " + reader["ClusterTotalCpu"].ToString() + "GHz ";
            totalMemory.InnerText = "Total Memory " + reader["ClusterTotalMemory"].ToString() + " GB";

            PopulateTable(eventsTable, reader["ClusterEventDates"].ToString(), reader["ClusterEventMessages"].ToString());
            PopulateTable(dsTable, reader["DSName"].ToString(), reader["DSCapacity"].ToString(), reader["DSFree"].ToString(), reader["DSPercentUsing"].ToString());
            string[] hostNames = reader["VMHostNames"].ToString().Split('~');
            reader.Close();
            getHosts(hostNames);
        }
        private void getHosts(string[] hostNames)
        {
            double hostUsedCpu = 0;
            double hostUsedMem = 0;
            foreach (string name in hostNames)
            {
                string queryString = $"SELECT * FROM vminfoHosts WHERE HostName = '{name}'";
                using (SqlCommand cmd = new SqlCommand(queryString, con))
                {
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hostUsedCpu += Convert.ToDouble(reader["HostCPUUsage"]);
                            hostUsedMem += Convert.ToDouble(reader["HostMemoryUsage"]);
                            PopulateTable(hostsTable, reader["HostName"].ToString(), reader["HostModel"].ToString(), reader["VMsCount"].ToString(), reader["HostTotalMemory"].ToString(), reader["HostTotalCPU"].ToString());
                        }
                        reader.Close();

                    }
                }
            }
            usedCpu.InnerHtml = "Used CPU: <strong>" + hostUsedCpu.ToString("N2") + "GHz  </strong >";
            freeCpu.InnerHtml = "Free CPU: <strong>" + (Double.Parse(totalCpu.InnerText.Split(' ')[2])-hostUsedCpu).ToString("N2") + "GHz  </strong >";
            double perc = hostUsedCpu / Convert.ToDouble(totalCpu.InnerText.Split(' ')[2])*100;
            cpuPercentage.Style["width"] = perc .ToString() + "%";
            cpuBar.InnerText = perc.ToString("N2") + "%";

            usedMemory.InnerHtml = "Used Memory: <strong>" + hostUsedMem.ToString("N2") + "GB </strong >";
            freeMemory.InnerHtml = "Free Memory: <strong>" + (Convert.ToDouble(totalMemory.InnerText.Split(' ')[2]) - hostUsedMem).ToString("N2") + "GB </strong >";
            perc = hostUsedMem/ Convert.ToDouble(totalMemory.InnerText.Split(' ')[2])*100;
            memoryPercentage.Style["width"] = perc.ToString() + "%";
            memoryBar.InnerText = perc.ToString("N2") + "%";

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

        protected void exportToExcelClick(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM ClusterInfos WHERE ClusterName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataColumn col in dt.Columns)
            {
                string[] keys = dt.Rows[0][col].ToString().Split('~');
                for (int i = 1; i < keys.Length; i++)
                {
                    if (dt.Rows.Count < keys.Length)
                        dt.Rows.Add();
                    dt.Rows[i][col.ColumnName] = keys[i - 1].Trim();
                }
                dt.Rows[0][col] = keys[0];
            }

            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", ("attachment;filename=" + hostName + ".xls"));

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    dg.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }

            }


        }

    }
}
