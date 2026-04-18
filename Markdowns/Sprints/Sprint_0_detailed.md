# SPRINT 0: PROJE BAŞLATMA VE SİSTEM MİMARİSİNİN KURULMASI

**Sorumlu:** Batuhan (Core Data Engineer)
**Süre:** ~1–2 gün (kickstart sprint)
**Hedef:** GitHub repository altyapısının kurulması, projenin mikroservis mimarisine uygun şekilde yapılandırılması ve 5 kişilik ekip yapısına göre geliştirme ortamının hazır hale getirilmesi.

---

### Sprint 0.1: GitHub Repo Kurulumu ve Başlangıç Yapılandırması

Batuhan tarafından projenin ana deposu GitHub üzerinde oluşturulur. İlk yapılandırma kapsamında aşağıdaki adımlar uygulanır:

- GitHub üzerinde **"SocialGraphProject"** isimli public repository'nin oluşturulması.
- Projenin temel özetini, ekip bilgilerini ve çalıştırma talimatlarını içeren başlangıç **README.md** dosyasının hazırlanması.
- .NET ve Node.js ekosistemlerine uygun, gereksiz dosyaların takibini engelleyen **.gitignore** dosyasının kök dizine eklenmesi.
- Proje kök dizininde dökümantasyon takibi için **Markdowns/** klasörünün oluşturulması.
- Süreç yönetimi için **TODO.md** (yapılacaklar) ve **DONE.md** (tamamlananlar) dosyalarının oluşturulması.

**Kabul Kriterleri:**
- [x] Repository GitHub'da erişilebilir durumda.
- [x] README.md en az proje adını, amacını ve ekip üyelerini içeriyor.
- [x] .gitignore dosyası .NET (`bin/`, `obj/`) ve Node.js (`node_modules/`) patterns'larını barındırıyor.
- [x] `Markdowns/` klasörü, `TODO.md` ve `DONE.md` dosyaları kök dizinde mevcut.

---

### Sprint 0.2: Mikroservis Klasör Yapılandırması (Monorepo)

Proje mimarisi, gereksinim dokümanındaki B.1 maddesinde yer alan "Mikroservis Yaklaşımı" kriterine uygun olarak monorepo düzeninde tasarlanır. Aşağıdaki bağımsız servis klasörleri oluşturulur:

- **src/SocialGraph.API/** — Property Graph verilerinin ve RAM tabanlı veri yapılarının tutulduğu ana iş mantığı servisi.
- **src/SocialGraph.AI/** — Veri simülasyonunu asenkron olarak yürütecek, ana API servisinden bağımsız çalışacak mikroservis.
- **src/SocialGraph.UI/** — React ve TypeScript tabanlı kullanıcı arayüzü bileşenlerinin yer alacağı frontend projesi.

**Kabul Kriterleri:**
- [x] `src/SocialGraph.API/`, `src/SocialGraph.AI/`, `src/SocialGraph.UI/` dizinleri oluşturuldu.
- [x] Her dizinde en az bir placeholder dosya (README.md veya .gitkeep) mevcut.
- [x] Klasör yapısı `git ls-tree` veya dosya gezgini ile doğrulandı.

---

### Sprint 0.3: 5 Kişilik Ekip Yapısına Uygun Branch Stratejisi

Geliştirme sürecinin disiplinli ilerlemesi ve B.1 kuralındaki "her üyenin kendi branch yapısını kullanması" şartını sağlamak adına şu dallanma modeli kurgulanır:

- **develop** — Tüm geliştirmelerin `main` branch'e aktarılmadan önce toplandığı ana entegrasyon dalı.
- **feature/batuhan-core** — Çekirdek veri yapıları ve ana mimari geliştirmeleri için ayrılmış çalışma dalı.
- **feature/ozcan-algorithms** — Graf algoritmaları ve traversal işlemleri için ayrılmış çalışma dalı.
- **feature/sude-frontend** — Arayüz ve görselleştirme çalışmaları için ayrılmış çalışma dalı.
- **feature/furkan-infrastructure** — Sistem altyapısı ve asenkron servis yönetimi için ayrılmış çalışma dalı.
- **feature/isra-optimization** — Test, analiz ve optimizasyon çalışmaları için ayrılmış çalışma dalı.

**Kabul Kriterleri:**
- [x] `develop` + 5 adet `feature/*` branch oluşturuldu.
- [x] Tüm branch'ler remote'a push edildi (`git branch -r` ile doğrulandı).
- [x] Her ekip üyesi kendi branch'ine checkout yapabildi (üyelere bildirildi).

---

### Sprint 0.4: Teknik Dökümantasyon ve Yol Haritası Kaydı

Sürecin izlenebilirliği için `Markdowns/` dizini altında teknik dokümanlar oluşturulur:

- **Markdowns/Roadmaps/Roadmap_Full.md** — 5 kişilik ekip dağılımına göre 5 sprint planını ve görev dağılımını içeren detaylı yol haritası.
- **Markdowns/Sprints/** — Her sprint'in ayrıntılı görev tanımlarını ve DoD'lerini içeren sprint dosyaları dizini.
- **Markdowns/Project/Interim_Report.md** — Ara rapor gereksinimlerini karşılamak amacıyla; teknoloji seçim kararları ve teknik bulguların kaydedileceği doküman taslağı.

**Kabul Kriterleri:**
- [x] `Roadmap_Full.md` dosyası tüm sprint'leri ve rol dağılımlarını içeriyor.
- [x] `Sprints/` dizini altında en az Sprint 0 detay dosyası mevcut.
- [x] `Interim_Report.md` taslak olarak oluşturuldu ve bölüm başlıkları tanımlı.

---

### Sprint 0.5: Köprü Aşaması — GitHub Issues Ön Hazırlığı

Ara rapor tesliminden önce tamamlanması zorunlu olan "ekip içi teknik tartışma ve bulgu kayıtları" için altyapı hazırlanmıştır. Ancak bu tartışmaların Sprint 1 ve Sprint 2 geliştirme süreçleriyle doğal bir şekilde senkronize ilerlemesi gerekmektedir. Bu nedenle Sprint 0.5, sonraki sprintler (Sprint 1.1 vb.) için bir köprü vazifesi görür.

- **[Mimari Karar]** HashTable veri yapısının bellek dostu olması (Open Addressing) hedeflendi, implementasyon aşamasında referans alınacaktır.
- **[Teknik Tartışma]** AI ve API mikroservisleri arasındaki asenkron veri akışında REST HTTP + Retry bazlı altyapı tasarımına onay verildi.

**Kabul Kriterleri:**
- [x] Ekip içi değerlendirme toplantısında HashTable collision stratejisi ve AI-API asenkron iletişim protokolü tartışılıp karara bağlandı.
- [ ] Alınan bu takım kararları projenin geliştirme aşamasında (Sprint 1 ve Sprint 2) ilgili geliştiriciler tarafından GitHub Issues üzerinden resmiyete kavuşturuldu. (Şimdilik Beklemede)

---

## Sprint 0: Bitti Tanımı (Definition of Done)

| # | Kriter | Doğrulama Yöntemi |
|---|--------|-------------------|
| 1 | GitHub repository'si kuruldu ve yönetim dosyaları (.gitignore, README, TODO, DONE) eklendi | Repository URL'sine tarayıcıdan erişim + dosya listesi kontrolü |
| 2 | Mikroservis mimarisine uygun 3 ayrı klasör (API, AI, UI) hiyerarşisi oluşturuldu | `src/` altında 3 dizin + placeholder dosyaların varlığı |
| 3 | `develop` + 5 adet `feature/*` branch tanımlandı ve remote'a push edildi | `git branch -r` çıktısında 6 branch (develop + 5 feature) görünür |
| 4 | Roadmap, Sprint dokümanları ve ara rapor taslağı dökümantasyon dizinine eklendi | `Markdowns/` altında ilgili dosyaların varlığı |
| 5 | GitHub Issues üzerinden en az 3 tartışma/bulgu kaydı açıldı | Issues sekmesinde 3 açık issue + açıklama + etiket |
| 6 | Tüm ekip üyeleri kendi branch'lerine erişebildi | Ekip üyelerinden onay alındı veya test checkout yapıldı |