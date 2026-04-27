# DONE — Tamamlanan Gorevler

---

## SPRINT 0: Proje Başlatma ve Sistem Mimarisinin Kurulması

### Sprint 0.1: GitHub Repo Kurulumu ve Başlangıç Yapılandırması
- [x] GitHub üzerinde "SocialGraphProject" isimli repository oluşturuldu. **(Batuhan)**
- [x] Projenin temel özetini, ekip bilgilerini ve çalıştırma talimatlarını içeren başlangıç README.md dosyası hazırlandı. **(Batuhan)**
- [x] .NET (`bin/`, `obj/`) ve Node.js (`node_modules/`) ekosistemlerine uygun .gitignore dosyası kök dizine eklendi. **(Batuhan)**
- [x] Proje kök dizininde dökümantasyon takibi için `Markdowns/` klasörü oluşturuldu. **(Batuhan)**
- [x] Süreç yönetimi için `TODO.md` ve `DONE.md` dosyaları oluşturuldu. **(Batuhan)**

### Sprint 0.2: Mikroservis Klasor Yapilandirmasi (Monorepo)
- [x] `src/SocialGraph.API/` dizini olusturuldu ve placeholder README eklendi. **(Batuhan)**
- [x] `src/SocialGraph.AI/` dizini olusturuldu ve placeholder README eklendi. **(Batuhan)**
- [x] `src/SocialGraph.UI/` dizini olusturuldu ve placeholder README eklendi. **(Batuhan)**

### Sprint 0.3: 5 Kisilik Ekip Yapisina Uygun Branch Stratejisi
- [x] `develop` branch olusturuldu ve remote'a push edildi. **(Batuhan)**
- [x] `feature/batuhan-core` branch olusturuldu. **(Batuhan)**
- [x] `feature/ozcan-algorithms` branch olusturuldu. **(Batuhan)**
- [x] `feature/sude-frontend` branch olusturuldu. **(Batuhan)**
- [x] `feature/furkan-infrastructure` branch olusturuldu. **(Batuhan)**
- [x] `feature/isra-optimization` branch olusturuldu. **(Batuhan)**

### Sprint 0.4: Teknik Dökümantasyon ve Yol Haritası Kaydı
- [x] `Roadmap_Full.md` dosyası tüm sprint'leri ve rol dağılımlarını içerecek şekilde hazırlandı. **(Batuhan)**
- [x] `Sprints/` dizini altında Sprint 0–4 detay dosyaları oluşturuldu. **(Batuhan)**
- [x] `Interim_Report.md` taslağı bölüm başlıklarıyla güncellendi. **(Batuhan)**

### Sprint 0.5: Köprü Aşaması — GitHub Issues Ön Hazırlığı
- [x] Ekip içi değerlendirme toplantısında HashTable collision yönetimi ve çözümleri tartışıldı. **(Batuhan)**
- [x] Mikroservisler arası asenkron veri iletişim protokolleri değerlendirildi ve karara bağlandı. **(Batuhan)**
- [x] Gelecek sprintlerde referans alınacak mimari tasarım kararları dokümante edildi. **(Batuhan)**
- [x] Alınan takım kararları projenin geliştirme aşamasında ilgili geliştiriciler tarafından GitHub Issues ve Discussions açılarak resmiyete kavuşturuldu. **(Batuhan, Furkan)**

## SPRINT 1: Altyapı ve Çekirdek Veri Yapıları

### Sprint 1.1: Batuhan (Core Data Engineer) — Node/Edge Modelleri + Custom Hash Table
- [x] C# Console test altyapısı kurularak `CustomHashTable` (Linear Probing), `Node` ve `Edge` veri yapıları %100 "from scratch" kodlandı. **(Batuhan)**
- [x] Terminal üzerinden 2500 adet veriyle performans ve O(N) Rehashing simulasyon (yük testi) gerçekleştirilerek doğrulandı. **(Batuhan)**
- [x] Yazılan kodlar `feature/batuhan-core` branch'inden ana havuza (`develop`) PR açılarak gönderildi. **(Batuhan)**

### Sprint 1.2: Özcan (Algorithm Master) — Custom Queue + BFS/DFS İskeletleri
- [x] `CustomQueue` sınıfı dairesel dizi (circular array) tabanlı ve thread-safe (`lock`) olarak sıfırdan implemente edildi. **(Özcan)**
- [x] `GraphTraversal` modülü oluşturularak, BFS ve DFS algoritmaları iskelet olarak yazıldı ve Mock graf eşliğinde doğrulandı. **(Özcan)**
- [x] `Context.md` gereksinimleri (Özcan isimlendirmesi, lock ile eşzamanlılık, Big-O yorumları) tam olarak sağlandı. **(Özcan)**

### Sprint 1.3: Fatma Sude (Frontend Lead) — React + TypeScript Proje Kurulumu
- [x] `SocialGraph.UI` projesi Vite + React + TypeScript kullanılarak sıfırdan oluşturuldu. **(Sude)**
- [x] **Premium Swiss Minimal** tasarım sistemi (Glassmorphism, HSL paleti) `index.css` üzerinde kuruldu. **(Sude)**
- [x] `AppLayout`, `SearchBar` ve `GraphCanvas` (placeholder) bileşenleri geliştirildi. **(Sude)**
- [x] Backend modelleriyle uyumlu TypeScript interface'leri (`INode`, `IEdge` vb.) tanımlandı. **(Sude)**

### Sprint 1.4: Muhammed Furkan (Architect & Infrastructure) — Web API Projesi + API Contract
- [x] Console Application, ASP.NET Core Web API projesine donusturuldu (SDK degisimi, Swashbuckle entegrasyonu). **(Furkan)**
- [x] Program.cs uzerinde CORS, Swagger/OpenAPI, JSON Serialization ve Singleton DI yapilandirmasi tamamlandi. **(Furkan)**
- [x] Frontend interface'leriyle birebir uyumlu DTO modelleri (NodeDto, EdgeDto, SearchRequestDto, TraversalResultDto) olusturuldu. **(Furkan)**
- [x] NodesController, SearchController ve TraversalController ile toplam 5 placeholder endpoint Swagger uzerinde dokumante edildi. **(Furkan)**
- [x] CORS ayarlari frontend'in localhost portunu kabul edecek sekilde yapilandirildi. **(Furkan)**

### Sprint 1.5: Isra (Testing & Analysis Specialist) — Custom Trie + Test Altyapisi
- [x] `CustomTrie` ve `TrieNode` sifirdan implemente edildi (cocuk dugumler icin CustomHashTable kullanildi). **(Isra)**
- [x] Insert, Search, StartsWith ve AutoComplete operasyonlari case-insensitive olarak yazildi. **(Isra)**
- [x] xUnit test projesi (`SocialGraph.Tests`) olusturuldu ve API projesine referans eklendi. **(Isra)**
- [x] CustomHashTable, CustomQueue ve CustomTrie icin toplam 14 birim test senaryosu yazildi ve %100 gecti. **(Isra)**

---

> **SPRINT 1 CHECKPOINT: TAMAMLANDI**
> Tum alt gorevler (1.1 - 1.5) basariyla tamamlandi. 10/10 kabul kriteri karsilandi.
> Zorunlu veri yapilari (Hash Table, Queue, Trie), Node/Edge modelleri, BFS/DFS iskeletleri,
> React + TS frontend iskeleti, Web API + Swagger altyapisi ve 14 birim test senaryosu %100 hazir.
> Sprint 2'ye gecis icin onay verildi.

## SPRINT 2: Property Graph Entegrasyonu ve API Servisleri

### Sprint 2.1: Batuhan (Core Data Engineer) — Adjacency List Tabanli Property Graph
- [x] Adjacency list tabanli `PropertyGraph` sinifi sifirdan implemente edildi. Ic depolama tamamen `CustomHashTable` ile yapildi (standart kutuphane yasagi tam uyum). **(Batuhan)**
- [x] 3 dugum turu (User, Photo, Event) ve 4 kenar turu (FRIEND, LIKES, POSTED, ATTENDS) tip dogrulama ile desteklendi. **(Batuhan)**
- [x] Yonsuz kenarlar (FRIEND) icin cift yonlu adjacency kaydi, yonlu kenarlar (LIKES, POSTED, ATTENDS) icin tek yonlu kayit mekanizmasi kuruldu. **(Batuhan)**
- [x] `ReaderWriterLockSlim` ile temel read/write lock altyapisi saglandi (Context.md B.1 eszemanlilik gereksinimi). **(Batuhan)**
- [x] `PropertyGraph` Singleton olarak DI container'a kaydedildi; mevcut Sprint 1 kayitlari (CustomHashTable, CustomTrie) korundu. **(Batuhan)**
- [x] `dotnet build` 0 hata, `dotnet test` 14/14 test yesil (regresyon yok) ile dogrulandi. **(Batuhan)**

### Sprint 2.2: Özcan (Algorithm Master) — Graf Algoritmaları Entegrasyonu
- [x] `GraphTraversal` sınıfındaki BFS ve DFS metotları, `MockGraph` yerine `PropertyGraph` üzerinde çalışacak şekilde güncellendi. **(Özcan)**
- [x] BFS algoritması kullanılarak iki düğüm arasındaki en kısa yolu bulan ve düğüm ID'lerini `string[]` olarak döndüren `ShortestPath` metodu eklendi. **(Özcan)**
- [x] Tüm algoritmalara düğüm (`Func<Node, bool>`) ve kenar (`Func<Edge, bool>`) seviyesinde dinamik filtreleme yeteneği kazandırıldı. **(Özcan)**
- [x] Yol bulma (parent takibi) ve ziyaret edilen düğüm (visited set) operasyonları için `%100` oranında `CustomHashTable` ve `CustomQueue` kullanılarak standart kütüphane yasağı korundu. **(Özcan)**

### Sprint 2.3: Fatma Sude (Frontend Lead) — API Servis Katmanı ve Arama Arayüzü
- [x] Backend API servisiyle haberleşecek `apiService.ts` dosyası, `axios` yerine native `fetch` kullanılarak TypeScript tipleriyle projeye kazandırıldı. **(Sude)**
- [x] `nodeService.ts` ve `traversalService.ts` dosyaları oluşturulup, ilgili API çağrıları (CRUD ve algoritmalar) modüler hale getirildi. **(Sude)**
- [x] `SearchBar.tsx` bileşenine asenkron veri çeken ve sonuçları dropdown şeklinde sunan bir "Autocomplete" mekanizması eklendi. **(Sude)**
- [x] Seçilen düğümlerin detay özelliklerini göstermek üzere `ResultPanel.tsx` bileşeni güncellendi ve Redux kullanmadan `AppLayout` üzerinden state yönetimi sağlandı. **(Sude)**

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
