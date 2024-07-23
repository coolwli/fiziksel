<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Panel</title>
    <style>
        /* Reset styles */
        *,
        *::before,
        *::after {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Arial, sans-serif;
            background-color: #f0f0f0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .panel {
            width: 100%;
            max-width: 600px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            padding: 10px;
        }

        .options-bar {
            display: flex;
            justify-content: space-between;
            border-bottom: 1px solid #ccc;
            margin-bottom: 20px;
            padding-bottom: 10px;
            position: relative;
        }

        .option-btn {
            padding: 12px 24px;
            font-size: 16px;
            border-radius: 4px;
            cursor: pointer;
            transition: background-color 0.3s ease, color 0.3s ease;
        }

        .option-btn.active {
            background-color: #e0e0e0;
            color: #333;
        }

        .option-btn:not(:last-child) {
            border-right: 1px solid #ccc;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th,
        td {
            padding: 16px;
            text-align: center;
            border: 1px solid #ccc;
            transition: background-color 0.3s ease;
        }

        th {
            background-color: #f0f0f0;
        }

        tr:nth-child(even) {
            background-color: #f9f9f9;
        }
    </style>
</head>

<body>
    <div class="panel">
        <div class="options-bar">
            <div class="option-btn active">1 Gün</div>
            <div class="option-btn">7 Gün</div>
            <div class="option-btn">30 Gün</div>
            <div class="option-btn">90 Gün</div>
            <div class="option-btn">360 Gün</div>
        </div>
        <table id="usage-table">
            <thead>
                <tr>
                    <th>Ad</th>
                    <th>Ortalama (%)</th>
                    <th>Max (%)</th>
                </tr>
            </thead>
            <tbody>
                <tr id="cpu-data">
                    <td>CPU</td>
                    <td data-values="20,30,40,50,1">20%</td>
                    <td data-values="30,40,50,60,1">30%</td>
                </tr>
                <tr id="memory-data">
                    <td>Bellek</td>
                    <td data-values="25,35,45,55,1">25%</td>
                    <td data-values="35,45,55,65,1">35%</td>
                </tr>
                <tr id="network-data">
                    <td>Ağ</td>
                    <td data-values="5,15,25,35,1">5%</td>
                    <td data-values="10,20,30,40,1">10%</td>
                </tr>
                <tr id="disk-data">
                    <td>Disk</td>
                    <td data-values="15,25,35,45,1">15%</td>
                    <td data-values="20,30,40,50,1">20%</td>
                </tr>
            </tbody>
        </table>
    </div>

    <script>
        const buttons = document.querySelectorAll('.options-bar .option-btn');
        const rows = document.querySelectorAll('#usage-table tbody tr');

        buttons.forEach(button => {
            button.addEventListener('click', () => {
                buttons.forEach(btn => btn.classList.remove('active'));
                button.classList.add('active');
                updateTableData(button);
            });
        });

        function updateTableData(button) {
            const index = Array.from(button.parentNode.children).indexOf(button);
            rows.forEach(row => {
                const cells = row.querySelectorAll('td:not(:first-child)');
                cells.forEach((cell, i) => {
                    const values = cell.getAttribute('data-values').split(',');
                    cell.textContent = values[index] + '%';
                });


            });
        }
    </script>
</body>

</html>
