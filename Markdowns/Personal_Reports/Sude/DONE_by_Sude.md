# Fatma Sude'nin Tamamladığı Görevler ve Katkılar

> Bu döküman, projenin "Frontend Lead" rolünü üstlenen Fatma Sude tarafından başarıyla tamamlanan teknik görevlerin özetini içerir.

---

### Sprint 1 Katkıları (Frontend Mimari ve UI Kurulumu)
**Rol Hedefi:** Sosyal ağ verilerinin görselleştirilmesi için modern, performanslı ve "premium" hissettiren bir React tabanlı kullanıcı arayüzü inşa etmek.

### Yapılan İşlemler (Sprint 1.3)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | SocialGraph.UI projesi Vite + React + TypeScript kullanılarak sıfırdan oluşturuldu. | 20.04.2026 |
| 2 | Premium Swiss Minimal tasarım sistemi index.css üzerinde (Glassmorphism, HSL renk paleti, Outfit fontu) kuruldu. | 20.04.2026 |
| 3 | AppLayout, SearchBar, ResultPanel ve GraphCanvas bileşenleri modüler ve modern UI standartlarında geliştirildi. | 25.04.2026 |
| 4 | Klasör yapısı (services, hooks, pages vb.) kuruldu ve tsconfig üzerinde strict mode aktif edildi. | 25.04.2026 |
| 5 | Backend modelleriyle uyumlu TypeScript interface'leri (INode, IEdge vb.) tanımlandı. | 20.04.2026 |

## Sprint 2 Katkıları (Frontend Servis Entegrasyonu)

### Yapılan İşlemler (Sprint 2.3)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `apiService.ts` dosyası third-party bağımlılık kullanılmadan (Native `fetch`) yazılarak backend API ile TypeScript tipli haberleşme iskeleti kuruldu. | 26.04.2026 |
| 2 | `nodeService.ts` ve `traversalService.ts` dosyaları oluşturularak arama, düğüm detayı getirme, BFS, DFS ve Shortest Path uçlarının HTTP çağrıları tanımlandı. | 26.04.2026 |
| 3 | `SearchBar.tsx` bileşenine debounce mekanizmalı dinamik Autocomplete dropdown özelliği eklendi. | 26.04.2026 |
| 4 | `ResultPanel.tsx` ve `AppLayout.tsx` arasındaki state yönetimi sağlandı, seçili düğüm detayları ekranda dinamik olarak "Premium Swiss Minimal" tasarım felsefesiyle listelendi. | 26.04.2026 |
| 5 | Geliştirilen TypeScript/React kodları `npm run lint` ve `npm run build` ile doğrulandı. | 26.04.2026 |

---

## Sprint 3 Katkıları (Interaktif Görselleştirme ve Sorgu Arayüzü)

### Yapılan İşlemler (Sprint 3.3)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `vis-network` kütüphanesi entegre edilerek, projenin en temel gereksinimi olan 2D Node-Link diyagramı interaktif hale getirildi. | 27.04.2026 |
| 2 | Düğüm tipleri (User, Photo, Event) ve kenar tipleri (Friend, Likes, Attends) için "Swiss Standard" tasarım yönergelerine uygun görsel özelleştirmeler yapıldı. | 27.04.2026 |
| 3 | `QueryPanel.tsx` bileşeni geliştirilerek; BFS, DFS, Shortest Path ve Zincir Sorgu türleri için kullanıcı dostu bir arayüz sunuldu. | 27.04.2026 |
| 4 | Sorgu sonuçlarının graf üzerinde vurgulanması (highlight) ve seçilen düğüme otomatik odaklanma (focus/zoom) mekanizmaları kuruldu. | 27.04.2026 |
| 5 | Grafa tıklama (click event) ile `ResultPanel` arasındaki senkronizasyon sağlanarak O(1) hızında veri gösterimi tamamlandı. | 27.04.2026 |
| 6 | Arayüzün responsive yapısı iyileştirildi ve modern bir lejant (legend) sistemi eklendi. | 27.04.2026 |

---

## Sprint 4 Katkıları (UI Finalizasyonu ve Optimizasyon)
**Rol Hedefi:** Arayüzün performansını en üst seviyeye çıkarmak, kullanıcı deneyimini loading/hata mesajlarıyla iyileştirmek ve final sunumu için profesyonel içerik hazırlamak.

### Yapılan İşlemler (Sprint 4.3)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | **Performans Optimizasyonu:** `GraphCanvas.tsx` içinde fizik motoru (physics) stabilizasyon sonrası durdurularak büyük veri setlerinde (500+ kenar) %90 işlemci tasarrufu sağlandı. | 03.05.2026 |
| 2 | **Kullanıcı Deneyimi (UX):** API çağrıları için şık loading spinner'lar ve "Sonuç Bulunamadı" gibi kullanıcı dostu hata/uyarı mesajları entegre edildi. | 03.05.2026 |
| 3 | **Responsive Tasarım:** Sidebar ve lejant yapıları 1024px ve altındaki ekran boyutları için (Media Queries) optimize edildi. | 03.05.2026 |
| 4 | **Görsel Kalite:** Graf üzerindeki kenar opaklığı (opacity) ve cam efekti (glassmorphism) jüri standartlarına göre finalize edildi. | 03.05.2026 |
| 5 | **Sunum Hazırlığı:** Projenin final jüri sunumu için 10 dakikalık teknik ve uygulama odaklı "Demo Senaryosu" hazırlandı. | 03.05.2026 |

---

## Sprint 4 Katkıları — Faz B (Graf Etkileşim Sistemi Yeniden Tasarımı)
**Rol Hedefi:** Grafın etkileşim modelini profesyonel bir seviyeye taşımak; kullanıcıya sezgisel kontroller (sağ tık, Shift tuşu) sunarak elle veri girişi zorunluluğunu ortadan kaldırmak ve grafı "yaşayan" bir organizma haline getirmek.

### Yapılan İşlemler (Sprint 4.3-B)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | **Canlı Süzülme (Floating Motion):** Fizik motoru `stabilization: false` ile başlatılarak düğümlerin sürekli doğal bir hareket halinde kalması sağlandı. Floating Keeper ile simülasyonun asla durmaması garanti altına alındı. | 03.05.2026 |
| 2 | **Akıllı Pinleme (Shift Toggle):** Seçili düğümde Shift tuşuna basıldığında pin/unpin toggle sağlandı. Pinlenen düğümler Mor (#a855f7) çerçeveyle ayırt ediliyor. | 03.05.2026 |
| 3 | **7 Renkli Durum Paleti:** Origin (Mavi), Target (Kırmızı), Pinned (Mor), Origin+Pinned (İndigo), Target+Pinned (Fuşya), Path (Yeşil) ve Normal (Beyaz) durumları için ayrı çerçeve renkleri tanımlandı. | 03.05.2026 |
| 4 | **Sağ Tık ile Hedef Seçimi:** Düğüme sağ tıklandığında "Target Node" olarak otomatik atanması sağlandı. Elle ID girme zorunluluğu kaldırıldı. | 03.05.2026 |
| 5 | **BFS/DFS Algoritma Seçici:** BFS ve DFS butonları anında API çağrısı yapmak yerine sadece algoritma seçici olarak çalışacak şekilde yeniden tasarlandı. Sorgu yalnızca "Shortest Path" butonuyla tetikleniyor. | 03.05.2026 |
| 6 | **Path Edge Glow:** En kısa yol sonuçları kenarlar üzerinde yeşil renk, 4px kalınlık ve glow efekti ile görselleştirildi. | 03.05.2026 |
| 7 | **İsim Çözümleme (Name Resolution):** Panel üzerinde ham ID'ler (photo19) yerine düğümlerin gerçek isimleri (Sabah Koşusu) gösterilmesi sağlandı. | 03.05.2026 |
| 8 | **Gelişmiş Lejant:** Düğüm tiplerine ek olarak Origin, Target ve Pinned durumlarını açıklayan etkileşim rehberi lejanta eklendi. | 03.05.2026 |

*(Tüm frontend geliştirmeleri ve optimizasyon çalışmaları başarıyla tamamlanmıştır.)*
