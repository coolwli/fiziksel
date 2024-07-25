<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Percentage Bar</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            background-color: #f7f9fc;
            font-family: 'Arial', sans-serif;
        }

        .center-panel {
            width: 300px;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            background-color: #ffffff;
            text-align: center;
        }

        .title {
            font-size: 18px;
            margin-bottom: 10px;
            color: #333;
        }

        .bar-container {
            width: 100%;
            height: 20px;
            background-color: #e0e0e0;
            border-radius: 10px;
            overflow: hidden;
        }

        .bar {
            height: 100%;
            width: 70%;
            background: linear-gradient(90deg, #4caf50, #8bc34a);
        }
    </style>
</head>

<body>
    <div class="center-panel">
        <div class="title">Progress</div>
        <div class="bar-container">
            <div class="bar"></div>
        </div>
    </div>
</body>

</html>
