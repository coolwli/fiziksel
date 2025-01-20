using System;
using System.IO;
using System.Web.UI;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Data.OleDb;

public partial class UploadFile : Page
{
    private const string UploadDirectory = "~/Uploads";
    private const string AllowedFileExtensions = ".csv,.xlsx";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlColumns.Items.Clear();
        }
    }

    protected void UploadFile(object sender, EventArgs e)
    {
        if (!fileUpload.HasFile)
        {
            ShowMessage("Lütfen bir dosya yükleyin.");
            return;
        }

        string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
        if (!AllowedFileExtensions.Contains(fileExtension))
        {
            ShowMessage("Geçersiz dosya tipi. Lütfen CSV veya XLSX dosyası yükleyin.");
            return;
        }

        string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
        string filePath = Path.Combine(Server.MapPath(UploadDirectory), uniqueFileName);

        try
        {
            fileUpload.SaveAs(filePath);

            string[] columns = fileExtension == ".csv" ? ReadCsvFile(filePath) : ReadExcelFile(filePath);
            if (columns.Length > 0)
            {
                PopulateDropdown(columns);
            }
            else
            {
                ShowMessage("Dosya okuma hatası: Sütunlar alınamadı.");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Dosya yüklenirken bir hata oluştu: " + ex.Message);
        }
    }

    private void ShowMessage(string message)
    {
        // Assuming there's a Label control (lblMessage) in your page for messages
        lblMessage.Text = message;
    }

    private void PopulateDropdown(string[] columns)
    {
        ddlColumns.Items.Clear();
        foreach (var column in columns)
        {
            ddlColumns.Items.Add(new ListItem(column, column));
        }
    }

    private string[] ReadCsvFile(string filePath)
    {
        try
        {
            var lines = File.ReadLines(filePath).Take(1).ToList();
            if (lines.Any())
            {
                return lines[0].Split(',');
            }
        }
        catch (Exception ex)
        {
            ShowMessage("CSV dosyası işlenirken bir hata oluştu: " + ex.Message);
        }
        return Array.Empty<string>();
    }

    private string[] ReadExcelFile(string filePath)
    {
        try
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetSchema("Columns");
                return schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray();
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Excel dosyası işlenirken bir hata oluştu: " + ex.Message);
        }
        return Array.Empty<string>();
    }

    protected void RunPowerShellScript(object sender, EventArgs e)
    {
        string selectedColumn = ddlColumns.SelectedValue;

        if (string.IsNullOrEmpty(selectedColumn))
        {
            ShowMessage("Lütfen bir sütun seçin.");
            return;
        }

        string filePath = Path.Combine(Server.MapPath(UploadDirectory), fileUpload.FileName);
        string fileExtension = Path.GetExtension(filePath).ToLower();
        string fileContent = fileExtension == ".csv" 
            ? ReadCsvFileContent(filePath, selectedColumn) 
            : ReadExcelFileContent(filePath, selectedColumn);

        if (string.IsNullOrEmpty(fileContent))
        {
            ShowMessage("Seçilen sütun ile ilgili veri alınamadı.");
            return;
        }

        ExecutePowerShell(fileContent);
    }

    private string ReadCsvFileContent(string filePath, string selectedColumn)
    {
        var sb = new StringBuilder();
        var lines = File.ReadLines(filePath).Skip(1);

        foreach (var line in lines)
        {
            var columns = line.Split(',');
            int columnIndex = Array.IndexOf(columns, selectedColumn);
            if (columnIndex >= 0)
            {
                sb.AppendLine(columns[columnIndex]);
            }
        }
        return sb.ToString();
    }

    private string ReadExcelFileContent(string filePath, string selectedColumn)
    {
        var sb = new StringBuilder();
        try
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable sheetTable = connection.GetSchema("Tables");
                string sheetName = sheetTable.Rows[0]["TABLE_NAME"].ToString();

                string query = $"SELECT * FROM [{sheetName}]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                int columnIndex = dt.Columns.IndexOf(selectedColumn);
                if (columnIndex >= 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(row[columnIndex].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Excel dosyası işlenirken bir hata oluştu: " + ex.Message);
        }
        return sb.ToString();
    }

    private void ExecutePowerShell(string fileContent)
    {
        string psScriptPath = @"C:\path\to\your\script.ps1";
        string arguments = $"-ExecutionPolicy Bypass -File \"{psScriptPath}\" -InputData \"{fileContent}\"";

        try
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    ShowMessage("PowerShell Script Output: " + result);
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("PowerShell betiği çalıştırılırken bir hata oluştu: " + ex.Message);
        }
    }
}
