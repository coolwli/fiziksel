<!DOCTYPE html>
<html lang="tr">
<head>
  <meta charset="UTF-8">
  <title>Element Görüntü Kopyala</title>
  <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
</head>
<body>

  <div id="hedef" style="width:300px;height:150px;background:#88c;text-align:center;color:white;line-height:150px;">
    Bu bir örnek içerik
  </div>

  <button onclick="ekranGoruntusunuKopyala()">Görseli Kopyala</button>

  <script>
    function ekranGoruntusunuKopyala() {
      const hedef = document.getElementById('hedef');

      html2canvas(hedef).then(canvas => {
        canvas.toBlob(blob => {
          if (navigator.clipboard && navigator.clipboard.write) {
            const item = new ClipboardItem({ 'image/png': blob });
            navigator.clipboard.write([item]).then(() => {
              alert('Görsel panoya kopyalandı!');
            }).catch(err => {
              alert('Kopyalama başarısız: ' + err);
            });
          } else {
            alert('Tarayıcınız panoya görsel kopyalamayı desteklemiyor.');
          }
        });
      });
    }
  </script>

</body>
</html>
