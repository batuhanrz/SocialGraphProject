# Özcan'ın Tamamladığı Görevler ve Katkılar

> Bu döküman, projenin "Algorithm Master" rolünü üstlenen Özcan tarafından başarıyla tamamlanan teknik görevlerin özetini ve bireysel takım içi katkılarını içerir.

---

## Sprint 1 Katkıları (Sıfırdan Algoritma ve Gezinme)
**Rol Hedefi:** Graf üzerindeki verilerin akıllı ve en performanslı şekilde gezilmesi için gerekli olan klasik algoritmaların (BFS/DFS) tamamen custom veri yapılarıyla (Custom Queue/Stack) ayağa kaldırılması.

### Yapılan İşlemler (Sprint 1.2)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `CustomQueue` sınıfı dairesel dizi (circular array) tabanlı ve thread-safe (`lock`) olarak sıfırdan implemente edildi. Big-O karmaşıklıkları kod yorumu olarak eklendi. | 20.04.2026 |
| 2 | `GraphTraversal` modülü oluşturularak, `CustomQueue` ve `CustomHashTable` kullanılarak BFS ve DFS iskeletleri (Mock graf eşliğinde) başarıyla kodlandı. | 20.04.2026 |
| 3 | `Program.cs` üzerinde 1000+ elemanlık yük testi ve BFS/DFS algoritma doğrulama senaryoları çalıştırılarak tüm kabul kriterleri %100 sağlandı. | 20.04.2026 |

---

## Sprint 2 Katkıları (Property Graph ve Gelişmiş Algoritmalar)

### Yapılan İşlemler (Sprint 2.2)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `GraphTraversal.cs` içerisindeki `BFS` ve `DFS` metotları `PropertyGraph` veri yapısını destekleyecek şekilde güncellendi. | 26.04.2026 |
| 2 | BFS ve DFS algoritmalarına, esnek arama senaryoları için düğüm (`Node`) ve kenar (`Edge`) bazlı filtreleme yeteneği (`Func<T, bool>`) kazandırıldı. | 26.04.2026 |
| 3 | İki düğüm arasındaki en kısa yolu (kenar sayısına göre) bulan `ShortestPath` algoritması BFS kullanılarak sıfırdan yazıldı. Yol takibi için `CustomHashTable` kullanıldı. | 26.04.2026 |
| 4 | Tüm güncellenen metotların regresyon testleri `TestRunner.cs` üzerinden çalıştırılıp doğrulandı. | 26.04.2026 |

---

## Sprint 3 Katkıları (Çok Adımlı Sorgular ve Öneri Sistemi)

### Yapılan İşlemler (Sprint 3.2)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `RelationalQueryEngine.cs` sınıfı oluşturuldu; sonsuz derinlikte zincir sorgu (`ExecuteChainQuery`) mantığı implemente edildi. | 27.04.2026 |
| 2 | Her bir sorgu adımında benzersiz düğüm kümeleri oluşturularak (CustomHashTable ile) ara sonuçların doğru aktarılması sağlandı. | 27.04.2026 |
| 3 | "Triadic Closure" algoritması kullanılarak ortak arkadaş sayısına dayalı arkadaş önerisi sistemi (`GetRecommendations`) geliştirildi. | 27.04.2026 |
| 4 | Önerilerin ortak arkadaş sayısına göre büyükten küçüğe sıralanması için custom "Selection Sort" implementasyonu yapıldı. | 27.04.2026 |
| 5 | `TraversalController` güncellenerek zincir sorgu ve öneri sistemi API uçları (`GET` tabanlı) olarak dışa açıldı. | 27.04.2026 |
| 6 | `RelationalQueryTests.cs` ile tüm ilişkisel sorgu senaryoları (Complex Chain, Mutual Friends) %100 başarıyla test edildi. | 27.04.2026 |

---

## Sprint 4 Katkıları (Finalizasyon ve Dokümantasyon)

### Yapılan İşlemler (Sprint 4.2)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `BigO_Analysis.md` dosyasına teorik zaman/uzay karmaşıklık tablosu ve operasyonel analizler eklendi. | 04.05.2026 |
| 2 | Yük testi (load test) sonuçları ile Big-O teorik sınırları karşılaştırılarak analiz edildi. | 04.05.2026 |
| 3 | Code Defense için `Algorithm_Documentation.md` hazırlandı; BFS, DFS ve ShortestPath pseudocode'ları belgelendi. | 04.05.2026 |
| 4 | Arayüzden gelen algoritma seçimini (BFS/DFS) işleyebilmesi için `TraversalController.cs` güncellendi ve `GraphTraversal.cs`'e recursive `DFS_Path` metodu eklendi. | 04.05.2026 |
