# PROJE YOL HARİTASI VE TEKNİK YAPILANDIRMA DÖKÜMANI

## 1. PROJE ÖZETİ VE AMAÇ
Bu proje, sosyal ağ sistemlerinde kullanılan property graph veri modelinin sadeleştirilmiş bir versiyonunu geliştirmeyi hedefler. Varlıklar (Kullanıcı, Fotoğraf, Etkinlik vb.) düğüm, bu varlıklar arasındaki ilişkiler ise kenar olarak modellenmiştir. Temel amaç; sıfırdan implemente edilmiş veri yapılarını (Hash Table, Trie, Queue) kullanarak, ana bellek üzerinde çalışan, mikroservis mimarisine sahip ve çok adımlı ilişkisel sorguları verimli şekilde gerçekleştirebilen bir sistem inşa etmektir.

---

## 2. EKİP ROLLERİ VE SORUMLULUKLARI

**Batuhan (Core Data Engineer)**
* Ana Sorumluluk: Temel veri depolama motorunun ve zorunlu veri yapılarının sıfırdan inşası.
* Teknik Görevler: Node/Edge modelleri, Custom Hash Table, Property Graph mimarisinin kurulması, Thread-Safety yönetimi.

**Özcan (Algorithm Master)**
* Ana Sorumluluk: Graf üzerindeki arama, gezinme ve ilişkisel sorgu algoritmalarının yönetimi.
* Teknik Görevler: Custom Queue yazımı, BFS, DFS, Bağlantı Derecesi hesaplama ve çok adımlı ilişkisel sorgu motorunun geliştirilmesi.

**Fatma Sude (Frontend Lead)**
* Ana Sorumluluk: React tabanlı görselleştirme ve kullanıcı etkileşim arayüzü.
* Teknik Görevler: React + TypeScript kurulumu, Vis-network entegrasyonu, dinamik styling ve yan panel etkileşim mantığının kodlanması.

**Muhammed Furkan (Architect & Infrastructure)**
* Ana Sorumluluk: Sistem mimarisi, mikroservis yönetimi ve asenkron süreçlerin koordinasyonu.
* Teknik Görevler: ASP.NET Core Web API mimarisi, REST Controller yönetimi, AI Simulation Worker (IHostedService) geliştirilmesi, Docker konfigürasyonları.

**Isra (Testing & Analysis Specialist)**
* Ana Sorumluluk: Veri yapısı implementasyonu (Trie), sentetik veri üretimi, sistem testi ve performans analizi.
* Teknik Görevler: Custom Trie implementasyonu, sentetik veri üretim motoru, birim/entegrasyon testleri, Big-O analiz dökümantasyonu.

---

## 3. SPRINT PLANLAMASI

### SPRINT 0: Proje Başlatma ve Sistem Mimarisinin Kurulması (Batuhan)
**Hedef:** Repository altyapısının kurulması ve mikroservis yapısının monorepo düzeninde yapılandırılması.
**Süre:** ~1–2 gün (kickstart sprint)
* Sprint 0.1: GitHub Repository kurulumu ve başlangıç README / .gitignore yapılandırması.
* Sprint 0.2: Mikroservis klasör hiyerarşisinin (API, AI, UI) oluşturulması.
* Sprint 0.3: Üye bazlı branch stratejisinin (develop + 5 feature branch) kurgulanması ve uzak sunucuya aktarılması.
* Sprint 0.4: Teknik dökümantasyon taslaklarının (Roadmap.md, InterimReport.md) hazırlanması.
* Sprint 0.5: Planlama ve mimari tartışmaların GitHub Issues üzerinden başlatılması.

---

### SPRINT 1: Altyapı ve Çekirdek Veri Yapıları (Faz 1)
**Hedef:** Standart kütüphane koleksiyonlarını kullanmadan temel veri yapılarının sıfırdan implementasyonu ve projelerin iskeletinin kurulması.

| Kişi | Görev | Detay |
|------|-------|-------|
| **Batuhan** | Node/Edge modelleri + Custom Hash Table | Düğüm ve kenar veri modellerinin tanımlanması, open addressing veya chaining tabanlı Hash Table implementasyonu |
| **Özcan** | Custom Queue + BFS/DFS algoritma iskeletleri | Circular array veya linked-list tabanlı Queue, BFS ve DFS temel traversal fonksiyonlarının yazılması |
| **Fatma Sude** | React + TypeScript proje kurulumu + temel bileşen mimarisi | Proje scaffolding, layout yapısı, arama çubuğu ve sonuç paneli bileşenlerinin oluşturulması |
| **Muhammed Furkan** | ASP.NET Core Web API projesi + API contract tanımlaması | Web API iskeletinin başlatılması, Singleton yaşam döngüsü planlaması, endpoint sözleşmelerinin (request/response modelleri) belirlenmesi |
| **Isra** | Custom Trie implementasyonu + test altyapısı kurulumu | Prefix-tree yapısının sıfırdan kodlanması (insert, search, autocomplete), birim test framework'ünün projeye entegrasyonu |

---

### SPRINT 2: Property Graph Entegrasyonu ve API Servisleri (Faz 1 & 2)
**Hedef:** Veri yapılarının graf mimarisinde birleştirilmesi, dış dünyaya API ile açılması ve sentetik test verisinin üretilmesi.

| Kişi | Görev | Detay |
|------|-------|-------|
| **Batuhan** | Adjacency List tabanlı Property Graph inşası | Sprint 1'deki Hash Table ve Node/Edge modellerinin graf yapısına entegrasyonu, DI konteynırına Singleton kaydı |
| **Özcan** | BFS, DFS ve Bağlantı Derecesi algoritmalarının graf entegrasyonu | Algoritmaların Property Graph üzerinde çalışacak şekilde uyarlanması, shortest path (ağırlıksız) implementasyonu |
| **Fatma Sude** | Frontend API servis katmanı + arama arayüzü | Axios/Fetch ile API bağlantılarının kurulması, arama ve sonuç listeleme ekranlarının kodlanması |
| **Muhammed Furkan** | REST API Controller endpointleri + AI Worker iskelet yapısı | Düğüm/kenar CRUD, arama ve traversal endpointlerinin yazılması; BackgroundService altyapısının hazırlanması |
| **Isra** | Sentetik veri üretim motoru + veri yapıları birim testleri | Programatik sentetik graf verisi üreten modülün yazılması, Hash Table / Trie / Queue için kapsamlı birim testlerin tamamlanması |

---

### SPRINT 3: Çok Adımlı Sorgular, Görselleştirme ve AI Worker (Faz 2 & 3)
**Hedef:** Asenkron veri akışının sağlanması, interaktif ağ haritasının oluşturulması ve ileri düzey sorgu motorunun tamamlanması.

| Kişi | Görev | Detay |
|------|-------|-------|
| **Batuhan** | Thread-safety (locking) mekanizmaları + graf operasyonları optimizasyonu | Tüm yazım/okuma operasyonları için kilit mekanizmalarının implementasyonu, eşzamanlı erişim senaryolarının yönetimi |
| **Özcan** | Çok adımlı ilişkisel sorgu motoru | User → Friends → Events → Photos zincir sorgularının tamamlanması, filtreli graf traversal algoritmalarının yazılması |
| **Fatma Sude** | Vis-network 2D görselleştirme + yan panel etkileşimi | Node-link diyagramı, düğüm tiplerine göre şekil/renk mapping, tıklama ile düğüm özelliklerinin yan panelde gösterimi |
| **Muhammed Furkan** | AI Simulation Worker (IHostedService) | Her 15 saniyede sentetik veri üreten BackgroundService'in tamamlanması, API servisi ile asenkron iletişim |
| **Isra** | Entegrasyon testleri + ilk Big-O analiz taslağı | Servisler arası entegrasyon testlerinin yazılması, AI motoru yük testleri, algoritma karmaşıklıklarının ilk analizleri |

---

### SPRINT 4: Dağıtım, Teknik Analiz ve Finalizasyon (Faz 3 & Teslimat)
**Hedef:** Sistemin paketlenmesi, raporlanması, cross-review yapılması ve sunuma hazır hale getirilmesi.

| Kişi | Görev | Detay |
|------|-------|-------|
| **Batuhan** | UML diyagramları + kod temizliği | Sistem mimarisi ve veri akış diyagramlarının çizimi, kod kalitesi iyileştirmeleri |
| **Özcan** | Big-O analiz tablosu + algoritma dökümantasyonu | Tüm veri yapıları ve algoritmaların zaman/uzay karmaşıklığı analizi, rapor dosyasına işlenmesi |
| **Fatma Sude** | UI son rötuşlar + demo videosu hazırlığı | Responsive düzenlemeler, görsel iyileştirmeler, demo video akışının planlanması |
| **Muhammed Furkan** | Dockerfile + docker-compose konfigürasyonları | API, UI ve AI servisleri için container tanımları, tek komutla (`docker-compose up`) çalışır hale getirme |
| **Isra** | Performans raporu + AI prompt dökümü + final test | AI simülasyonu için kullanılan prompt'ların dökümü, nihai performans test raporu, B.3 isimlendirme kontrolü |
| **Tüm Ekip** | Cross-review (Code Defense hazırlığı) + demo videosu | Her üye en az bir başka üyenin modülünü inceleyip açıklayabilir hale gelir, demo videosu çekilir |

---

## 4. GENEL KURALLAR VE KONVANSIYONLAR
* **İsimlendirme Standardı (B.3):** Tüm sprint'lerde veritabanı, fonksiyon ve değişken adlarında Türkçe karakter kullanılmaz. Bu kural Sprint 1'den itibaren zorunludur.
* **Branch Kuralı:** Hiçbir geliştirici doğrudan `main` veya `develop` dalına push yapamaz. Tüm değişiklikler Pull Request ile birleştirilir.
* **Commit Mesajları:** Her commit açıklayıcı bir mesaj ile atılır (ör. `feat: add custom hash table with chaining`).

---

## 5. BİTTİ TANIMI (DEFINITION OF DONE) — PROJE GENELİ
* Tüm zorunlu veri yapıları (Hash Table, Trie, Queue) hazır kütüphane kullanılmadan sıfırdan yazılmıştır.
* Property Graph, adjacency list tabanlı olarak implemente edilmiştir.
* Sistem mikroservis mimarisinde, asenkron ve thread-safe (B.1) çalışmaktadır.
* Çok adımlı ilişkisel sorgular ve 2D graf görselleştirme (A.2, A.3) tam fonksiyoneldir.
* Proje tek komutla (`docker-compose up`) Docker üzerinden ayağa kalkmaktadır.
* GitHub üzerinde branch yönetimi ve PR mekanizması aktif olarak kullanılmıştır.
* Tüm ekip üyeleri, herhangi bir modülün çalışma mantığını ve karmaşıklığını açıklayabilir durumdadır (Code Defense).
* Big-O analiz raporu, UML diyagramları ve AI prompt dökümü teslim edilmiştir.
