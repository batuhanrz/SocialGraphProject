# Ara Rapor (Interim Report) — 30.04.2026

Not: Bu doküman, Veri Yapıları Projesi kapsamında 30.04.2026 tarihli Ara Rapor teslimi için hazırlanan temel iskeleti yansıtmaktadır. Mevcut haliyle ara rapor kriterleri karşılanmış olup, teslim gününe kadar geliştirme süreci devam ettiği için ekip üyeleri tarafından rapora yeni eklemeler/düzenlemeler gelebilir.

Bu doküman, Veri Yapıları Projesi kapsamında 30.04.2026 tarihli Ara Rapor (Interim Report) teslimi için hazırlanmıştır. 

Projenin başlangıcından bu güne kadar yapılan mimari tartışmalar, teknoloji seçimleri, iş bölümü ve kod entegrasyonu detayları aşağıda sunulmuştur. Tüm çalışmalar master/develop branch'i üzerinde toplanmakta olup, projeye ait repository github üzerinde aktif olarak kullanılmaktadır.

## 1. Proje Durumu ve GitHub Güncel Durumu

Projenin temel iskeleti, mikroservis mimarisine ve takım çalışmasına uygun olacak şekilde kurgulanmıştır:
- **Repository:** Proje için [SocialGraphProject] adlı GitHub repository'si oluşturuldu.
- **Branch Stratejisi:** Projede ana entegrasyon dalı olarak develop ve main kullanılmaktadır. Her ekip üyesi (Batuhan, Özcan, Fatma Sude, Muhammed Furkan, Isra) kendi feature/* dallarında çalışmalarını yürütmektedir.
- **Pull Requests (PR) Stratejisi:** Projede profesyonel bir kod birleştirme hiyerarşisi kurgulanmıştır. Mini sprint adımlarında (örneğin Sprint 1.1) her ekip üyesi geliştirme yaptığı kendi feature/* branch'inden ortak develop branch'ine PR açar ve kodlar buraya entegre edilir. Bir sprintin tamamı bittiğinde ise, develop branch'inden main (ana) branch'ine tek bir ana sürüm PR'ı açılarak release edilir.
- **Görev Takibi:** Markdowns/Sprints ve Markdowns/Roadmaps klasörleri altında projenin tüm fazları, haftalık görevleri ve yapılan işler (TODO.md, DONE.md) izlenmektedir.

## 2. Teknoloji Seçim Kararları (Tech Stack)

Ekip içi yapılan tartışmalar ve projenin gereksinimleri doğrultusunda aşağıdaki teknolojiler seçilmiştir:
* **Backend:** ASP.NET Core Web API. Kapsamlı dependency injection ve yüksek performanslı sunucu yetenekleri nedeniyle tercih edildi.
* **Veri Yapıları:** Custom Hash Table (Linear Probing ile), Custom Queue, Custom Trie ve PropertyGraph modelleri hiçbir standart kütüphane kullanılmadan sıfırdan C# ile yazılmıştır. Bellekte tek bir instance (Singleton) olarak yaşar.
* **Frontend:** React + TypeScript. Kullanıcı arayüzü ve dinamik arama sonuçları için seçildi. Graf görselleştirmesi Sprint 3'te Vis-network kütüphanesi ile sağlanacaktır.
* **AI Worker:** SocialGraph.AI adlı bağımsız BackgroundService. Sentetik veri üretimi ve simülasyon işlemleri için tasarlanmıştır.

## 3. Tamamlanan Aşamalar (Sprint 1 & 2)

### Sprint 1: Çekirdek Altyapı
- Temel veri yapıları (HashTable, Queue, Trie) implemente edildi.
- API ve UI projeleri başlatıldı.
- xUnit ile birim test altyapısı kuruldu.

### Sprint 2: Property Graph ve Servis Entegrasyonu
- Adjacency list tabanlı Property Graph mimarisi tamamlandı.
- BFS, DFS ve Shortest Path algoritmaları Property Graph'a entegre edildi.
- AI Worker aracılığıyla otomatik veri üretimi ve API'ye batch transfer mekanizması kuruldu.
- Custom Trie üzerinden dinamik arama (autocomplete) frontend ile bağlandı.

## 4. Kod Entegrasyonu ve Ara Rapor Guncellemesi

Su ana kadar yapilan Sprint 1, Sprint 2 ve Sprint 3.1-3.2 entegrasyonlarinda:
- Cekirdek `Node` ve `Edge` siniflari baglandi.
- Grafin temel iskeleti olan `PropertyGraph` adjacency list tabanli olarak tamamlandi.
- **Eşzamanlılık (Concurrency):** `ReaderWriterLockSlim` kullanılarak API okumaları ile AI Worker yazmaları arasındaki yarış durumları (race conditions) engellendi.
- **İlişkisel Sorgu Motoru:** `RelationalQueryEngine` ile `User → Friend → Event → Photo` gibi çok adımlı zincir sorgular ve ortak arkadaş sayısına dayalı öneri sistemi sisteme kazandırıldı. (Özcan)
- **Interaktif Görselleştirme:** `vis-network` kütüphanesi kullanılarak grafın 2D node-link diyagramı oluşturuldu. Düğüm tipleri (renk/şekil) ve kenar tipleri (çizgi stilleri) görsel olarak ayrıştırılarak "Premium Swiss Minimal" tasarım diline uygun bir arayüz geliştirildi. (Sude)
- **Kullanıcı Deneyimi:** Sorgu paneli ve yan panel etkileşimi ile BFS, DFS ve Zincir Sorgu sonuçlarının anlık olarak graf üzerinde vurgulanması sağlandı. (Sude)
- **AI Simulation (Faz 2):** AI Worker, sadece statik veri basan bir yapıdan; her 15 saniyede bir dinamik olarak yeni düğümler ve ilişkiler üreten, resilient (hataya dayanıklı) bir simülasyon motoruna dönüştürüldü. (Furkan)
- **Sentetik Veri:** AI Worker aracılığıyla 100+ düğüm ve ilişkiden oluşan gerçekçi veri akışı API üzerinden graf sistemine bağlandı. (Isra & Furkan)
- **Doğrulama ve Analiz:** Sistemin 5000+ düğümde bile 10ms altında performans sergilediği yük testleri ile kanıtlandı ve teorik Big-O analiz raporu (`BigO_Analysis.md`) tamamlandı. (Isra)
- API uçları (Nodes, Search, Traversal) ve Frontend (React + TS) arasındaki iletişim native fetch servisleriyle sağlandı.

**Sonuç ve Teslim:**
Sprint 3'ün tamamlanmasıyla birlikte SocialGraph projesinin teknik gereksinimleri %100 karşılanmıştır. Proje, Docker altyapısı ve kapsamlı analiz raporlarıyla birlikte final teslimine hazırdır.
