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
            SqlCommand command = new SqlCommand("SELECT * FROM VMHostInfos WHERE HostName = @name", con);
            command.Parameters.AddWithValue("@name", hostName);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
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
                    avcpu.InnerText = reader["averageCpu"].ToString()+"%";
                    avmem.InnerText = reader["averageMemory"].ToString() + "%";
                    avdisk.InnerText = reader["averageDisk"].ToString() + " KBps";
                    avnet.InnerText = reader["averageNetwork"].ToString() + " KBps";
                    lasttime.InnerText = reader["LastWriteTime"].ToString();
                    uptimedays.InnerText = reader["uptimeDay"].ToString() + " Day";
                    




                }
            }
            showVMs();
            showCpus();
            showDss();
            showEvents();
        }

        public void showCpus()
        {
            string[] cpuId, cpuName;
            SqlCommand cmd = con.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT CPUIndex,CPUDesc FROM VMHostInfos  WHERE HostName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            cpuName = (dt.Rows[0][1]).ToString().Split('~');
            cpuId = (dt.Rows[0][0]).ToString().Split('~');
            for (int i = 0; i < cpuId.Length - 1; i++)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.InnerText = cpuId[i].ToString();
                newRow.Cells.Add(cell2);
                cell1.InnerText = cpuName[i].ToString();
                newRow.Cells.Add(cell1);

                cpuTable.Rows.Add(newRow);
            }
        }
        public void showEvents()
        {
            string[] ed, em;
            SqlCommand cmd = con.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT eventDate,eventMessage FROM VMHostInfos  WHERE HostName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
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
        public void showVMs() {
            string[] vmNames;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT vmsName FROM VMHostInfos  WHERE HostName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            vmNames = (dt.Rows[0][0]).ToString().Split('~');

        
            foreach (string vmname in vmNames)
            {
                cmd.CommandText = $"SELECT VMName,VMOwner,VMPowerState,VMTotalDisk FROM VMInfos2  WHERE VMName = '{vmname}'";
                cmd.ExecuteNonQuery();

                DataTable dtv = new DataTable();
                SqlDataAdapter daV = new SqlDataAdapter(cmd);
                daV.Fill(dtv);
                foreach (DataRow row in dtv.Rows)
                {
                    HtmlTableRow newRow = new HtmlTableRow();
                    for (int j = 0; j < 4; j++)
                    {
                        HtmlTableCell newCell = new HtmlTableCell();
                        newCell.InnerText = row[j].ToString();
                        newRow.Cells.Add(newCell);
                    }
                    vmsTable.Rows.Add(newRow);

                }

            }
            

        }

        public void showDss() {
            string[] dsName, dsCapacity, dsFree, dsPercentUsing;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT dsName, dsCapacity, dsFree, dsPercentUsing FROM VMHostInfos  WHERE HostName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dsName = (dt.Rows[0][0]).ToString().Split('~');
            dsCapacity = (dt.Rows[0][1]).ToString().Split('~');
            dsFree= (dt.Rows[0][2]).ToString().Split('~');
            dsPercentUsing= (dt.Rows[0][3]).ToString().Split('~');
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
                cell4.InnerText = "%"+dsPercentUsing[i].ToString();
                newRow.Cells.Add(cell4);

                dsTable.Rows.Add(newRow);
            }
        }

        protected void exportToExcelClick(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM VMHostInfos WHERE HostName = @name";
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
