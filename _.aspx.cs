private void GenerateAlarmControls(Dictionary<string, Dictionary<string, object>> alarmData)
{
    // Clear previous controls
    phAlarms.Controls.Clear();

    int index = 0;
    foreach (var kvp in alarmData)
    {
        string vmName = kvp.Key;
        var data = kvp.Value;

        var alerts = data["Alerts"] as List<Dictionary<string, object>>;
        var latestAlert = alerts?.FirstOrDefault();

        // Ana satır
        var mainRow = new HtmlGenericControl("tr");
        mainRow.Attributes["class"] = "main-row";

        // VM Name
        var vmNameCell = new HtmlGenericControl("td");
        vmNameCell.InnerText = vmName;
        mainRow.Controls.Add(vmNameCell);

        // vCenter
        var vCenterCell = new HtmlGenericControl("td");
        vCenterCell.InnerText = data["vCenterName"]?.ToString() ?? "";
        mainRow.Controls.Add(vCenterCell);

        // CPU %
        var cpuCell = new HtmlGenericControl("td");
        cpuCell.InnerText = latestAlert != null ? $"{latestAlert["CpuPercentage"]}%" : "-";
        mainRow.Controls.Add(cpuCell);

        // Date
        var dateCell = new HtmlGenericControl("td");
        dateCell.InnerText = latestAlert != null ? latestAlert["Date"]?.ToString() : "-";
        mainRow.Controls.Add(dateCell);

        // Dropdown
        var actionCell = new HtmlGenericControl("td");
        var ddl = new DropDownList();
        ddl.ID = $"durumddl_{index}";
        ddl.CssClass = "alarm-dropdown";
        ddl.AutoPostBack = true;
        ddl.SelectedIndexChanged += durumddl_SelectedIndexChanged;

        ddl.Items.Add(new ListItem("1 Hafta Boyunca Uyarma", "0"));
        ddl.Items.Add(new ListItem("Bu Sunucuyu Asla Uyarma", "1"));

        string neverSentEmail = data["NeverSentEmail"]?.ToString() ?? "0";
        ddl.SelectedValue = neverSentEmail == "True" ? "1" : "0";
        ddl.Attributes["data-vmname"] = vmName;

        actionCell.Controls.Add(ddl);
        mainRow.Controls.Add(actionCell);

        // Toggle icon (sadece birden fazla alarm varsa)
        var toggleCell = new HtmlGenericControl("td");
        toggleCell.Attributes["class"] = "toggle-icon-cell";

        if (alerts.Count > 1)
        {
            var toggleIcon = new HtmlGenericControl("span");
            toggleIcon.InnerText = "▶";
            toggleIcon.Attributes["class"] = "toggle-icon";
            toggleIcon.Attributes["data-index"] = index.ToString();
            toggleCell.Controls.Add(toggleIcon);
        }
        else
        {
            toggleCell.InnerHtml = "&nbsp;";
        }

        mainRow.Controls.Add(toggleCell);

        phAlarms.Controls.Add(mainRow);

        // Detay satırları (ilk alert zaten yukarıda gösterildi)
        foreach (var alert in alerts.Skip(1))
        {
            var alertRow = new HtmlGenericControl("tr");
            alertRow.Attributes["class"] = "alert-detail-row";
            alertRow.Attributes["data-parent-index"] = index.ToString();
            alertRow.Style["display"] = "none";

            // Boş hücreler (VM ve vCenter kolonları için)
            for (int i = 0; i < 2; i++)
            {
                alertRow.Controls.Add(new HtmlGenericControl("td"));
            }

            // CPU %
            var cpuDetailCell = new HtmlGenericControl("td");
            cpuDetailCell.InnerText = $"{alert["CpuPercentage"]}%";
            alertRow.Controls.Add(cpuDetailCell);

            // Date
            var dateDetailCell = new HtmlGenericControl("td");
            dateDetailCell.InnerText = alert["Date"]?.ToString();
            alertRow.Controls.Add(dateDetailCell);

            // Dropdown ve ikon için boş hücreler
            alertRow.Controls.Add(new HtmlGenericControl("td"));
            alertRow.Controls.Add(new HtmlGenericControl("td"));

            phAlarms.Controls.Add(alertRow);
        }

        index++;
    }
}



<script>
    document.addEventListener("DOMContentLoaded", function () {
        const toggleIcons = document.querySelectorAll(".toggle-icon");

        toggleIcons.forEach(icon => {
            icon.addEventListener("click", function () {
                const index = this.getAttribute("data-index");
                const rows = document.querySelectorAll(`tr[data-parent-index='${index}']`);

                rows.forEach(row => {
                    const isHidden = row.style.display === "none";
                    row.style.display = isHidden ? "" : "none";
                });

                this.innerText = this.innerText === "▶" ? "▼" : "▶";
            });
        });
    });
</script>


.toggle-icon {
    cursor: pointer;
    font-weight: bold;
    display: inline-block;
    padding-left: 10px;
}

.alert-detail-row {
    display: none;
}
