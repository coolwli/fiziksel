<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Content Slider</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #f0f0f0;
        }

        .container {
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
        }

        .panel {
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            width: 300px;
            text-align: center;
        }

        h1 {
            font-size: 24px;
            margin-bottom: 20px;
        }

        .content {
            display: none;
            font-size: 18px;
            padding: 20px;
            background-color: #e6e6e6;
            border-radius: 5px;
            margin: 10px 0;
        }

        .controls {
            display: flex;
            justify-content: space-between;
            margin: 10px 0;
        }

        .nav-btn {
            background-color: #fff;
            border: 1px solid #ccc;
            border-radius: 50%;
            padding: 10px;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .nav-btn:hover {
            background-color: #f5f5f5;
        }

        .dots {
            display: flex;
            justify-content: center;
            margin-top: 10px;
        }

        .dot {
            height: 10px;
            width: 10px;
            margin: 0 5px;
            background-color: #bbb;
            border-radius: 50%;
            display: inline-block;
            transition: background-color 0.3s;
            cursor: pointer;
        }

        .active-dot {
            background-color: #4CAF50;
        }

        .active {
            background-color: #717171;
        }
    </style>
</head>

<body>
    <div class="container">
        <div class="panel">
            <h1>Content Slider</h1>
            <div class="content-box" id="contentBox"></div>
            <div class="controls">
                <button class="nav-btn" id="prevBtn">&#8592;</button>
                <button class="nav-btn" id="nextBtn">&#8594;</button>
            </div>
            <div class="dots" id="dots"></div>
        </div>
    </div>

    <script>
        // İçerik verisi
        const contentData = [
            "Content 1: This is the first content.",
            "Content 2: This is the second content.",
            "Content 3: This is the third content.",
            "Content 4: This is the fourth content."
        ];

        let currentIndex = 0;

        // İçerik ve noktaların dinamik olarak eklenmesi
        const contentBox = document.getElementById('contentBox');
        const dotsContainer = document.getElementById('dots');

        contentData.forEach((content, index) => {
            // İçerik ekle
            const contentDiv = document.createElement('div');
            contentDiv.classList.add('content');
            contentDiv.id = `content-${index}`;
            contentDiv.innerText = content;
            contentBox.appendChild(contentDiv);

            // Nokta ekle
            const dot = document.createElement('span');
            dot.classList.add('dot');
            dot.setAttribute('data-index', index);
            dotsContainer.appendChild(dot);
        });

        const contents = document.querySelectorAll('.content');
        const dots = document.querySelectorAll('.dot');

        // İçeriği gösterme fonksiyonu
        function showContent(index) {
            // Tüm içerikleri gizle
            contents.forEach(content => content.style.display = 'none');
            dots.forEach(dot => dot.classList.remove('active-dot'));

            // Şu anki içeriği göster ve aktif noktayı güncelle
            contents[index].style.display = 'block';
            dots[index].classList.add('active-dot');
        }

        // Sağ ve Sol ok butonları
        document.getElementById('nextBtn').addEventListener('click', () => {
            currentIndex = (currentIndex + 1) % contents.length;
            showContent(currentIndex);
        });

        document.getElementById('prevBtn').addEventListener('click', () => {
            currentIndex = (currentIndex - 1 + contents.length) % contents.length;
            showContent(currentIndex);
        });

        // Noktalara tıklama olayları
        dots.forEach(dot => {
            dot.addEventListener('click', () => {
                const index = parseInt(dot.getAttribute('data-index'));
                currentIndex = index;
                showContent(currentIndex);
            });
        });

        // İlk içeriği göster
        showContent(currentIndex);
    </script>
</body>

</html>
