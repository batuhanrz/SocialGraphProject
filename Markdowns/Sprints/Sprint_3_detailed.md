# SPRINT 3: ÇOK ADIMLI SORGULAR, GÖRSELLEŞTİRME VE AI WORKER (FAZ 2 & 3)

**Hedef:** Asenkron veri akışının sağlanması, interaktif 2D ağ haritasının oluşturulması ve çok adımlı sorgu motorunun tamamlanması.

**Kapsam:** Context.md — A.2 (Algoritmalar, Sorgu Modeli), A.3 (Arayüz), B.1 (Eşzamanlılık)

---

## Görev Dağılımı

### Batuhan — Thread-Safety ve Eşzamanlılık Yönetimi

**Görev 1: Read/Write Lock Mekanizmaları**
- Tüm graf okuma/yazma operasyonları için `ReaderWriterLockSlim` veya custom lock implementasyonu.
- Birden fazla eşzamanlı okuma + tek yazma garantisi.
- AI Worker'ın graf'a veri yazarken API okumalarını bloklamadan çalışması.

**Görev 2: Eşzamanlılık Test Senaryoları**
- Çoklu thread'den aynı anda okuma/yazma testi.
- Deadlock ve race condition kontrolü.
- Performans: lock altında ortalama okuma süresi ölçümü.

**Kabul Kriterleri:**
- [ ] Graf operasyonları thread-safe çalışıyor.
- [ ] Eşzamanlı 10+ okuma + 1 yazma senaryosu deadlock olmadan tamamlanıyor.
- [ ] Lock mekanizması doğru çalıştığını gösteren test senaryoları yazıldı.
- [ ] PR açıldı (`feature/batuhan-core`).

---

### Özcan — Çok Adımlı İlişkisel Sorgu Motoru

**Görev 1: Zincir Sorgu Motoru**
Context.md'deki örnek sorgu akışının implementasyonu:
- `User → FRIEND → Users → ATTENDS → Events → UPLOADED → Photos` zincir traversal'ı.
- Her adımda ara sonuç kümesi oluşturulması ve bir sonraki adıma iletilmesi.
- Sorgu motoru generic olmalı: farklı zincir tipleri parametre olarak verilebilmeli.

**Görev 2: Opsiyonel — Triadic Closure (Arkadaş Önerisi)**
- Ortak arkadaş sayısına dayalı arkadaş önerisi algoritması.
- Kullanıcı A ve B'nin ortak arkadaş sayısı → öneri skoru.

**Kabul Kriterleri:**
- [ ] User→Friends→Events→Photos zincir sorgusu tam fonksiyonel.
- [ ] En az 3 farklı zincir tipi ile test edildi.
- [ ] Sonuç kümesi doğru ve tutarlı.
- [ ] (Opsiyonel) Arkadaş önerisi algoritması çalışıyor.
- [ ] PR açıldı (`feature/ozcan-algorithms`).

---

### Kişi C — Vis-network 2D Görselleştirme + Etkileşim

**Görev 1: 2D Node-Link Diyagramı**
- Vis-network (veya D3.js) kütüphanesi ile interaktif graf görselleştirme.
- Düğüm tiplerine göre görsel ayrım:
  - User → daire (mavi)
  - Photo → kare (yeşil)
  - Event → üçgen (turuncu)
- Kenar tiplerine göre stil: FRIEND (düz çizgi), LIKES (kesikli), ATTENDS (noktalı).
- Zoom, pan ve sürükleme desteği.

**Görev 2: Yan Panel Etkileşimi**
- Graf üzerinde bir düğüme tıklandığında yan panelde detay gösterimi.
- Düğüm özellikleri (properties) Hash Table üzerinden O(1) erişimle getirilir.
- Tıklanan düğümün komşularının listesi.
- Traversal sonuçlarının graf üzerinde renkli vurgulanması (highlight).

**Görev 3: Sorgu Arayüzü**
- Kullanıcının BFS/DFS/shortest-path/zincir sorgusu seçebileceği sorgu paneli.
- Sorgu parametreleri form alanı (başlangıç düğümü, hedef düğümü, filtre türü).
- Sonuç: hem liste hem graf üzerinde görsel olarak gösterim.

**Kabul Kriterleri:**
- [ ] Graf 50+ düğüm ile sorunsuz render ediliyor.
- [ ] 3 düğüm türü farklı şekil/renk ile gösterilliyor.
- [ ] Düğüme tıklayınca yan panelde özellikler görünüyor.
- [ ] Sorgu sonuçları graf üzerinde vurgulanıyor.
- [ ] PR açıldı (`feature/c-frontend`).

---

### Kişi D — AI Simulation Worker (BackgroundService)

**Görev 1: AI Simulation Motoru**
- `BackgroundService` / `IHostedService` olarak çalışan worker servis.
- Her 15 saniyede bir sentetik veri üretimi ve API'ye gönderimi.
- Üretilen veri: rastgele kullanıcılar, ilişkiler, fotoğraflar, etkinlikler.
- GenAI kullanımı opsiyonel: basit randomized generation yeterli.

**Görev 2: Servisler Arası İletişim**
- AI Worker → API servisi HTTP client ile haberleşme.
- Hata yönetimi: API'ye ulaşılamazsa retry mekanizması.
- Loglama: üretilen veri sayıları ve hata durumları.

**Görev 3: Konfigürasyon**
- `appsettings.json`: simülasyon aralığı (ms), her döngüde üretilecek veri miktarı.
- Başlatma/durdurma kontrolü (graceful shutdown).

**Kabul Kriterleri:**
- [ ] Worker 15 saniyede bir veri üretip API'ye gönderiyor.
- [ ] Üretilen veriler PropertyGraph'a başarıyla ekleniyor.
- [ ] API kapalıyken worker crash etmiyor (retry/log çalışıyor).
- [ ] Frontend'de graf verisi dinamik olarak güncelleniyor.
- [ ] PR açıldı (`feature/d-infrastructure`).

---

### Kişi E — Entegrasyon Testleri + Big-O Analiz Taslağı

**Görev 1: Servisler Arası Entegrasyon Testleri**
- API ↔ PropertyGraph entegrasyon testleri.
- AI Worker → API → PropertyGraph veri akış testi.
- Frontend → API → PropertyGraph uçtan uca test senaryoları.

**Görev 2: Yük Testleri**
- AI Worker'ın sürekli veri üretmesi durumunda sistem performansı ölçümü.
- 500, 1000, 5000 düğüm ile arama ve traversal süreleri.
- Sonuçların tablo halinde raporlanması.

**Görev 3: İlk Big-O Analiz Taslağı**
- Her veri yapısı ve algoritma için teorik Big-O analizi:
  - Hash Table: Put O(1) avg, Get O(1) avg, Rehash O(n)
  - Trie: Insert O(m), Search O(m), Autocomplete O(m + k)
  - Queue: Enqueue O(1), Dequeue O(1)
  - BFS O(V+E), DFS O(V+E), Shortest Path O(V+E)
- Ölçülen gerçek süreler ile teorik analiz karşılaştırması.

**Kabul Kriterleri:**
- [ ] 3+ entegrasyon test senaryosu yazıldı ve geçiyor.
- [ ] 3 farklı graf boyutuyla yük testi yapıldı, sonuçlar tablo halinde.
- [ ] Big-O analiz taslağı markdown olarak yazıldı.
- [ ] PR açıldı (`feature/e-optimization`).

---

## Sprint 3: Bitti Tanımı (Definition of Done)

| # | Kriter | Doğrulama |
|---|--------|-----------|
| 1 | Graf operasyonları thread-safe | Eşzamanlılık testi geçiyor |
| 2 | Çok adımlı zincir sorguları fonksiyonel | 3+ zincir tipi testi |
| 3 | 2D graf görselleştirme interaktif çalışıyor | 50+ düğüm ile demo |
| 4 | Düğüm tıklama → yan panel detay gösterimi | Tarayıcı testi |
| 5 | AI Worker 15 sn'de bir veri üretip API'ye gönderiyor | Log kontrolü |
| 6 | Frontend'de dinamik veri güncellemesi görünüyor | Tarayıcı demo |
| 7 | Entegrasyon testleri geçiyor | Test çıktısı |
| 8 | Big-O analiz taslağı oluşturuldu | Markdown doküman |
| 9 | Her kişi kendi branch'inde, PR açtı | GitHub kontrolü |
