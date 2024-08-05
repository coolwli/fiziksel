
        <script>
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get("id");

            const baslik = document.getElementById("baslik");
            baslik.textContent = id;

            const addClickListener = (elementId, url) => {
                document.getElementById(elementId).addEventListener("click", function () {
                    window.location.href = `${url}?id=${this.textContent}`;
                });
            };

            addClickListener("host", "hostscreen.aspx");
            addClickListener("cluster", "clusterscreen.aspx");

            document.querySelectorAll('.tab-button').forEach(button => {
                button.addEventListener('click', (event) => {
                    event.preventDefault();
                    openTab(button.getAttribute('data-panel'), button);
                });
            });

            function sortTable(columnIndex, column) {
                const table = column.closest('table');
                let rows = Array.from(table.rows).slice(1);
                let dir = "asc", switchcount = 0, switching = true;

                while (switching) {
                    switching = false;
                    for (let i = 0; i < rows.length - 1; i++) {
                        let xValue = rows[i].cells[columnIndex].innerText;
                        let yValue = rows[i + 1].cells[columnIndex].innerText;

                        if (!isNaN(xValue) && !isNaN(yValue)) {
                            xValue = parseFloat(xValue);
                            yValue = parseFloat(yValue);
                        }

                        if ((dir === "asc" && xValue > yValue) || (dir === "desc" && xValue < yValue)) {
                            [rows[i], rows[i + 1]] = [rows[i + 1], rows[i]];
                            switching = true;
                            switchcount++;
                            break;
                        }
                    }
                    if (!switchcount && dir === "asc") {
                        dir = "desc";
                        switching = true;
                    }
                }
                table.tBodies[0].append(...rows);
            }

            function openTab(panelId, button) {
                document.querySelectorAll(".columnRight").forEach(content => content.style.display = "none");
                document.querySelectorAll(".tab-button").forEach(tabButton => tabButton.classList.remove("active"));

                document.getElementById(panelId).style.display = "flex";
                button.classList.add("active");
            }
            
        </script>
