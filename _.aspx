<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="example_table._default" %>

<html>
<head runat="server">
    <title>All VMs</title>
    <link rel="stylesheet" type="text/css" href="tableStyle.css" />
    <link rel="stylesheet" type="text/css" href="screenStyle.css" />

</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1 class="baslik" id="baslik" runat="server">Example Table</h1>
            <div id="logo"></div>

        </div>
        <div class="table-top">
            <h2 id="rowCounter"></h2>
            <button id="reset-button">Reset</button>
        </div>
        <div class="table-container">
            <table id="contentTable">
                <thead>
                    <tr>
                        <th class="dropdown">
                            Name<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="nameDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                        <th class="dropdown">
                            Age<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ageDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                        <th class="dropdown">
                            Gender<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="genderDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                        <th class="dropdown">
                            Country<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="country4Dropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                        </th>
                        <th class="dropdown">
                            Mail<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="mailDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server">
                </tbody>
            </table>
        </div>
        <div class="pagination" id="pagination"></div>
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>

        <script src="tableOrganizer.js"></script>
        <script>
            function add1000rowsTable() {
                for (let i = 0; i < 1000; i++) {
                    data.push({
                        "Name": "Name" + i,
                        "Age": Math.floor(Math.random() * 100),
                        "Gender": i % 2 == 0 ? "Male" : "Female",
                        "Country": "Country" + i % 10,
                        "Mail": "mail" + i + "@mail.com"
                    });
                }
            }
            add1000rowsTable();
            initializeTable();

        </script>
    </form>
</body>

</html>
