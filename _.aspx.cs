        void loadServices(Excel.Worksheet ws)
        {
            Excel.Range range = ws.UsedRange;
            for (int i = 2; i <= range.Rows.Count; i++)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "service-panel");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.ID = range.Rows[i].Cells[3].Text;
                h3.InnerText = range.Rows[i].Cells[1].Text;
                HtmlGenericControl p = new HtmlGenericControl("p");
                p.InnerText = range.Rows[i].Cells[2].Text;
                div.Controls.Add(h3);
                div.Controls.Add(p);
                services.Controls.Add(div);
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
        }




                <div class="serv-group">
                    <div class="serv-group-title">Backend Projects</div>
                    <div class="serv-item">
                        <h2 class="serv-title">
                            API Server <span class="serv-meta">(Public)</span>
                        </h2>
                        <p class="serv-desc">gres sofkijmsoa dojkf nsdjkfn </p>
                    </div>
                    <div class="serv-item">
                        <h2 class="serv-title">
                        Database Manager <span class="serv-meta">(Public)</span>
                        </h2>
                        <p class="serv-desc">s dsfsd f,,df efewnia d</p>
                    </div>
                </div>

Servis Adı;	Servis Açıklaması;	Servis Linki(IIS Site Adı);	Kategori;	Meta;
Fiziksel Sunucu Envanteri	Fiziksel Envanter görüntüle ve düzenle...	fiziksel	Inventory	
Fiziksel Sunucu Envanteri(Historical)	Envanteri tarihe göre incele...	fizikselHistorical	Historic	
Fiziksel Sunucu Envanteri(Otomasyon)	Fiziksel Envanter görüntüle	hwpedia	Inventory	
Fiziksel Sunucu Envanteri(Otomasyon-Historical)	Fiziksel otomasyon envanterini tarihe göre incele...	hwpediaHistorical	Historic	Public
VMWare Envanter	VMWare envanter görüntüle...	vmpedia	Inventory	
RvTools Backups	RvTools Envanter Yedeklemeleri Görüntüle...	rvtools	Backups	
Script Control DashBoard	Otomasyondaki Scriptleri Kontrol Et...	ScriptControl	Admin	
ODM Sanal Sunucu Envanteri	Pendik replikasyonlu TEST, PRD sunucuları görüntüle	odmvms	Inventory	Public
Created and Deleted VMs	VMWare silinen ve oluşturulan VMlerı görüntüle..	createdanddeletedvms	Inventory	
Pendik Hall Lists	Pendik Hall Ayrımlı VMleri görüntüle..	hallLists	Inventory	
CloudUnited Servis Yonetim Paneli	Servisleri için Erişimleri Yönet..	authconfiger	Admin	






                            
