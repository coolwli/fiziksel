function setTable(nameelement) {
    const names = document.querySelectorAll("#page-selector a");
    names.forEach((name) => name.classList.remove("active"));
    nameelement.classList.add("active");

    const name = nameelement.innerText;

    const chartContainer = document.getElementById("chart-container");
    const table = document.getElementById("hierarchy-table");

    // Her durumda önce tüm classları ve stilleri sıfırla
    chartContainer.classList.remove("all");
    table.style.display = "block";

    if (name === "All") {
        // "All" için class'ı geri ekle
        chartContainer.classList.add("all");

        // Tabloyu gizle
        table.style.display = "none";

        // Grafik için "All" (root level) verisini göster
        const startDate = new Date(document.getElementById("startDate").value);
        const endDate = new Date(document.getElementById("endDate").value);

        const filtered = datasets.filter((item) => {
            const d = new Date(item.tarih);
            return (
                d >= startDate &&
                d <= endDate &&
                item.parent === null // sadece root-level (All)
            );
        });

        if (filtered.length > 0) {
            const dummyRow = document.createElement("tr");
            dummyRow.dataset.name = "All";
            updateChart(dummyRow);
        }

        return;
    }

    // "Pendik", "Ankara" gibi alt sayfalar için:
    chartContainer.classList.remove("all");
    table.style.display = "block";

    const filtered = datasets.filter((item) =>
        item.name === name || item.parent === name
    );

    generateDate(filtered); // tablo ve ilk grafik otomatik yapılır
}
