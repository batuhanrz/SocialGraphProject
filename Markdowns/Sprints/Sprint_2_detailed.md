# SPRINT 2: PROPERTY GRAPH ENTEGRASYONU VE API SERVİSLERİ (FAZ 1 & 2)

**Hedef:** Veri yapılarının graf mimarisinde birleştirilmesi, REST API ile dış dünyaya açılması ve sentetik test verisi üretilmesi.

**Kapsam:** Context.md — A.1 (Property Graph), A.2 (Algoritmalar), B.1 (Mikroservis)

---

## Görev Dağılımı

### Sprint 2.1: Batuhan — Adjacency List Tabanlı Property Graph

**Görev 1: Property Graph Çekirdek Yapısı**
- `PropertyGraph` sınıfı: Custom Hash Table ile düğüm deposu, adjacency list ile kenar deposu.
- Heterojen düğüm desteği: User, Photo, Event aynı graf içinde.
- Operasyonlar: `AddNode`, `AddEdge`, `GetNode`, `GetNeighbors`, `GetEdgesByType`, `RemoveNode`, `RemoveEdge`.

**Görev 2: DI Container Kaydı**
- PropertyGraph, Trie ve Hash Table'ın Singleton olarak DI'a kaydı.
- Temel read/write lock altyapısı (detaylı optimizasyon Sprint 3'te).

**Kabul Kriterleri:**
- [ ] PropertyGraph adjacency list tabanlı çalışıyor.
- [ ] 3 düğüm türü + 4 kenar türü destekleniyor.
- [ ] Singleton DI kaydı yapıldı, controller'lardan erişim doğrulandı.
- [ ] PR açıldı (`feature/batuhan-core`).

---

### Sprint 2.2: Özcan — Graf Algoritmaları Entegrasyonu

**Görev 1: BFS/DFS'in PropertyGraph'a Uyarlanması**
- BFS: Custom Queue ile katmanlı gezinme, visited set olarak Custom Hash Table.
- DFS: Recursive derinlik öncelikli gezinme.

**Görev 2: Shortest Path + Filtreli Traversal**
- BFS tabanlı shortest path: iki düğüm arası minimum kenar sayısı + yol izleme.
- Filtreli traversal: kenar türü / düğüm türü filtresi ile gezinme (delegate/func parametre).

**Kabul Kriterleri:**
- [ ] BFS, DFS PropertyGraph üzerinde doğru çalışıyor.
- [ ] Shortest path doğru mesafe ve yol döndürüyor.
- [ ] Filtreli traversal en az 2 farklı filtre ile test edildi.
- [ ] Tüm algoritmalarda custom veri yapıları kullanılıyor.
- [ ] PR açıldı (`feature/ozcan-algorithms`).

---

### Sprint 2.3: Fatma Sude — API Servis Katmanı + Arama Arayüzü

**Görev 1: Frontend API Servis Katmanı**
- `apiService.ts`: Axios/Fetch wrapper, base URL config.
- `nodeService.ts`: Düğüm CRUD çağrıları.
- `traversalService.ts`: BFS, DFS, shortest path çağrıları.
- Hata yönetimi + TypeScript tip güvenliği.

**Görev 2: Arama ve Sonuç Listeleme**
- SearchBar → API autocomplete bağlantısı.
- ResultPanel'de düğüm listesi gösterimi.
- Düğüme tıklayınca detay gösterimi.

**Kabul Kriterleri:**
- [ ] Frontend → Backend API bağlantısı çalışıyor.
- [ ] Autocomplete sonuçları dropdown'da gösteriliyor.
- [ ] Sonuç panelinde düğüm listesi var.
- [ ] PR açıldı (`feature/sude-frontend`).

---

### Sprint 2.4: Muhammed Furkan — REST API Controller'ları + AI Worker İskeleti

**Görev 1: API Controller Endpointleri**

| Endpoint | Method | Açıklama |
|----------|--------|----------|
| `/api/nodes` | GET | Tüm düğümleri listele |
| `/api/nodes/{id}` | GET | Düğüm detayı (Hash Table O(1)) |
| `/api/nodes/search?query=` | GET | Trie ile metin arama |
| `/api/edges/{nodeId}` | GET | Düğümün kenarlarını listele |
| `/api/traversal/bfs` | POST | BFS traversal |
| `/api/traversal/dfs` | POST | DFS traversal |
| `/api/traversal/shortest-path` | POST | En kısa yol |

- Controller → Service → PropertyGraph katmanlı mimari.

**Görev 2: AI Worker İskeleti**
- `SocialGraph.AI` projesinde BackgroundService sınıfı oluşturma.
- API ile haberleşme mekanizması planlaması.
- `appsettings.json` konfigürasyonu.

**Kabul Kriterleri:**
- [ ] 7 endpoint Swagger'dan test edilebiliyor.
- [ ] Doğru HTTP status code'lar dönüyor.
- [ ] AI Worker projesi hatasız başlıyor.
- [ ] PR açıldı (`feature/furkan-infrastructure`).

---

### Sprint 2.5: Isra — Sentetik Veri Üretimi + Birim Testler

**Görev 1: Sentetik Veri Üretim Motoru**
- `DataGenerator` sınıfı: parametrik veri üretimi.
- Farklı topolojiler: yoğun, seyrek, yıldız, zincir.
- Seed data: 50 User, 30 Photo, 20 Event, 200+ edge.

**Görev 2: Veri Yapıları Birim Testleri**

| Yapı | Testler |
|------|---------|
| Hash Table | Ekleme, arama, silme, collision, rehashing, duplicate key |
| Queue | Enqueue, dequeue, peek, boş kuyruk, kapasite artırımı |
| Trie | Insert, search, autocomplete, case-insensitive |
| PropertyGraph | AddNode, AddEdge, GetNeighbors, silme, izole düğüm |

**Kabul Kriterleri:**
- [ ] DataGenerator farklı boyutlarda graf üretebiliyor.
- [ ] Seed data başarıyla yükleniyor.
- [ ] 20+ birim test yazıldı ve hepsi geçiyor.
- [ ] PR açıldı (`feature/isra-optimization`).

---

## Sprint 2: Bitti Tanımı (Definition of Done)

| # | Kriter | Doğrulama |
|---|--------|-----------|
| 1 | PropertyGraph adjacency list tabanlı, 3 tür düğüm + 4 tür kenar | CRUD test |
| 2 | BFS, DFS, shortest path, filtreli traversal fonksiyonel | 3+ sorgu senaryosu |
| 3 | 7 REST API endpoint'i çalışır durumda | Swagger test |
| 4 | Frontend API'ye bağlanıyor, arama çalışıyor | Tarayıcı demo |
| 5 | Sentetik veri motoru parametrik çalışıyor | 3 farklı topoloji |
| 6 | 20+ birim test yazıldı ve geçiyor | `dotnet test` |
| 7 | AI Worker iskelet yapısı oluşturuldu | Proje hatasız başlıyor |
| 8 | Her kişi kendi branch'inde, PR açtı | GitHub kontrolü |
