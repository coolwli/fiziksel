<!DOCTYPE html>
<html lang="tr">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Panel Tasarımı</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }

        .container {
            width: 80%;
            margin: 50px auto;
        }

        .tab-button {
            background: #eee;
            border: none;
            padding: 15px 20px;
            cursor: pointer;
            font-size: 16px;
            flex: 1;
            text-align: center;
            transition: background-color 0.3s ease;
        }

        .tab-button:hover {
            background-color: #ddd;
        }

        .tab-button.active {
            background-color: #fff;
            border-bottom: 2px solid #007BFF;
        }

        .tab-content {
            padding: 20px;
            background-color: #fff
        }

        .tab-content h1 {
            margin-top: 0;
            font-size: 24px;
        }

        .tab-content p {
            font-size: 16px;
            color: #555;
        }
    </style>
</head>

<body>
    <div class="container">
        <div class="tabs">
            <button class="tab-button active" onclick="openTab(event, 'tab1')">Tab 1</button>
            <button class="tab-button" onclick="openTab(event, 'tab2')">Tab 2</button>
        </div>
        <div class="tab-content" id="tab1">
            <h1>Tab 1 İçeriği</h1>
            <p>Bu, ilk sekmenin içeriğidir.</p>
        </div>
        <div class="tab-content" id="tab2" style="display: none;">
            <h1>Tab 2 İçeriği</h1>
            <p>Bu, ikinci sekmenin içeriğidir.</p>
        </div>
    </div>

    <script>
        function openTab(event, tabId) {
            var i, tabContent, tabButtons;
            tabContent = document.getElementsByClassName("tab-content");
            for (i = 0; i < tabContent.length; i++) {
                tabContent[i].style.display = "none";
            }
            tabButtons = document.getElementsByClassName("tab-button");
            for (i = 0; i < tabButtons.length; i++) {
                tabButtons[i].className = tabButtons[i].className.replace(" active", "");
            }
            document.getElementById(tabId).style.display = "block";
            event.currentTarget.className += " active";
        }
    </script>
</body>

</html>
