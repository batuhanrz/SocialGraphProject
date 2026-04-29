# Detaylandırılmış Teknik Rapor

**Tarih:** 30.04.2026  
**Versiyon:** 1.1 (Ultra-Detailed)  
**Ekip Üyeleri:** Batuhan, Özcan, Fatma Sude, Muhammed Furkan, Isra

---

## 1. Mimari Tasarım ve Sistem Mühendisliği

SocialGraph projesi, verimlilik, thread-safety ve modülerlik prensipleri üzerine inşa edilmiş çok katmanlı bir mikroservis ekosistemidir.

### 1.1. Backend API (The Core Engine)
Sistemin kalbi olan API, tüm veri yapılarını ve algoritmaları bellekte (In-Memory) yönetir.
- **Singleton Pattern:** Veri tutarlılığı için `PropertyGraph`, `CustomHashTable` ve `CustomTrie` nesneleri DI (Dependency Injection) konteynerinde Singleton olarak tanımlanmıştır.
- **Thread-Safety (ReaderWriterLockSlim):** Sistemde "Yüksek Okuma / Düşük Yazma" senaryosu hakimdir. Yazma işlemleri (AI Worker) ile okuma işlemleri (UI Sorguları) arasındaki çakışmalar `ReaderWriterLockSlim` ile yönetilir. Yazma kilidi (`EnterWriteLock`) sadece verinin fiziksel olarak tabloya eklendiği milisaniyelik anda devreye girerken, okuma kilitleri (`EnterReadLock`) eşzamanlı sorgulara izin vererek sistemin throughput değerini maksimize eder.

### 1.2. AI Simulation Worker (Resilient Background Service)
`SocialGraph.AI` projesi, sistemin dinamik doğasını simüle eder.
- **Worker Logic:** `BackgroundService` sınıfından türetilen bu yapı, her 15 saniyede bir `DataGenerator` sınıfını tetikler.
- **Resiliency:** API ile olan iletişimde `Polly` benzeri bir mantıkla hata yönetimi yapılır. API geçici olarak ulaşılamaz olduğunda servis crash etmez, log atarak bir sonraki döngüde tekrar dener.
- **Batch Processing:** Veriler tek tek değil, her döngüde üretilen tüm düğüm ve kenarlar bir liste halinde toplu olarak API'ye gönderilir.

### 1.3. Frontend UI (Interactive Visualization)
- **State Management:** React hook'ları ile arama sonuçları ve graf durumu senkronize edilir.
- **Service Layer:** API ile olan tüm etkileşimler `Native Fetch` ve `TypeScript Interfaces` üzerinden tip güvenli şekilde yürütülür.

---

## 2. Veri Yapıları: Derinlemesine Analiz (From Scratch)

Projenin en kritik kuralı olan "Standart Koleksiyon Kütüphanesi Kullanmama" şartı, tüm çekirdek yapılarda %100 uygulanmıştır.

### 2.1. CustomHashTable<K, V> (Dinamik Karma Tablo)
- **Hash Fonksiyonu:** Anahtarların `GetHashCode()` değeri üzerinden modüler aritmetik kullanılarak slot belirlenir.
- **Çakışma Çözümü (Linear Probing):** Bir slot doluysa, sıradaki boş slot bulunana kadar tablo üzerinde doğrusal olarak ilerlenir.
- **Dinamik Büyüme (Rehashing):** Tablodaki doluluk oranı (Load Factor) %75'i geçtiğinde, tüm tablo kapasitesi iki katına (veya bir sonraki asal sayıya yakın değere) çıkarılır ve tüm elemanlar yeni tabloya yeniden hash'lenerek yerleştirilir.

### 2.2. CustomTrie (Gelişmiş Önek Ağacı)
- **Hibrit Yapı:** Trie'nin her düğümü (`TrieNode`), alt dallarını saklamak için kendi içinde bir `CustomHashTable` barındırır. Bu, standart dizi veya liste kullanımına göre çok daha hızlı dallanma sağlar.
- **Search Complexity:** O(m) karmaşıklığı ile kelime uzunluğuna bağlı sabit sürede arama yapılır. Düğüm sayısı artsa bile arama süresi etkilenmez.

### 2.3. PropertyGraph (Adjacency List Modeli)
- **Depolama:** Düğümler bir `CustomHashTable` içinde saklanır (O(1) erişim). Komşuluk ilişkileri her düğümün kendi içinde tuttuğu bir `CustomHashTable` (veya CustomList) ile yönetilir.
- **Heterojen Yapı:** Düğümler `Type` enum'u ve `Properties` sözlüğü (yine custom hash table) ile farklı veri türlerini destekler.

---

## 3. Gelişmiş Algoritmalar ve Sorgu Motoru

### 3.1. Çok Adımlı İlişkisel Sorgu Motoru (Relational Engine)
Sorgu motoru, "Pipeline" mantığıyla çalışır:
1. **Girdi:** Başlangıç düğümü ve bir dizi "EdgeType" filtresi.
2. **İşlem:** Her adımda bir önceki adımın sonuçları "Source" olarak alınır ve ilgili kenar türüyle komşularına gidilir.
3. **Tekilleştirme:** Sonuç setinde aynı düğümün birden fazla kez yer almaması için ara sonuçlar her adımda bir `CustomHashTable` (HashSet mantığıyla) üzerinden geçirilerek tekilleştirilir.

### 3.2. Arkadaş Önerisi (Recommendation Scoring)
Algoritma, "Triadic Closure" prensibini bir skorlama mekanizmasına dönüştürür:
- **Formül:** `Score(A, B) = Count(Neighbors(A) ∩ Neighbors(B))`.
- **Skorlama:** Ortak komşu sayısı ne kadar fazlaysa, öneri o kadar üst sırada yer alır. O(V * degree^2) karmaşıklığında optimize edilmiş bir tarama yapılır.

---

## 4. Kullanıcı Arayüzü Bileşen Mimarisi

- **GraphCanvas:** Vis-network motorunu barındırır. `physics.barnesHut` optimizasyonu ile 500+ düğümün 60 FPS hızında render edilmesi sağlanır.
- **QueryPanel:** BFS, DFS ve Zincir Sorgu parametrelerinin girildiği komuta merkezidir.
- **ResultPanel (SidePanel):** Grafa tıklandığında devreye giren bu panel, seçilen düğümün verilerini API'den O(1) hızında çekerek JSON veya liste formatında sunar.

---

## 5. Performans ve Karmaşıklık Analizi (Big-O)

| Yapı / Algoritma | Operasyon | Zaman (Ortalama) | Zaman (En Kötü) | Uzay |
|-------------------|-----------|-------------------|-------------------|------|
| Hash Table | Put / Get | O(1) | O(n) | O(n) |
| Trie | Insert / Search| O(m) | O(m) | O(ALPHABET * m) |
| BFS / DFS | Traversal | O(V + E) | O(V + E) | O(V) |
| Shortest Path | Finding Path | O(V + E) | O(V + E) | O(V) |
| Recommendation | Suggestion | O(V * deg^2) | O(V^2) | O(V) |

---

## 6. Takım İletişimi ve Teknik Tartışmalar (Discussions)

Geliştirme sürecinde GitHub Discussions üzerinden aşağıdaki kararlar alınmıştır:
1. **gRPC vs HTTP:** Projenin eğitim amaçlı olması ve görselleştirme kolaylığı nedeniyle RESTful HTTP mimarisinde karar kılınmıştır.
2. **Singleton vs Static:** Veri yapılarının birer "Service" olarak yönetilmesi ve test edilebilirliği için Singleton DI tercih edilmiştir.
3. **Naming Convention:** B.3 maddesi gereği tüm kod tabanı Türkçe karakterden arındırılmış ve PascalCase standartlarına getirilmiştir.

---

## 7. Sonuç ve Gelecek Vizyonu
Sistem, Faz 1-3 gereksinimlerini (sıfırdan veri yapıları, mikroservis mimarisi, ilişkisel sorgu motoru ve interaktif görselleştirme) belirtilen kriterlere uygun olarak karşılamaktadır. 30.04.2026 itibariyle projenin çekirdek mimarisi tamamlanmış ve test edilmiştir. Final aşamasında sistemin dokümantasyonu UML diyagramları ve Code Defense hazırlıklarıyla tamamlanacaktır.

---
**GitHub Repository:** [https://github.com/batuhanrz/SocialGraphProject](https://github.com/batuhanrz/SocialGraphProject)
