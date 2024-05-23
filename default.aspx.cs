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
    public partial class clusterScreen : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        string hostName;



        protected void Page_Load(object sender, EventArgs e)
        {
            hostName = Request.QueryString["id"];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            showHost();

        }
        public void showHost()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM ClusterInfos WHERE ClusterName = @name", con);
            command.Parameters.AddWithValue("@name", hostName);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    vcenter.InnerText = reader["vCenter"].ToString();
                    datacenter.InnerText = reader["DataCenter"].ToString();
                    hae.InnerText = reader["ClusterHAE"].ToString();
                    drs.InnerText = reader["ClusterDRSEnabled"].ToString();
                    cpumhz.InnerText = reader["ClusterCPUCapacityMhz"].ToString();
                    totalmemory.InnerText = reader["ClusterTotalMemory"].ToString();
                    effectivememory.InnerText = reader["ClusterEffectiveMemory"].ToString();
                    totalcpu.InnerText = reader["ClusterTotalCPU"].ToString();
                    vmotion.InnerText = reader["ClusterNumVmotions"].ToString();
                    effectivecpu.InnerText = reader["ClusterEffectiveCPU"].ToString();
                    hostcount.InnerText = reader["ClusterNumHosts"].ToString();
                    cpucore.InnerText = reader["ClusterNumCPUCores"].ToString();
                    cputhread.InnerText = reader["ClusterNumCPUThreads"].ToString();
                    vmcount.InnerText = reader["TotalVMCount"].ToString();
                    lasttime.InnerText = reader["LastWriteTime"].ToString();

                }
            }
            showHosts();
            showDss();
            showEvents();

        }

        public void showEvents()
        {
            string[] ed, em;
            SqlCommand cmd = con.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT ClusterEventDates,ClusterEventMessages FROM ClusterInfos  WHERE ClusterName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {

                ed = (dt.Rows[0][1]).ToString().Split('~');
                em = (dt.Rows[0][0]).ToString().Split('~');
                for (int i = 0; i < ed.Length - 1; i++)
                {
                    HtmlTableRow newRow = new HtmlTableRow();
                    HtmlTableCell cell1 = new HtmlTableCell();
                    HtmlTableCell cell2 = new HtmlTableCell();
                    cell2.InnerText = ed[i].ToString();
                    newRow.Cells.Add(cell2);
                    cell1.InnerText = em[i].ToString();
                    newRow.Cells.Add(cell1);

                    eventsTable.Rows.Add(newRow);
                }
            }
        }

        public void showHosts()
        {
            string[] hostnames;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT VMHostNames FROM ClusterInfos  WHERE ClusterName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0) { 
                hostnames = (dt.Rows[0][0]).ToString().Split('~');


                foreach (string hostname in hostnames)
                {
                    cmd.CommandText = $"SELECT HostName,HostModel,vmsCount,HostNumCPUCores,HostMemorySize FROM VMHostInfos  WHERE hostName = '{hostname}'";
                    cmd.ExecuteNonQuery();

                    DataTable dtv = new DataTable();
                    SqlDataAdapter daV = new SqlDataAdapter(cmd);
                    daV.Fill(dtv);
                    foreach (DataRow row in dtv.Rows)
                    {
                        HtmlTableRow newRow = new HtmlTableRow();
                        for (int j = 0; j < 5; j++)
                        {
                            HtmlTableCell newCell = new HtmlTableCell();
                            newCell.InnerText = row[j].ToString();
                            newRow.Cells.Add(newCell);
                        }
                        hostsTable.Rows.Add(newRow);

                    }

                }
            }

        }

        public void showDss()
        {
            string[] dsName, dsCapacity, dsFree, dsPercentUsing;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT dsName, dsCapacity, dsFree, dsPercentUsing FROM ClusterInfos  WHERE ClusterName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {

                dsName = (dt.Rows[0][0]).ToString().Split('~');
                dsCapacity = (dt.Rows[0][1]).ToString().Split('~');
                dsFree = (dt.Rows[0][2]).ToString().Split('~');
                dsPercentUsing = (dt.Rows[0][3]).ToString().Split('~');
                for (int i = 0; i < dsName.Length - 1; i++)
                {
                    HtmlTableRow newRow = new HtmlTableRow();
                    HtmlTableCell cell1 = new HtmlTableCell();
                    HtmlTableCell cell2 = new HtmlTableCell();
                    HtmlTableCell cell3 = new HtmlTableCell();
                    HtmlTableCell cell4 = new HtmlTableCell();
                    cell1.InnerText = dsName[i].ToString();
                    newRow.Cells.Add(cell1);
                    cell2.InnerText = dsCapacity[i].ToString();
                    newRow.Cells.Add(cell2);
                    cell3.InnerText = dsFree[i].ToString();
                    newRow.Cells.Add(cell3);
                    cell4.InnerText = "%" + dsPercentUsing[i].ToString();
                    newRow.Cells.Add(cell4);

                    dsTable.Rows.Add(newRow);
                }

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
