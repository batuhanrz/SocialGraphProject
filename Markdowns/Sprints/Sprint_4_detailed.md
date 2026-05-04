# SPRINT 4: DAĞITIM, TEKNİK ANALİZ VE FİNALİZASYON (FAZ 3 & TESLİMAT)

**Hedef:** Sistemin Docker ile paketlenmesi, teknik raporların tamamlanması, cross-review ile Code Defense hazırlığı ve demo videosunun çekilmesi.

**Kapsam:** Context.md — B.2 (Teslim Edilecekler), B.3 (Code Defense), tüm fazların finalizasyonu.

---

## Görev Dağılımı

### Sprint 4.1: Batuhan — UML Diyagramları + Kod Kalitesi

**Görev 1: Sistem UML Diyagramları**
- Sınıf diyagramı (Class Diagram): Node, Edge, PropertyGraph, HashTable, Trie, Queue sınıfları ve aralarındaki ilişkiler.
- Bileşen diyagramı (Component Diagram): API, AI, UI servislerinin bağımlılıkları.
- Sıra diyagramı (Sequence Diagram): Çok adımlı sorgu akışı (User → API → PropertyGraph → Response → UI).
- Diyagramlar rapor dosyasına eklenir.

**Görev 2: Kod Temizliği ve Refactoring**
- Tüm sınıflarda XML/JSDoc açıklamaları eklenmesi.
- Kullanılmayan kod ve import'ların temizlenmesi.
- Naming convention kontrolü (Türkçe karakter yok — B.3).

**Kabul Kriterleri:**
- [ ] En az 3 UML diyagramı (class, component, sequence) çizildi ve rapora eklendi.
- [ ] Tüm public metotlarda açıklama/dokümantasyon mevcut.
- [ ] B.3 isimlendirme şartı tüm kodda sağlanıyor.
- [ ] PR açıldı (`feature/batuhan-core`).

---

### Sprint 4.2: Özcan — Big-O Analiz Tablosu + Algoritma Dokümantasyonu

**Görev 1: Zaman ve Uzay Karmaşıklığı Analiz Tablosu**

| Yapı / Algoritma | Operasyon | Zaman (Ortalama) | Zaman (En Kötü) | Uzay |
|-------------------|-----------|-------------------|-------------------|------|
| Hash Table | Put / Get / Remove | O(1) | O(n) | O(n) |
| Hash Table | Rehash | O(n) | O(n) | O(n) |
| Trie | Insert / Search | O(m) | O(m) | O(ALPHABET × m × n) |
| Trie | AutoComplete | O(m + k) | O(m + k) | O(k) |
| Queue | Enqueue / Dequeue | O(1) | O(1) amortized | O(n) |
| BFS | Full traversal | O(V + E) | O(V + E) | O(V) |
| DFS | Full traversal | O(V + E) | O(V + E) | O(V) |
| Shortest Path | BFS-based | O(V + E) | O(V + E) | O(V) |
| Multi-step Query | Chain traversal | O(k × (V + E)) | O(k × (V + E)) | O(V) |

- Her satır için açıklama paragrafı.
- Sprint 3'teki yük testi sonuçları ile karşılaştırma.

**Görev 2: Algoritma Dökümantasyonu**
- Her algoritmanın çalışma mantığı (pseudocode + açıklama).
- Hangi veri yapısını neden kullandığının gerekçesi.
- Code Defense'e hazırlık: herkesin okuyabileceği açıklıkta yazılması.

**Kabul Kriterleri:**
- [x] Big-O analiz tablosu tüm yapıları ve algoritmaları kapsıyor.
- [x] Her analiz için açıklama paragrafı mevcut.
- [x] Gerçek ölçüm sonuçları ile teorik analiz karşılaştırıldı.
- [x] Algoritma dökümantasyonu (pseudocode + açıklama) tamamlandı.
- [x] PR açıldı (`feature/ozcan-algorithms`).

---

### Sprint 4.3: Fatma Sude — UI Son Rötuşlar + Demo Videosu Hazırlığı

**Görev 1: Arayüz İyileştirmeleri**
- Responsive layout: farklı ekran boyutlarında düzgün görünüm.
- Loading/spinner göstergeleri API çağrıları sırasında.
- Hata durumlarında kullanıcı dostu mesajlar.
- Graf görselleştirmede büyük veri setlerinde performans optimizasyonu (filtreleme/sınırlama).

**Görev 2: Demo Videosu Akışı ve Hazırlık**
- Demo video senaryosunun planlanması (max 10 dk):
  1. Sistem başlatma (docker-compose up).
  2. Arayüz tanıtımı (arama, graf görselleştirme, yan panel).
  3. Sorgu demosu (BFS, shortest path, çok adımlı sorgu).
  4. AI Worker'ın dinamik veri üretiminin gösterimi.
  5. Kod yapısının kısa tanıtımı (veri yapıları).
- UI ekran kaydı ve demo rehearsal.

**Kabul Kriterleri:**
- [x] UI responsive çalışıyor (en az 2 farklı ekran boyutunda test).
- [x] Loading göstergeleri ve hata mesajları mevcut.
- [x] Demo senaryosu yazıldı ve en az 1 kez prova yapıldı.
- [x] Graf render optimizasyonu (physics stabilization) yapıldı.
- [x] PR açıldı (`feature/sude-frontend`).

---

### Sprint 4.3-B: Fatma Sude — Graf Etkileşim Sistemi Yeniden Tasarımı

**Görev 1: Sürekli Süzülme (Floating Motion)**
- Fizik motoru `stabilization: false` ile başlatılarak düğümlerin sürekli doğal bir hareket halinde kalması sağlandı.
- Floating Keeper mekanizması ile simülasyonun asla durmaması garanti altına alındı.

**Görev 2: Akıllı Pinleme Sistemi (Shift Toggle)**
- Seçili düğümde Shift tuşuna basıldığında pin/unpin toggle yapılması sağlandı.
- Drag tabanlı pinleme kaldırılarak daha doğal bir UX oluşturuldu.
- Pinlenmiş düğümler Mor (#a855f7) çerçeveyle ayırt ediliyor.

**Görev 3: Çoklu Durum Renk Paleti**
- Origin (Mavi), Target (Kırmızı), Pinned (Mor), kombinasyonlar (İndigo, Fuşya) ve Path (Yeşil) için 7 farklı çerçeve rengi tanımlandı.
- Sağ tık ile Target Node otomatik seçimi ve kırmızı işaretleme eklendi.

**Görev 4: Sorgu Akışı Optimizasyonu**
- BFS/DFS butonları algoritmayı seçer, sorgu sadece "Shortest Path" butonuyla gider.
- Yol üzerindeki kenarlar yeşil glow efekti ile vurgulanıyor.
- Panel üzerinde ham ID'ler yerine gerçek isimler (Name Resolution) gösteriliyor.

**Kabul Kriterleri:**
- [x] Düğümler sürekli süzülüyor, asla tamamen durmuyor.
- [x] Shift ile pin/unpin toggle çalışıyor, görsel geri bildirim anında oluşuyor.
- [x] Origin, Target ve Pinned durumları farklı renklerle ayrışıyor.
- [x] BFS/DFS sadece algoritma seçiyor, Shortest Path butonu sorguyu tetikliyor.
- [x] Yol kenarlarında yeşil glow efekti görünüyor.
- [x] Panel'de düğüm isimleri (Name/Title) doğru çözümleniyor.

---

### Sprint 4.4: Muhammed Furkan — Docker Konfigürasyonları

**Görev 1: Dockerfile'lar**
Her servis için ayrı Dockerfile:
- `src/SocialGraph.API/Dockerfile`: .NET multi-stage build.
- `src/SocialGraph.AI/Dockerfile`: .NET multi-stage build.
- `src/SocialGraph.UI/Dockerfile`: Node.js build + nginx serve.

**Görev 2: docker-compose.yml**
Tüm sistemin tek komutla ayağa kalkması:
```yaml
services:
  api:
    build: ./src/SocialGraph.API
    ports: ["5000:5000"]
  ai:
    build: ./src/SocialGraph.AI
    depends_on: [api]
  ui:
    build: ./src/SocialGraph.UI
    ports: ["3000:80"]
    depends_on: [api]
```
- Servisler arası ağ konfigürasyonu.
- Environment variables ile konfigürasyon.
- Health check tanımları.

**Görev 3: README Güncelleme**
- Çalıştırma talimatları: `docker-compose up --build`.
- Gereksinimler: Docker, Docker Compose versiyon bilgileri.
- Port bilgileri ve erişim URL'leri.

**Kabul Kriterleri:**
- [x] `docker-compose up --build` ile tüm sistem hatasız ayağa kalkıyor.
- [x] API, AI ve UI servisleri container içinde çalışıyor.
- [x] Servisler arası iletişim container ağı üzerinden sağlanıyor (Health checks dahil).
- [x] README çalıştırma talimatlarını içeriyor.
- [x] Graf veri yoğunluğu optimizasyonu yapıldı (Frontend performansı için).
- [x] PR açıldı (`feature/furkan-infrastructure`).

---

### Sprint 4.5: Isra — Performans Raporu + AI Prompt Dökümü + Final Test

**Görev 1: AI Prompt Dökümü**
- AI simülasyonunda kullanılan prompt'ların (varsa) tam dökümü.
- GenAI ile üretilen sentetik veri örneklerinin belgelenmesi.
- AI kullanım gerekçesi ve sonuç değerlendirmesi.

**Görev 2: Nihai Performans Raporu**
- Farklı graf boyutlarında (100, 500, 1000, 5000 düğüm) benchmark sonuçları.
- Bellek kullanımı ölçümleri.
- Sonuçların grafik/tablo olarak sunumu.
- Darboğaz (bottleneck) analizi ve iyileştirme önerileri.

**Görev 3: B.3 İsimlendirme Kontrolü + Final Doğrulama**
- Tüm kodda Türkçe karakter taraması.
- Tüm ekip üyelerinin ad-soyadlarının uygun formatta yerleştirildiğinin kontrolü.
- Docker ile temiz bir ortamda son uçtan uca test.

**Kabul Kriterleri:**
- [ ] AI prompt dökümü tamamlandı ve rapora eklendi.
- [ ] 4 farklı graf boyutunda benchmark tablosu hazırlandı.
- [ ] B.3 isimlendirme kontrolü tüm kodda yapıldı.
- [ ] Temiz ortamda `docker-compose up` ile sistem çalıştı.
- [ ] PR açıldı (`feature/isra-optimization`).

---

### Sprint 4.6: Tüm Ekip — Cross-Review (Code Defense Hazırlığı) + Demo Videosu

**Cross-Review:**
Her ekip üyesi, en az bir başka üyenin modülünü inceler ve açıklayabilir hale gelir:

| İnceleyen | İncelediği Modül | Odak |
|-----------|------------------|------|
| Batuhan | Özcan'ın algoritmaları | BFS/DFS/Sorgu mantığı |
| Özcan | Batuhan'ın veri yapıları | Hash Table / PropertyGraph |
| Fatma Sude | Muhammed Furkan'nin API'si | Controller → Service akışı |
| Muhammed Furkan | Isra'nin testleri + Trie | Trie yapısı + test stratejisi |
| Isra | Fatma Sude'nin frontend'i | Görselleştirme + API bağlantısı |

**Demo Videosu (max 10 dk):**
- Ekip adına bir kişi tarafından (veya faceless/screencast şeklinde) çekilebilir.
- İçerik: Arayüz demosu + dinamik veri üretimi + core veri yapılarının kod üzerinden hızlıca gösterimi.

**Kabul Kriterleri:**
- [ ] Her üye en az 1 başka modülü inceledi ve açıklayabiliyor.
- [ ] Demo videosu çekildi (max 10 dk).
- [ ] Video linki teslim dosyasına eklendi.

---

## Sprint 4: Bitti Tanımı (Definition of Done)

| # | Kriter | Doğrulama |
|---|--------|-----------|
| 1 | UML diyagramları (class, component, sequence) rapora eklendi | Rapor dosyası incelemesi |
| 2 | Big-O analiz tablosu tüm yapıları kapsıyor | Rapor dosyası incelemesi |
| 3 | Docker ile tüm sistem tek komutla ayağa kalkıyor | `docker-compose up --build` testi |
| 4 | AI prompt dökümü ve performans raporu tamamlandı | Rapor dosyası incelemesi |
| 5 | B.3 isimlendirme kontrolü yapıldı | Kod taraması |
| 6 | Cross-review yapıldı, her üye başka bir modülü açıklayabiliyor | Sözlü doğrulama |
| 7 | Demo videosu çekildi (max 10 dk) | Video dosyası/linki |
| 8 | README çalıştırma talimatlarını içeriyor | README incelemesi |
| 9 | Tüm PR'lar merge edildi, `main` branch stabil | CI/build kontrolü |
