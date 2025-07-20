<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>VM Monitoring</title>
        <style>
            * {
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }

            body {
                font-family: Arial, sans-serif;
                background: #f5f5f5;
            }

            .header {
                background: #2c3e50;
                color: white;
                padding: 20px;
                text-align: center;
            }

            .header h1 {
                font-size: 24px;
            }

            .container {
                max-width: 1000px;
                margin: 20px auto;
                background: white;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                padding: 20px;
            }

            .buttons {
                margin-bottom: 20px;
            }

            .btn {
                background: #3498db;
                color: white;
                border: none;
                padding: 10px 15px;
                border-radius: 5px;
                cursor: pointer;
                margin-right: 10px;
            }

            .btn:hover {
                background: #2980b9;
            }

            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 10px;
            }

            th,
            td {
                padding: 12px;
                text-align: left;
                border-bottom: 1px solid #ddd;
                position: relative;
            }

            th {
                background: #f8f9fa;
                font-weight: bold;
                cursor: pointer;
            }

            th:hover {
                background: #e9ecef;
            }

            tr:hover {
                background: #f5f5f5;
            }

            th:hover .resize-handle {
                display: block;
                background: #ccc; /* isteğe bağlı: görünürlüğü artırmak için */
            }

            .resize-handle {
                display: none;
                position: absolute;
                right: 0;
                top: 50%;
                transform: translateY(-50%);
                height: 60%;
                width: 3px;
                background-color: rgba(0, 0, 0, 0.15);
                border-radius: 2px;
                cursor: col-resize;
                transition: background-color 0.3s ease;
            }

            .filter-box {
                position: absolute;
                top: 100%;
                left: 0;
                background: white;
                border: 1px solid #ddd;
                border-radius: 5px;
                padding: 10px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                display: none;
                z-index: 100;
                min-width: 200px;
            }

            th:hover .filter-box {
                display: block;
            }

            .filter-input {
                width: 100%;
                padding: 5px;
                border: 1px solid #ddd;
                border-radius: 3px;
                margin-bottom: 10px;
            }

            .filter-item {
                margin: 5px 0;
            }
        </style>
    </head>
    <body>
        <div class="header">
            <h1>VM CPU Monitoring</h1>
        </div>

        <div class="container">
            <table id="dataTable">
                <thead>
                    <tr>
                        <th onclick="sortTable(0)">
                            VM Name
                            <div class="resize-handle"></div>
                            <div class="filter-box">
                                <input type="text" class="filter-input" placeholder="Filter..." />
                                <div class="filter-item"><input type="checkbox" value="VM-01" /> VM-01</div>
                                <div class="filter-item"><input type="checkbox" value="VM-02" /> VM-02</div>
                                <div class="filter-item"><input type="checkbox" value="VM-03" /> VM-03</div>
                            </div>
                        </th>
                        <th onclick="sortTable(1)">
                            Host
                            <div class="resize-handle"></div>
                            <div class="filter-box">
                                <input type="text" class="filter-input" placeholder="Filter..." />
                                <div class="filter-item"><input type="checkbox" value="Host-01" /> Host-01</div>
                                <div class="filter-item"><input type="checkbox" value="Host-02" /> Host-02</div>
                            </div>
                        </th>
                        <th onclick="sortTable(2)">
                            CPU Usage
                            <div class="resize-handle"></div>
                            <div class="filter-box">
                                <input type="text" class="filter-input" placeholder="Filter..." />
                                <div class="filter-item"><input type="checkbox" value="low" /> Low (0-30%)</div>
                                <div class="filter-item"><input type="checkbox" value="medium" /> Medium (31-70%)</div>
                                <div class="filter-item"><input type="checkbox" value="high" /> High (71-100%)</div>
                            </div>
                        </th>
                        <th onclick="sortTable(3)">
                            Status
                            <div class="resize-handle"></div>
                        </th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    <tr>
                        <td>VM-01</td>
                        <td>Host-01</td>
                        <td>25%</td>
                        <td><span class="status online">online</span></td>
                    </tr>
                    <tr>
                        <td>VM-02</td>
                        <td>Host-02</td>
                        <td>65%</td>
                        <td><span class="status warning">warning</span></td>
                    </tr>
                    <tr>
                        <td>VM-03</td>
                        <td>Host-01</td>
                        <td>85%</td>
                        <td><span class="status online">online</span></td>
                    </tr>
                    <tr>
                        <td>VM-04</td>
                        <td>Host-02</td>
                        <td>15%</td>
                        <td><span class="status offline">offline</span></td>
                    </tr>
                    <tr>
                        <td>VM-05</td>
                        <td>Host-01</td>
                        <td>45%</td>
                        <td><span class="status online">online</span></td>
                    </tr>
                    <tr>
                        <td>VM-06</td>
                        <td>Host-02</td>
                        <td>92%</td>
                        <td><span class="status warning">warning</span></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <script>
            // Sütun boyutlandırma
            function makeResizable() {
                var cols = document.querySelectorAll("th");

                for (var i = 0; i < cols.length; i++) {
                    var col = cols[i];
                    var resizer = col.querySelector(".resize-handle");

                    resizer.addEventListener("mousedown", function (e) {
                        var startX = e.pageX;
                        var startWidth = this.parentElement.offsetWidth;
                        var col = this.parentElement;

                        function doResize(e) {
                            var newWidth = startWidth + (e.pageX - startX);
                            if (newWidth > 50) {
                                col.style.width = newWidth + "px";
                            }
                        }

                        function stopResize() {
                            document.removeEventListener("mousemove", doResize);
                            document.removeEventListener("mouseup", stopResize);
                        }

                        document.addEventListener("mousemove", doResize);
                        document.addEventListener("mouseup", stopResize);
                    });
                }
            }

            makeResizable();
        </script>
    </body>
</html>
