using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Data.SqlClient;

namespace vminfo
{
    public partial class vmScreen : System.Web.UI.Page
    {
        private readonly SqlConnection _con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        private string _hostName;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _hostName = Request.QueryString["id"];
                if (string.IsNullOrEmpty(_hostName))
                {
                    // Handle missing or invalid ID here
                    return;
                }

                if (_con.State == ConnectionState.Open)
                {
                    _con.Close();
                }

                _con.Open();
                ShowHost();
            }
        }

        private void ShowHost()
        {
            DataTable dt = new DataTable();
            using (SqlCommand command = new SqlCommand("SELECT * FROM VMInfos2 WHERE VMName = @name", _con))
            {
                command.Parameters.AddWithValue("@name", _hostName);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PopulateHostDetails(reader);
                    }
                }
            }

            ShowNetwork();
            ShowDisks();
            ShowEvents();
        }

        private void PopulateHostDetails(SqlDataReader reader)
        {
            vcenter.InnerText = reader["vCenter"].ToString();
            host.InnerText = reader["VMHost"].ToString();
            cluster.InnerText = reader["VMCluster"].ToString();
            datacenter.InnerText = reader["VMDataCenter"].ToString();
            power.InnerText = reader["VMPowerState"].ToString();
            notes.InnerText = reader["VMNotes"].ToString();
            numcpu.InnerText = reader["VMNumCPU"].ToString();
            corespersocket.InnerText = reader["VMCoresPerSocket"].ToString();
            totalmemory.InnerText = reader["VMMemoryCapacity"].ToString();
            totaldisk.InnerText = reader["VMTotalDisk"].ToString();
            averageDisk.InnerText = reader["VMAverageDiskUsage"].ToString() + " KBps";
            averageCpu.InnerText = reader["VMAverageCPUUsage"].ToString() + "%";
            averageMemory.InnerText = reader["VMAverageMemoryUsage"].ToString() + "%";
            averageNetwork.InnerText = reader["VMAverageNetworkUsage"].ToString() + " KBps";
            lasttime.InnerText = reader["LastWriteTime"].ToString();
            hostmodel.InnerText = reader["VMHostModel"].ToString();
            os.InnerText = reader["VMGuestOS"].ToString();
            string tempCreatedDate = reader["VMCreatedDate"].ToString();

            PopulateCreatedDate(tempCreatedDate);
        }

        private void PopulateCreatedDate(string tempCreatedDate)
        {
            using (SqlCommand command = new SqlCommand("SELECT CreatedDate FROM VMsExtras WHERE VMName = @name", _con))
            {
                command.Parameters.AddWithValue("@name", _hostName);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        createdDate.InnerText = reader["CreatedDate"].ToString();
                    }
                    else
                    {
                        createdDate.InnerText = tempCreatedDate;
                    }
                }
            }
        }

        private void ShowEvents()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT VMEventDates, VMEventMessages FROM VmInfos2 WHERE VMName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] eventDates = reader["VMEventDates"].ToString().Split('~');
                        string[] eventMessages = reader["VMEventMessages"].ToString().Split('~');

                        for (int i = 0; i < eventDates.Length - 1; i++)
                        {
                            HtmlTableRow newRow = new HtmlTableRow();
                            newRow.Cells.Add(new HtmlTableCell { InnerText = eventDates[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = eventMessages[i] });
                            eventsTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        private void ShowNetwork()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT VMNetworkAdapterName, VMNetworkName, VMNetworkCardType, VMNetworkAdapterMacAddress FROM VMInfos2 WHERE VMName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] adapterNames = reader["VMNetworkAdapterName"].ToString().Split('~');
                        string[] networkNames = reader["VMNetworkName"].ToString().Split('~');
                        string[] macAddresses = reader["VMNetworkAdapterMacAddress"].ToString().Split('~');
                        string[] networkCardTypes = reader["VMNetworkCardType"].ToString().Split('~');

                        for (int i = 0; i < adapterNames.Length - 1; i++)
                        {
                            HtmlTableRow newRow = new HtmlTableRow();
                            newRow.Cells.Add(new HtmlTableCell { InnerText = adapterNames[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = networkNames[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = macAddresses[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = networkCardTypes[i] });
                            networkTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        private void ShowDisks()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT VMDiskName, VMDiskTotalSize, VMDiskStorageFormat, VMDiskDataStore FROM VMInfos2 WHERE VMName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] diskNames = reader["VMDiskName"].ToString().Split('~');
                        string[] totalSizes = reader["VMDiskTotalSize"].ToString().Split('~');
                        string[] formats = reader["VMDiskStorageFormat"].ToString().Split('~');
                        string[] dataStores = reader["VMDiskDataStore"].ToString().Split('~');

                        for (int i = 0; i < diskNames.Length - 1; i++)
                        {
                            HtmlTableRow newRow = new HtmlTableRow();
                            newRow.Cells.Add(new HtmlTableCell { InnerText = diskNames[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = totalSizes[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = formats[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = dataStores[i] });
                            disksTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        protected void ExportToExcelClick(object sender, EventArgs e)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VMInfos2 WHERE VMName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                ExpandDataTable(dt);

                DataGrid dg = new DataGrid { DataSource = dt };
                dg.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={_hostName}.xls");

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

        private void ExpandDataTable(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                string[] keys = dt.Rows[0][col].ToString().Split('~');
                for (int i = 1; i < keys.Length; i++)
                {
                    if (dt.Rows.Count < keys.Length)
                    {
                        dt.Rows.Add();
                    }
                    dt.Rows[i][col.ColumnName] = keys[i - 1].Trim();
                }
                dt.Rows[0][col] = keys[0];
            }
        }
    }
}
