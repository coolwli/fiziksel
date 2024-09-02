document.getElementById('export-button').addEventListener('click', (event) => {
    event.preventDefault();

    const hdnInput = document.getElementById('hiddenField');
    // `filteredData` içindeki her bir öğeyi nesneye dönüştürüyoruz
    const vmData = filteredData.map(row => ({
        VMName: row.VMName,
        vCenter: row.vCenter
    }));

    // `vmData` dizisini JSON formatına çeviriyoruz
    var jsondata = JSON.stringify(vmData);
    hdnInput.value = jsondata;

    console.log(vmData);
    document.getElementById('hiddenButton').click();
});







public class VMData
{
    public string VMName { get; set; }
    public string vCenter { get; set; }
}


using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Linq;

protected void hiddenButton_Click(object sender, EventArgs e)
{
    var jsonData = hiddenField.Value;

    if (string.IsNullOrEmpty(jsonData))
    {
        Response.Write("No data received.");
        Response.End();
        return;
    }

    var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    var vmDataList = serializer.Deserialize<List<VMData>>(jsonData);

    if (vmDataList.Count == 0)
    {
        Response.Write("No VM data provided.");
        Response.End();
        return;
    }

    var csv = new StringBuilder();
    int batchSize = 2000;
    for (int batchStart = 0; batchStart < vmDataList.Count; batchStart += batchSize)
    {
        var batch = vmDataList.Skip(batchStart).Take(batchSize).ToList();

        var queryBuilder = new StringBuilder("SELECT VMName, vCenter FROM vminfoVMs WHERE (");
        for (int i = 0; i < batch.Count; i++)
        {
            if (i > 0) queryBuilder.Append(" OR ");
            queryBuilder.Append("(VMName = @name" + i + " AND vCenter = @vCenter" + i + ")");
        }
        queryBuilder.Append(")");

        using (var con = new SqlConnection(connectionString))
        {
            con.Open();

            using (var cmd = new SqlCommand(queryBuilder.ToString(), con))
            {
                for (int i = 0; i < batch.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@name" + i, batch[i].VMName);
                    cmd.Parameters.AddWithValue("@vCenter" + i, batch[i].vCenter);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    if (batchStart == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i > 0) csv.Append(";");
                            csv.Append(reader.GetName(i));
                        }
                        csv.AppendLine();
                    }

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i > 0) csv.Append(";");
                            csv.Append(reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString());
                        }
                        csv.AppendLine();
                    }
                }
            }
        }
    }

    Response.Clear();
    Response.ContentType = "text/csv";
    Response.ContentEncoding = Encoding.UTF32;
    Response.AddHeader("content-disposition", "attachment;filename=vmpedia_vms.csv");
    Response.Write(csv.ToString());
    Response.End();
}

