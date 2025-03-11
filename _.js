// Verilen verilerden benzersiz değerleri alır
function getUniqueValues(data) {
    // Set kullanarak benzersiz değerleri alırız
    return [...new Set(data.map(item => item[1]))];
}

// Butonları oluşturma
function createButtons(data) {
    const uniqueValues = getUniqueValues(data);
    const buttonContainer = document.getElementById('button-container');
    
    // Butonları oluşturalım
    uniqueValues.forEach(value => {
        const button = document.createElement('button');
        button.textContent = value;  // Butonun metnini unique değeriyle ayarlıyoruz
        button.classList.add('hall-button');  // Buton için uygun sınıfı ekliyoruz
        
        // Butona tıklama olayını dinleyicisini ekliyoruz
        button.addEventListener('click', () => handleButtonClick(button));

        // Butonu container'a ekliyoruz
        buttonContainer.appendChild(button);
    });
}

// Butona tıklandığında yapılacak işlemler
function handleButtonClick(clickedButton) {
    const buttons = document.querySelectorAll('.hall-button');
    
    // Tüm butonlardan 'active' sınıfını kaldırıyoruz
    buttons.forEach(button => button.classList.remove('active'));
    
    // Tıklanan butona 'active' sınıfını ekliyoruz
    clickedButton.classList.add('active');
}
