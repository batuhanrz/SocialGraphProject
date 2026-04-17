# SPRINT 1: ALTYAPI VE ÇEKİRDEK VERİ YAPILARI (FAZ 1)

**Hedef:** Standart kütüphane koleksiyonlarını (Dictionary, List, Queue vb.) kullanmadan temel veri yapılarının sıfırdan implementasyonu ve her servisin proje iskeletinin kurulması.

**Kapsam:** Context.md — A.1 (Zorunlu Veri Yapıları)

---

## Görev Dağılımı

### Batuhan (Core Data Engineer) — Node/Edge Modelleri + Custom Hash Table

**Görev 1: Node ve Edge Veri Modelleri**
Sistemin temel yapı taşları olan düğüm ve kenar modellerinin tasarlanması:
- `Node` sınıfı: benzersiz ID (string/int), tür bilgisi (User, Photo, Event), özellikler (properties dictionary — kendi Hash Table'ı ile).
- `Edge` sınıfı: kaynak düğüm ID, hedef düğüm ID, ilişki türü (FRIEND, LIKES, ATTENDS, UPLOADED), yön bilgisi (directed/undirected), ek özellikler (tarih vb.).
- Modellerin serileştirilebilir (serializable) olması sağlanır.

**Görev 2: Custom Hash Table**
Düğümlere O(1) ortalama erişim sağlayacak hash tablosu implementasyonu:
- Collision resolution: Separate Chaining veya Open Addressing (Linear/Quadratic Probing).
- Dinamik kapasite artırımı (rehashing): Load factor %75'i aştığında otomatik resize.
- Temel operasyonlar: `Put(key, value)`, `Get(key)`, `Remove(key)`, `ContainsKey(key)`, `Count`, `Keys()`.
- Generic tip desteği: `CustomHashTable<TKey, TValue>`.

**Kabul Kriterleri:**
- [ ] Node ve Edge modelleri oluşturuldu, her biri en az 3 farklı türle test edildi.
- [ ] Hash Table, standart kütüphane kullanılmadan sıfırdan yazıldı.
- [ ] Hash Table'a 1000+ eleman ekleme/arama işlemi başarıyla gerçekleştirildi.
- [ ] Rehashing mekanizması çalışıyor (load factor aşıldığında kapasite artıyor).
- [ ] Kod `feature/batuhan-core` branch'inde ve PR açıldı.

---

### Özcan (Algorithm Master) — Custom Queue + BFS/DFS İskeletleri

**Görev 1: Custom Queue**
BFS algoritmasında kullanılacak kuyruk veri yapısının implementasyonu:
- Circular Array tabanlı veya Linked-List tabanlı implementasyon.
- Temel operasyonlar: `Enqueue(item)`, `Dequeue()`, `Peek()`, `IsEmpty`, `Count`.
- Dinamik kapasite artırımı (array tabanlı tercih edilirse).
- Generic tip desteği: `CustomQueue<T>`.

**Görev 2: BFS ve DFS Algoritma İskeletleri**
Graf traversal algoritmalarının temel yapısının oluşturulması:
- BFS: Custom Queue kullanarak katmanlı (level-order) gezinme.
- DFS: Recursive veya iterative (stack tabanlı) gezinme.
- Her iki algoritma, parametre olarak başlangıç düğümü ve opsiyonel filtre fonksiyonu alır.
- Bu aşamada graf yapısı henüz hazır olmadığı için basit bir adjacency list mock'u üzerinde test edilir.

**Kabul Kriterleri:**
- [ ] Custom Queue, standart kütüphane kullanılmadan sıfırdan yazıldı.
- [ ] Queue'ya 1000+ eleman enqueue/dequeue işlemi doğru çalışıyor.
- [ ] BFS ve DFS fonksiyonları basit bir test graf'ında çalışıyor.
- [ ] Traversal sırası doğru (BFS → level-order, DFS → depth-order).
- [ ] Kod `feature/ozcan-algorithms` branch'inde ve PR açıldı.

---

### Kişi C (Frontend Lead) — React + TypeScript Proje Kurulumu

**Görev 1: Proje İskeletinin Kurulması**
`src/SocialGraph.UI/` dizininde React + TypeScript projesi oluşturulur:
- Vite veya Create React App ile proje scaffolding.
- TypeScript strict mode aktif.
- Klasör yapısı: `components/`, `services/`, `types/`, `hooks/`, `pages/`.
- ESLint + Prettier konfigürasyonu.

**Görev 2: Temel Bileşen Mimarisi**
Uygulamanın ana layout'u ve temel bileşenlerin oluşturulması:
- `AppLayout` — Ana sayfa düzeni (header, sidebar, main content area).
- `SearchBar` — Metin tabanlı arama girdi alanı (Trie autocomplete için hazırlık).
- `ResultPanel` — Sorgu sonuçlarının listeleneceği yan panel (placeholder).
- `GraphCanvas` — Graf görselleştirme alanı (placeholder, Sprint 3'te Vis-network ile doldurulacak).
- TypeScript interface tanımları: `INode`, `IEdge`, `IGraphData`, `ISearchResult`.

**Kabul Kriterleri:**
- [ ] React + TypeScript projesi `npm run dev` ile hatasız başlıyor.
- [ ] 4 temel bileşen oluşturuldu ve ekranda render ediliyor.
- [ ] TypeScript interface'leri backend modelleriyle uyumlu şekilde tanımlandı.
- [ ] Kod `feature/c-frontend` branch'inde ve PR açıldı.

---

### Kişi D (Architect & Infrastructure) — Web API Projesi + API Contract

**Görev 1: ASP.NET Core Web API Projesi**
`src/SocialGraph.API/` dizininde backend projesinin kurulması:
- ASP.NET Core Web API projesi oluşturma.
- Program.cs yapılandırması: CORS, JSON serialization, Swagger/OpenAPI.
- Katmanlı mimari planlaması: Controllers → Services → Data (in-memory).
- Singleton yaşam döngüsü planı: Veri yapılarının uygulama ömrü boyunca tek instance olarak DI container'a kaydedilmesi.

**Görev 2: API Contract / Endpoint Sözleşmeleri**
Sprint 2'de yazılacak endpointlerin sözleşmelerinin önceden belirlenmesi:
- Request/Response DTO modelleri: `NodeDto`, `EdgeDto`, `SearchRequestDto`, `TraversalResultDto`.
- Endpoint listesi taslağı: `GET /api/nodes/{id}`, `GET /api/search?query=`, `POST /api/traversal/bfs`, vb.
- Swagger üzerinden dokümantasyon.

**Kabul Kriterleri:**
- [ ] API projesi `dotnet run` ile hatasız başlıyor.
- [ ] Swagger UI erişilebilir durumda ve en az 2 placeholder endpoint görünüyor.
- [ ] DTO modelleri ve endpoint sözleşmeleri dokümente edildi.
- [ ] CORS ayarları frontend'in localhost portunu kabul edecek şekilde yapılandırıldı.
- [ ] Kod `feature/d-infrastructure` branch'inde ve PR açıldı.

---

### Kişi E (Testing & Analysis Specialist) — Custom Trie + Test Altyapısı

**Görev 1: Custom Trie (Önek Ağacı) İmplementasyonu**
Metin tabanlı arama ve otomatik tamamlama için Trie veri yapısının sıfırdan yazılması:
- `TrieNode` sınıfı: karakter, çocuk düğümler (kendi Hash Table'ı veya dizi), kelime-sonu bayrak.
- Temel operasyonlar: `Insert(word)`, `Search(word)`, `StartsWith(prefix)`, `AutoComplete(prefix, maxResults)`.
- Case-insensitive arama desteği.
- Türkçe karakter desteği (ç, ğ, ı, ö, ş, ü) — isimlendirme standardına uygun ID'ler için.

**Görev 2: Test Altyapısının Kurulması**
Proje genelinde birim test framework'ünün yapılandırılması:
- xUnit veya NUnit test projesinin oluşturulması.
- Hash Table, Queue ve Trie için temel test senaryolarının belirlenmesi (edge cases dahil).
- Test çalıştırma komutunun (`dotnet test`) doğrulanması.

**Kabul Kriterleri:**
- [ ] Trie, standart kütüphane kullanılmadan sıfırdan yazıldı.
- [ ] `Insert`, `Search`, `StartsWith`, `AutoComplete` fonksiyonları çalışıyor.
- [ ] 100+ kelime ile autocomplete doğru sonuç döndürüyor.
- [ ] Test projesi oluşturuldu ve en az 10 birim test senaryosu yazıldı.
- [ ] Kod `feature/e-optimization` branch'inde ve PR açıldı.

---

## Sprint 1: Bitti Tanımı (Definition of Done)

| # | Kriter | Doğrulama |
|---|--------|-----------|
| 1 | Custom Hash Table sıfırdan yazıldı, rehashing çalışıyor | 1000+ eleman testi + birim testler |
| 2 | Custom Queue sıfırdan yazıldı, generic tip desteği var | Enqueue/Dequeue döngü testi |
| 3 | Custom Trie sıfırdan yazıldı, autocomplete fonksiyonel | 100+ kelime ile autocomplete testi |
| 4 | Node/Edge modelleri tanımlı ve 3+ düğüm türünü destekliyor | Model instantiation testleri |
| 5 | BFS ve DFS iskeletleri basit graf üzerinde çalışıyor | Mock adjacency list ile traversal testi |
| 6 | React + TS projesi hatasız çalışıyor, 4 temel bileşen mevcut | `npm run dev` + ekran görüntüsü |
| 7 | Web API projesi hatasız çalışıyor, Swagger erişilebilir | `dotnet run` + Swagger UI kontrolü |
| 8 | API endpoint sözleşmeleri (DTO + URL listesi) dokümante edildi | Swagger + markdown doküman |
| 9 | Her kişi kendi feature branch'inde çalışıyor ve PR açtı | GitHub PR listesi kontrolü |
| 10 | Hiçbir veri yapısında standart kütüphane koleksiyonu kullanılmadı | Kod review |
