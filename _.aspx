    <form id="form1" runat="server" onkeypress="return preventEnter(event)">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <header>
            <div class="header">
                <div id="logo" role="link" aria-label="Anasayfa" onclick="window.location.href='/'"></div>
                <h1 class="baslik" id="baslik" runat="server">CloudUnited Servisler İçin Yönetim Paneli</h1>
            </div>
        </header>

        <main class="container">
            <h2>Web Config Dosyasındaki Yetkili Kişiler</h2>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="form-row">
                        <label for="ddlConfigFiles">Config Dosyasını Seçin:</label>
                        <asp:DropDownList 
                            ID="ddlConfigFiles" 
                            runat="server" 
                            AutoPostBack="true" 
                            OnSelectedIndexChanged="ddlConfigFiles_SelectedIndexChanged" 
                            aria-label="Config dosyasını seçin">
                            <asp:ListItem Text="Bir config dosyası seçin" Value="" />
                        </asp:DropDownList>
                    </div>

                    <div class="table-container">
                        <asp:GridView 
                            ID="gvAuthorizedUsers" 
                            runat="server" 
                            AutoGenerateColumns="False" 
                            OnRowCommand="gvAuthorizedUsers_RowCommand" 
                            CssClass="table" 
                            EmptyDataText="Hiç kullanıcı yok">
                            <Columns>
                                <asp:BoundField DataField="UserName" HeaderText="Kullanıcı Adı" SortExpression="UserName" />
                                <asp:TemplateField HeaderText="İşlem">
                                    <ItemTemplate>
                                        <asp:Button 
                                            ID="btnRemove" 
                                            runat="server" 
                                            Text="Kaldır" 
                                            CommandName="Remove" 
                                            CommandArgument='<%# Eval("UserName") %>' 
                                            CssClass="btn-remove" 
                                            OnClientClick="return confirmDelete();" 
                                            aria-label="Kullanıcıyı kaldır" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <br />
                    <div class="form-row">
                        <asp:TextBox 
                            ID="txtUserName" 
                            runat="server" 
                            placeholder="Kullanıcı Adı Girin" 
                            aria-label="Kullanıcı Adı" />
                        <asp:Button 
                            ID="btnAddUser" 
                            runat="server" 
                            Text="Kullanıcı Ekle" 
                            CssClass="button" 
                            OnClick="btnAddUser_Click" 
                            aria-label="Kullanıcı ekle" />
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAddUser" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

            <p class="footer">Kullanıcı adı kısmına ‘ * ’ yazmak herkesin erişimine izin verir...</p>
            <div id="errorMessage" class="error-message" runat="server"></div>
        </main>
        
        <footer>
            <p class="footer">© 2024 - Cloud United Team</p>
        </footer>

        <script>
            // Enter tuşuna basıldığında formun gönderilmesini engelleme
            function preventEnter(event) {
                if (event.key === "Enter") {
                    event.preventDefault();
                    return false;
                }
            }

            // Kaldır butonuna tıklanmadan önce onay istemek
            function confirmDelete() {
                return confirm("Bu kullanıcıyı kaldırmak istediğinizden emin misiniz?");
            }

            // Logo tıklama olayıyla anasayfaya yönlendirme
            document.getElementById('logo').addEventListener('click', function () {
                window.location.href = '/';
            });
        </script>
    </form>
