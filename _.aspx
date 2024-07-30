<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Disk Usage</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #e9ecef;
            color: #495057;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .container {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            padding: 20px;
            max-width: 500px;
            width: 100%;
        }

        .disk-info {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            font-size: 12px;
        }

        .disk-info p {
            margin: 0;
        }

        .disk-info .disk-free,
        .disk-info .disk-used {
            font-weight: bold;
        }

        .disk-info .disk-used {
            text-align: right;
        }

        .bar {
            background-color: #e0e0e0;
            border-radius: 10px;
            height: 25px;
            overflow: hidden;
            position: relative;
            width: 100%;
            margin: 10px 0;
        }

        .bar span {
            display: block;
            height: 100%;
            width: 0;
            background-color: #007bff;
            position: absolute;
            top: 0;
            left: 0;
            transition: width 0.3s;
        }

        .bar-label {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            color: #fff;
            font-weight: bold;
            font-size: 14px;
        }
    </style>
</head>

<body>
    <div class="container">
        <div class="disk-info">
            <p class="disk-free">Free Disk: <strong>200 GB</strong></p>
            <p class="disk-used">Used Disk: <strong>300 GB</strong></p>
        </div>
        <div class="bar">
            <span style="width: 60%"></span>
            <div class="bar-label">60%</div>
        </div>
        <p style="font-size: 12px; color: #6c757d; text-align: center;">Total Disk: <strong>500 GB</strong></p>
    </div>
</body>

</html>
