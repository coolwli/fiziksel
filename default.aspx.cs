using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Data.SqlClient;

namespace vminfo
{
    public partial class vmhostScreen : System.Web.UI.Page
    {
        private readonly SqlConnection _con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        private string _hostName;

        protected void Page_Load(object sender, EventArgs e)
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

        private void ShowHost()
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM VMHostInfos WHERE HostName = @name", _con))
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

            ShowVMs();
            ShowCpus();
            ShowDss();
            ShowEvents();
        }

        private void PopulateHostDetails(SqlDataReader reader)
        {
            vcenter.InnerText = reader["vCenter"].ToString();
            cluster.InnerText = reader["HostCluster"].ToString();
            datacenter.InnerText = reader["HostDataCenter"].ToString();
            manufacturer.InnerText = reader["HostManufacturer"].ToString();
            hostmodel.InnerText = reader["HostModel"].ToString();
            memorysize.InnerText = reader["HostMemorySize"].ToString();
            cpucore.InnerText = reader["HostNumCPUCores"].ToString();
            cpumhz.InnerText = reader["HostCPUMhz"].ToString();
            hostnic.InnerText = reader["HostNumNics"].ToString();
            hbas.InnerText = reader["HostHBAs"].ToString();
            avcpu.InnerText = reader["averageCpu"].ToString() + "%";
            avmem.InnerText = reader["averageMemory"].ToString() + "%";
            avdisk.InnerText = reader["averageDisk"].ToString() + " KBps";
            avnet.InnerText = reader["averageNetwork"].ToString() + " KBps";
            lasttime.InnerText = reader["LastWriteTime"].ToString();
            uptimedays.InnerText = reader["uptimeDay"].ToString() + " Day";
        }

        private void ShowCpus()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT CPUIndex, CPUDesc FROM VMHostInfos WHERE HostName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] cpuIds = reader["CPUIndex"].ToString().Split('~');
                        string[] cpuNames = reader["CPUDesc"].ToString().Split('~');

                        for (int i = 0; i < cpuIds.Length - 1; i++)
                        {
                            HtmlTableRow newRow = new HtmlTableRow();
                            newRow.Cells.Add(new HtmlTableCell { InnerText = cpuIds[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = cpuNames[i] });
                            cpuTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        private void ShowEvents()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT eventDate, eventMessage FROM VMHostInfos WHERE HostName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] eventDates = reader["eventDate"].ToString().Split('~');
                        string[] eventMessages = reader["eventMessage"].ToString().Split('~');

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

        private void ShowVMs()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT vmsName FROM VMHostInfos WHERE HostName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] vmNames = reader["vmsName"].ToString().Split('~');

                        foreach (string vmName in vmNames)
                        {
                            if (!string.IsNullOrWhiteSpace(vmName))
                            {
                                ShowVmDetails(vmName);
                            }
                        }
                    }
                }
            }
        }

        private void ShowVmDetails(string vmName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT VMName, VMOwner, VMPowerState, VMTotalDisk FROM VMInfos2 WHERE VMName = @vmName", _con))
            {
                cmd.Parameters.AddWithValue("@vmName", vmName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HtmlTableRow newRow = new HtmlTableRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            HtmlTableCell newCell = new HtmlTableCell { InnerText = reader[i].ToString() };
                            newRow.Cells.Add(newCell);
                        }
                        vmsTable.Rows.Add(newRow);
                    }
                }
            }
        }

        private void ShowDss()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT dsName, dsCapacity, dsFree, dsPercentUsing FROM VMHostInfos WHERE HostName = @name", _con))
            {
                cmd.Parameters.AddWithValue("@name", _hostName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] dsNames = reader["dsName"].ToString().Split('~');
                        string[] dsCapacities = reader["dsCapacity"].ToString().Split('~');
                        string[] dsFrees = reader["dsFree"].ToString().Split('~');
                        string[] dsPercentUsings = reader["dsPercentUsing"].ToString().Split('~');

                        for (int i = 0; i < dsNames.Length - 1; i++)
                        {
                            HtmlTableRow newRow = new HtmlTableRow();
                            newRow.Cells.Add(new HtmlTableCell { InnerText = dsNames[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = dsCapacities[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = dsFrees[i] });
                            newRow.Cells.Add(new HtmlTableCell { InnerText = "%" + dsPercentUsings[i] });
                            dsTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        protected void ExportToExcelClick(object sender, EventArgs e)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VMHostInfos WHERE HostName = @name", _con))
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
