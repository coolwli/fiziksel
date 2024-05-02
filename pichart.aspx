<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Pie Chart Excel</title>
  <style>
    table {
      border-collapse: collapse;
      width: 100%;
    }

    th,
    td {
      border: 1px solid #ddd;
      padding: 12px;
      text-align: left;
    }

    th {
      background-color: #f2f2f2;
      color: #333;
      font-weight: bold;
      cursor: pointer;
    }

    tr:nth-child(even) {
      background-color: #f9f9f9;
    }

    tr:hover {
      background-color: #f5f5f5;
    }

    .dropdown-content {
      display: none;
      position: absolute;
      background-color: #f9f9f9;
      min-width: 160px;
      box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
      padding: 12px 16px;
      z-index: 1;
    }

    .dropdown:hover .dropdown-content {
      display: block;
    }
  </style>
</head>

<body>
  <table>
    <thead>
      <tr>
        <th class="dropdown">Name
          <div class="dropdown-content" id="nameDropdown"></div>
        </th>
        <th class="dropdown">Age
          <div class="dropdown-content" id="ageDropdown"></div>
        </th>
        <th class="dropdown">Country
          <div class="dropdown-content" id="countryDropdown"></div>
        </th>
        <th class="dropdown">Occupation
          <div class="dropdown-content" id="occupationDropdown"></div>
        </th>
        <th class="dropdown">Email
          <div class="dropdown-content" id="emailDropdown"></div>
        </th>
      </tr>
    </thead>
    <tbody id="tableBody">
      <tr>
        <td>John</td>
        <td>25</td>
        <td>USA</td>
        <td>Engineer</td>
        <td>john@example.com</td>
      </tr>
      <tr>
        <td>Jane</td>
        <td>30</td>
        <td>Canada</td>
        <td>Doctor</td>
        <td>jane@example.com</td>
      </tr>
      <tr>
        <td>Michael</td>
        <td>35</td>
        <td>USA</td>
        <td>Lawyer</td>
        <td>michael@example.com</td>
      </tr>
      <tr>
        <td>Sarah</td>
        <td>25</td>
        <td>USA</td>
        <td>Teacher</td>
        <td>sarah@example.com</td>
      </tr>
      <tr>
        <td>David</td>
        <td>30</td>
        <td>Canada</td>
        <td>Engineer</td>
        <td>david@example.com</td>
      </tr>
      <tr>
        <td>Emily</td>
        <td>35</td>
        <td>USA</td>
        <td>Doctor</td>
        <td>emily@example.com</td>
      </tr>
      <tr>
        <td>Robert</td>
        <td>25</td>
        <td>USA</td>
        <td>Lawyer</td>
        <td>robert@example.com</td>
      </tr>
      <tr>
        <td>Lisa</td>
        <td>30</td>
        <td>Canada</td>
        <td>Teacher</td>
        <td>lisa@example.com</td>
      </tr>
      <tr>
        <td>James</td>
        <td>35</td>
        <td>USA</td>
        <td>Engineer</td>
        <td>james@example.com</td>
      </tr>
      <tr>
        <td>Amy</td>
        <td>25</td>
        <td>USA</td>
        <td>Doctor</td>
        <td>amy@example.com</td>
      </tr>
    </tbody>
  </table>

  <script>
    // Recreate dropdown menus for each column when a checkbox changes
    document.querySelectorAll("input[type='checkbox']").forEach(checkbox => {
      checkbox.addEventListener("change", function() {
        generateFilterDropdowns();
      });
    });

    // Add a reset button
    const resetButton = document.createElement("button");
    resetButton.textContent = "Reset";
    resetButton.addEventListener("click", function() {
      document.querySelectorAll("input[type='checkbox']").forEach(checkbox => {
        checkbox.checked = false;
      });
      filterTable();
    });
    document.body.appendChild(resetButton);

    // Generate dropdown menus for each column
    function generateFilterDropdowns() {
      const columns = Array.from(document.querySelectorAll("th.dropdown"));
      columns.forEach(column => {
        const columnName = column.textContent.trim();
        const dropdownContent = column.querySelector(".dropdown-content");

        // Clear existing checkboxes
        dropdownContent.innerHTML = "";

        // Create "Select All" checkbox
        const selectAllCheckbox = document.createElement("input");
        selectAllCheckbox.type = "checkbox";
        selectAllCheckbox.value = "Select All";
        selectAllCheckbox.addEventListener("change", function() {
          const checkboxes = dropdownContent.querySelectorAll("input[type='checkbox']");
          checkboxes.forEach(checkbox => {
            checkbox.checked = selectAllCheckbox.checked;
          });
          filterTable();
        });
        dropdownContent.appendChild(selectAllCheckbox);

        const selectAllLabel = document.createElement("label");
        selectAllLabel.textContent = "Select All";
        dropdownContent.appendChild(selectAllLabel);

        dropdownContent.appendChild(document.createElement("br"));

        const values = Array.from(new Set(Array.from(document.querySelectorAll(`td:nth-child(${columns.indexOf(column) + 1})`)).map(td => td.textContent)));

        values.forEach(value => {
          const checkbox = document.createElement("input");
          checkbox.type = "checkbox";
          checkbox.value = value;
          checkbox.addEventListener("change", filterTable);
          dropdownContent.appendChild(checkbox);

          const label = document.createElement("label");
          label.textContent = value;
          dropdownContent.appendChild(label);

          dropdownContent.appendChild(document.createElement("br"));
        });
      });
    }

    // Filter table based on selected checkboxes
    function filterTable() {
      const columns = Array.from(document.querySelectorAll("th.dropdown"));
      const filters = {};

      columns.forEach(column => {
        const columnName = column.textContent.trim();
        const dropdownContent = column.querySelector(".dropdown-content");
        const checkedValues = Array.from(dropdownContent.querySelectorAll("input:checked")).map(checkbox => checkbox.value);
        filters[columnName.toLowerCase()] = checkedValues;
      });

      const rows = Array.from(document.querySelectorAll("tbody tr"));
      rows.forEach(row => {
        const cells = Array.from(row.querySelectorAll("td"));
        const display = cells.every((cell, index) => {
          const columnName = columns[index].textContent.trim().toLowerCase();
          const cellValue = cell.textContent;
          return filters[columnName].length === 0 || filters[columnName].includes(cellValue);
        }) ? "" : "none";
        row.style.display = display;
      });
    }

    // Generate dropdown menus on page load
    generateFilterDropdowns();
  </script>
</body>
</body>

</html>
