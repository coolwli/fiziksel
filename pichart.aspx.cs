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
        SqlConnection con = new SqlConnection(@"Data Source=TEKSCR1\SQLEXPRESS;Initial Catalog=CloudUnited;Integrated Security=True");
        public string hostName;
        protected void Page_Load(object sender, EventArgs e)
        {
            string currenturl = Request.Url.AbsoluteUri;
            string id = Request.QueryString["id"];
            hostName = id;
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            showHost();


        }
        public void showHost()
        {
            DataTable dt = new DataTable();
            SqlCommand command = new SqlCommand("SELECT * FROM VMInfos2 WHERE VMName = @name", con);
            command.Parameters.AddWithValue("@name", hostName);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
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
                    averageDisk.InnerText = reader["VMAverageDiskUsage"].ToString()+ " KBps";
                    averageCpu.InnerText = reader["VMAverageCPUUsage"].ToString() + "%";
                    averageMemory.InnerText = reader["VMAverageMemoryUsage"].ToString() + "%";
                    averageNetwork.InnerText = reader["VMAverageNetworkUsage"].ToString() + " KBps";
                    lasttime.InnerText = reader["LastWriteTime"].ToString();
                    hostmodel.InnerText = reader["VMHostModel"].ToString();
                    os.InnerText = reader["VMGuestOS"].ToString();
                    string temp = reader["VMCreatedDate"].ToString();
                    reader.Close();
                    command.CommandText = $"SELECT * FROM VMsExtras WHERE VMName = '{hostName}'";
                    using (SqlDataReader cd = command.ExecuteReader())
                    {
                        if (cd.Read())
                            createdDate.InnerText = cd["CreatedDate"].ToString();
                        else
                            createdDate.InnerText = temp;


                    }



                }
            }
            showNetwork();
            showDisks();
            showEvents();

        }

        public void showEvents()
        {
            string[] ed, em;
            SqlCommand cmd = con.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT VMEventDates,VMEventMessages FROM VmInfos2  WHERE VMName = @name";
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
        public void showNetwork()
        {
            string[] adapterName, networkname, macAddress, NCType;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT VMNetworkAdapterName,VMNetworkName,VMNetworkCardType,VMNetworkAdapterMacAddress FROM VMInfos2  WHERE VMName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            adapterName = (dt.Rows[0][0]).ToString().Split('~');
            networkname = (dt.Rows[0][1]).ToString().Split('~');
            macAddress = (dt.Rows[0][3]).ToString().Split('~');
            NCType = (dt.Rows[0][2]).ToString().Split('~');
            for (int i = 0; i < adapterName.Length - 1; i++)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                HtmlTableCell cell3 = new HtmlTableCell();
                HtmlTableCell cell4 = new HtmlTableCell();
                cell1.InnerText = adapterName[i].ToString();
                newRow.Cells.Add(cell1);
                cell2.InnerText = networkname[i].ToString();
                newRow.Cells.Add(cell2);
                cell3.InnerText = macAddress[i].ToString();
                newRow.Cells.Add(cell3);
                cell4.InnerText = NCType[i].ToString();
                newRow.Cells.Add(cell4);

                networkTable.Rows.Add(newRow);
            }
        }
        public void showDisks()
        {
            string[] diskPath, totalSize, format, ds;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT VMDiskName,VMDiskTotalSize,VMDiskStorageFormat,VMDiskDataStore FROM VMInfos2 WHERE VMName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            diskPath = (dt.Rows[0][0]).ToString().Split('~');
            totalSize = (dt.Rows[0][1]).ToString().Split('~');
            format = (dt.Rows[0][2]).ToString().Split('~');
            ds = (dt.Rows[0][3]).ToString().Split('~');
            for (int i = 0; i < diskPath.Length - 1; i++)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                HtmlTableCell cell3 = new HtmlTableCell();
                HtmlTableCell cell4 = new HtmlTableCell();
                cell1.InnerText = diskPath[i].ToString();
                newRow.Cells.Add(cell1);
                cell2.InnerText = totalSize[i].ToString();
                newRow.Cells.Add(cell2);
                cell3.InnerText = format[i].ToString();
                newRow.Cells.Add(cell3);
                cell4.InnerText = ds[i].ToString();
                newRow.Cells.Add(cell4);

                disksTable.Rows.Add(newRow);
            }

        }
        protected void exportToExcelClick(object sender, EventArgs e)
        {

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM VMInfos2 WHERE VMName = @name";
            cmd.Parameters.AddWithValue("@name", hostName);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataColumn col in dt.Columns)
            {
                string[] keys = dt.Rows[0][col].ToString().Split('~');
                for(int i=1;i<keys.Length;i++)
                {
                    if(dt.Rows.Count<keys.Length)
                        dt.Rows.Add();
                    dt.Rows[i][col.ColumnName] = keys[i-1].Trim();
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

            using (StringWriter sw= new StringWriter())
            {
                using(HtmlTextWriter htw=new HtmlTextWriter(sw))
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
