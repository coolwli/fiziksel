<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="vminfo._default"  %>

<html>
<head runat="server">
    <title>All VMs</title>
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/default-style.css" />
    <link rel="stylesheet" type="text/css" href="https://cloudunited/Styles/table-style.css" />
    <style>

        .header {
            overflow: hidden;
            background-color: var(--primary-color);
        }

        .header a {
            color: white;
            text-decoration: none;
            float: left;
            text-transform: uppercase;
            padding: 10px 15px;
            line-height: 10px;
            position: relative;
            text-align: center;
            border-radius: 4px;
        }

        .header a:hover {
            background-color: #383d42;
        }

        .header a.active {
            background-color: #383d42;
            color: white;
        }

        .header a:not(:last-child)::after {
            content: '|';
            position: absolute;
            right: -10px;
            top: 50%;
            transform: translateY(-50%);
        }

    </style>

</head>

<body>
    <form id="form1" runat="server">
        <div class="header">
            <div id="logo"></div>
            <div>
                <a class="active">
                    <h2>VM</h2>
                </a>
                <a href="hosts.aspx">
                    <h2>VM Host</h2>
                </a>
                <a href="clusters.aspx">
                    <h2>Cluster</h2>
                </a>
            </div>
        </div>
        <div class="table-top">
            <h2 id="rowCounter"></h2>
            <button id="reset-button">Reset</button>
        </div>
        <div class="table-container" >
            <table id="contentTable">
                <thead>
                    <tr>
                        <th style="width:16%" class="dropdown">
                            Name<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="nameDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown">
                            vCenter<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="vcDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            CPU<span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="cpuDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Memory(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="memoryDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Total Disk(GB) <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="totaldiskDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown">
                            Power State <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="powerstateDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Cluster <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="clusterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown">
                            Data Center <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="datacenterDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>

                            </div>
                        </th>
                        <th class="dropdown" style="width:20%">
                            Owner <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="ownerDropdown">
                                <input type="text" placeholder="Search" onkeyup="searchCheckboxes(this)" />
                                <div class="select-all-div">
                                    <input type="checkbox">
                                    <label>Select All</label>
                                    <br>
                                </div>
                                <div class="checkboxes"></div>
                            </div>
                        </th>
                        <th class="dropdown" style="width:12%">
                            Created Date <span class="dropdown-arrow">&#9660;</span>
                            <div class="dropdown-content" id="createddateDropdown">
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
            document.getElementById('logo').addEventListener('click', function () {
                window.location.href='/';
            });
        </script>        

    </form>
</body>

</html>
