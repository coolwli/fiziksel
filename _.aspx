<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Soft Sekme ve Panel Tasarımı</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .container {
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            max-width: 600px;
            width: 100%;
        }

        .tabs {
            display: flex;
            background-color: #f7f7fa;
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }

        .back-button {
            margin-right: auto;
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            background-color: #ddd;
            color: #333;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .back-button:hover {
            background-color: #ccc;
        }

        .tab-button {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            background-color: transparent;
            cursor: pointer;
            transition: background-color 0.3s;
            color: #333;
            margin-left: 10px;
        }

        .tab-button.active,
        .tab-button:hover {
            background-color: #ddd;
        }

        .tab-content {
            padding: 20px;
            display: none;
        }

        .tab-content h2 {
            margin-top: 0;
        }
    </style>
</head>

<body>
    <div class="container">
        <div class="tabs">
            <button class="back-button">Geri Dön</button>
            <button class="tab-button active" onclick="openTab('panel1')">Sekme 1</button>
            <button class="tab-button" onclick="openTab('panel2')">Sekme 2</button>
        </div>

        <div id="panel1" class="tab-content">
            <h2>Panel 1 İçeriği</h2>
            <p>Burada birinci panelin içeriği bulunuyor. Burada içeriğinizi düzenleyebilirsiniz.</p>
        </div>

        <div id="panel2" class="tab-content">
            <h2>Panel 2 İçeriği</h2>
            <p>Burada ikinci panelin içeriği bulunuyor. Burada içeriğinizi düzenleyebilirsiniz.</p>
        </div>
    </div>

    <script>
        function openTab(panelId) {
            var i, tabcontent, tabbuttons;
            tabcontent = document.getElementsByClassName("tab-content");
            tabbuttons = document.getElementsByClassName("tab-button");

            // Tüm panelleri gizle
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }

            // Tüm sekme butonlarının aktifliğini kaldır
            for (i = 0; i < tabbuttons.length; i++) {
                tabbuttons[i].classList.remove("active");
            }

            if (panelId) {
                // Seçilen paneli göster
                document.getElementById(panelId).style.display = "block";

                // Aktif sekme butonunu güncelle
                document.querySelector(`button[onclick="openTab('${panelId}')"]`).classList.add("active");
            }
        }

        // Varsayılan olarak ilk sekmeyi aç
        document.addEventListener("DOMContentLoaded", function () {
            openTab('panel1');
        });
    </script>
</body>

</html>
