# DONE — Tamamlanan Görevler

---

## SPRINT 0: Proje Başlatma ve Sistem Mimarisinin Kurulması

### Sprint 0.1: GitHub Repo Kurulumu ve Başlangıç Yapılandırması
- [x] GitHub üzerinde SocialGraphProject isimli repository oluşturuldu. (Batuhan)
- [x] Projenin temel özetini, ekip bilgilerini ve çalıştırma talimatlarını içeren başlangıç README.md dosyası hazırlandı. (Batuhan)
- [x] .NET (bin/, obj/) ve Node.js (node_modules/) ekosistemlerine uygun .gitignore dosyası kök dizine eklendi. (Batuhan)
- [x] Proje kök dizininde dökümantasyon takibi için Markdowns/ klasörü oluşturuldu. (Batuhan)
- [x] Süreç yönetimi için TODO.md ve DONE.md dosyaları oluşturuldu. (Batuhan)

### Sprint 0.2: Mikroservis Klasör Yapılandırması (Monorepo)
- [x] src/SocialGraph.API/ dizini oluşturuldu ve placeholder README eklendi. (Batuhan)
- [x] src/SocialGraph.AI/ dizini oluşturuldu ve placeholder README eklendi. (Batuhan)
- [x] src/SocialGraph.UI/ dizini oluşturuldu ve placeholder README eklendi. (Batuhan)

### Sprint 0.3: 5 Kişilik Ekip Yapısına Uygun Branch Stratejisi
- [x] develop branch oluşturuldu ve remote'a push edildi. (Batuhan)
- [x] feature/batuhan-core branch oluşturuldu. (Batuhan)
- [x] feature/ozcan-algorithms branch oluşturuldu. (Batuhan)
- [x] feature/sude-frontend branch oluşturuldu. (Batuhan)
- [x] feature/furkan-infrastructure branch oluşturuldu. (Batuhan)
- [x] feature/isra-optimization branch oluşturuldu. (Batuhan)

### Sprint 0.4: Teknik Dökümantasyon ve Yol Haritası Kaydı
- [x] Roadmap_Full.md dosyası tüm sprint'leri ve rol dağılımlarını içerecek şekilde hazırlandı. (Batuhan)
- [x] Sprints/ dizini altında Sprint 0–4 detay dosyaları oluşturuldu. (Batuhan)
- [x] Interim_Report.md taslağı bölüm başlıklarıyla güncellendi. (Batuhan)

### Sprint 0.5: Köprü Aşaması — GitHub Issues Ön Hazırlığı
- [x] Ekip içi değerlendirme toplantısında HashTable collision yönetimi ve çözümleri tartışıldı. (Batuhan)
- [x] Mikroservisler arası asenkron veri iletişim protokolleri değerlendirildi ve karara bağlandı. (Batuhan)
- [x] Gelecek sprintlerde referans alınacak mimari tasarım kararları dokümante edildi. (Batuhan)
- [x] Alınan takım kararları projenin geliştirme aşamasında ilgili geliştiriciler tarafından GitHub Issues ve Discussions açılarak resmiyete kavuşturuldu. (Örn: HTTP POST vs gRPC, ve Live API vs Offline LLM Prompting tartışmaları) (Batuhan, Furkan, Isra)

---

## SPRINT 1: Altyapı ve Çekirdek Veri Yapıları

### Sprint 1.1: Batuhan (Core Data Engineer) — Node/Edge Modelleri + Custom Hash Table
- [x] C# Console test altyapısı kurularak CustomHashTable (Linear Probing), Node ve Edge veri yapıları %100 from scratch kodlandı. (Batuhan)
- [x] Terminal üzerinden 2500 adet veriyle performans ve O(N) Rehashing simülasyonu (yük testi) gerçekleştirilerek doğrulandı. (Batuhan)
- [x] Yazılan kodlar feature/batuhan-core branch'inden ana havuza (develop) PR açılarak gönderildi. (Batuhan)

### Sprint 1.2: Özcan (Algorithm Master) — Custom Queue + BFS/DFS İskeletleri
- [x] CustomQueue sınıfı dairesel dizi (circular array) tabanlı ve thread-safe (lock) olarak sıfırdan implemente edildi. (Özcan)
- [x] GraphTraversal modülü oluşturularak, BFS ve DFS algoritmaları iskelet olarak yazıldı ve Mock graf eşliğinde doğrulandı. (Özcan)
- [x] Context.md gereksinimleri (Özcan isimlendirmesi, lock ile eşzamanlılık, Big-O yorumları) tam olarak sağlandı. (Özcan)

### Sprint 1.3: Fatma Sude (Frontend Lead) — React + TypeScript Proje Kurulumu
- [x] SocialGraph.UI projesi Vite + React + TypeScript kullanılarak sıfırdan oluşturuldu. (Sude)
- [x] Premium Swiss Minimal tasarım sistemi index.css üzerinde (Glassmorphism, HSL paleti) kuruldu. (Sude)
- [x] AppLayout, SearchBar ve GraphCanvas (placeholder) bileşenleri geliştirildi. (Sude)
- [x] Backend modelleriyle uyumlu TypeScript interface'leri (INode, IEdge vb.) tanımlandı. (Sude)

### Sprint 1.4: Muhammed Furkan (Architect & Infrastructure) — Web API Projesi + API Contract
- [x] Console Application, ASP.NET Core Web API projesine dönüştürüldü (SDK değişimi, Swashbuckle entegrasyonu). (Furkan)
- [x] Program.cs üzerinde CORS, Swagger/OpenAPI, JSON Serialization ve Singleton DI yapılandırması tamamlandı. (Furkan)
- [x] Frontend interface'leriyle birebir uyumlu DTO modelleri (NodeDto, EdgeDto, SearchRequestDto, TraversalResultDto) oluşturuldu. (Furkan)
- [x] NodesController, SearchController ve TraversalController ile toplam 5 placeholder endpoint Swagger üzerinde dökümante edildi. (Furkan)
- [x] CORS ayarları frontend'in localhost portunu kabul edecek şekilde yapılandırıldı. (Furkan)

### Sprint 1.5: Isra (Testing & Analysis Specialist) — Custom Trie + Test Altyapısı
- [x] CustomTrie ve TrieNode sıfırdan implemente edildi (çocuk düğümler için CustomHashTable kullanıldı). (Isra)
- [x] Insert, Search, StartsWith ve AutoComplete operasyonları case-insensitive olarak yazıldı. (Isra)
- [x] xUnit test projesi (SocialGraph.Tests) oluşturuldu ve API projesine referans eklendi. (Isra)
- [x] CustomHashTable, CustomQueue ve CustomTrie için toplam 14 birim test senaryosu yazıldı ve %100 geçti. (Isra)

---

> **SPRINT 1 CHECKPOINT: TAMAMLANDI**
> Tüm alt görevler (1.1 - 1.5) başarıyla tamamlandı. 10/10 kabul kriteri karşılandı.

---

## SPRINT 2: Property Graph Entegrasyonu ve API Servisleri

### Sprint 2.1: Batuhan (Core Data Engineer) — Adjacency List Tabanlı Property Graph
- [x] Adjacency list tabanlı PropertyGraph sınıfı sıfırdan implemente edildi. İç depolama tamamen CustomHashTable ile yapıldı. (Batuhan)
- [x] 3 düğüm türü (User, Photo, Event) ve 4 kenar türü (FRIEND, LIKES, POSTED, ATTENDS) tip doğrulama ile desteklendi. (Batuhan)
- [x] Yönsüz kenarlar (FRIEND) için çift yönlü adjacency kaydı, yönlü kenarlar (LIKES, POSTED, ATTENDS) için tek yönlü kayıt mekanizması kuruldu. (Batuhan)
- [x] ReaderWriterLockSlim ile temel read/write lock altyapısı sağlandı. (Batuhan)
- [x] PropertyGraph Singleton olarak DI container'a kaydedildi. (Batuhan)

### Sprint 2.2: Özcan (Algorithm Master) — Graf Algoritmaları Entegrasyonu
- [x] GraphTraversal sınıfındaki BFS ve DFS metotları, MockGraph yerine PropertyGraph üzerinde çalışacak şekilde güncellendi. (Özcan)
- [x] BFS algoritması kullanılarak iki düğüm arasındaki en kısa yolu bulan ShortestPath metodu eklendi. (Özcan)
- [x] Tüm algoritmalara düğüm ve kenar seviyesinde dinamik filtreleme yeteneği kazandırıldı. (Özcan)
- [x] Tüm operasyonlarda custom veri yapıları kullanılarak standart kütüphane yasağı korundu. (Özcan)

### Sprint 2.3: Fatma Sude (Frontend Lead) — API Servis Katmanı ve Arama Arayüzü
- [x] Backend API servisiyle haberleşecek apiService.ts dosyası native fetch kullanılarak TypeScript tipleriyle projeye kazandırıldı. (Sude)
- [x] nodeService.ts ve traversalService.ts dosyaları oluşturulup, ilgili API çağrıları modüler hale getirildi. (Sude)
- [x] SearchBar.tsx bileşenine asenkron veri çeken ve sonuçları sunan Autocomplete mekanizması eklendi. (Sude)
- [x] Seçilen düğümlerin detay özelliklerini göstermek üzere ResultPanel.tsx bileşeni güncellendi. (Sude)

### Sprint 2.5: Isra (Testing & Analysis Specialist) — Sentetik Veri Üretimi + Birim Testler
- [x] Gemini 3.1 Pro kullanılarak elde edilen 100+ sofistike veri DataGenerator.cs içerisine entegre edildi. **(Isra)**
- [x] Dense, Sparse, Star ve Chain topolojilerinde programatik graf üretebilen algoritmalar yazıldı. **(Isra)**
- [x] AI Worker (Worker.cs) üzerinden üretilen verilerin API'ye batch transferi sağlandı. **(Isra)**
- [x] Toplam birim test sayısı 23'e çıkarıldı ve %100 başarıyla geçti. **(Isra)**

---

> **SPRINT 2 CHECKPOINT: TAMAMLANDI**
> PropertyGraph entegrasyonu, API servislerinin frontend ile bağlanması, AI Worker veri simülasyonu ve 23 birim test senaryosu %100 başarıyla tamamlandı. Sprint 3'e geçiş onaylandı.

---

## SPRINT 3: Çok Adımlı Sorgular ve Görselleştirme

### Sprint 3.1: Batuhan (Core Data Engineer) — Thread-Safety ve Eşzamanlılık Yönetimi
- [x] `PropertyGraph` üzerindeki tüm operasyonlar `ReaderWriterLockSlim` ile thread-safe hale getirildi. **(Batuhan)**
- [x] Yazma kilitlerinin (Write Lock) kapsamı daraltılarak (reverse edge hazırlığı kilit dışına alındı) performans artışı sağlandı. **(Batuhan)**
- [x] `GetAllEdges` metodu `_edgeCount` kullanılarak tek geçişte çalışacak şekilde optimize edildi. **(Batuhan)**
- [x] 15 okuyucu ve 2 yazıcı thread ile yapılan 30 saniyelik yük testinde deadlock ve race condition oluşmadığı doğrulandı. **(Batuhan)**
- [x] Eşzamanlı okuma/yazma senaryolarını içeren `PropertyGraphConcurrencyTests.cs` xUnit projesine eklendi. **(Batuhan)**

### Sprint 3.2: Özcan (Algorithm Master) — Çok Adımlı İlişkisel Sorgu Motoru
- [x] `RelationalQueryEngine.cs` implemente edilerek, `User → FRIEND → Event` gibi sınırsız derinlikte zincir sorgu desteği eklendi. **(Özcan)**
- [x] Her adımda benzersiz düğüm setleri oluşturularak (O(N) karmaşıklıkta) ara sonuçların doğru aktarımı sağlandı. **(Özcan)**
- [x] "Triadic Closure" prensibiyle ortak arkadaş sayısına dayalı arkadaş öneri sistemi (Recommendation Engine) geliştirildi. **(Özcan)**
- [x] Zincir sorgu motoru, karmaşık `User→Friend→Event→Photo` senaryoları ile `RelationalQueryTests.cs` üzerinden doğrulandı. **(Özcan)**
- [x] `PropertyGraph`'a `UPLOADED` kenar türü eklenerek projenin ilişkisel modeli genişletildi. **(Özcan)**
