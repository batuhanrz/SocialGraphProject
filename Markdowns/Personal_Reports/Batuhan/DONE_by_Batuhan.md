# Batuhan — Kişisel Katkı Raporu

...

## SPRINT 0: Proje Başlatma ve Sistem Mimarisinin Kurulması

### Sprint 0.1: GitHub Repo Kurulumu ve Baslangic Yapilandirmasi [TAMAMLANDI]
| # | Yapılan İş | Tarih |
|---|-----------|-------|
| 1 | GitHub üzerinde "SocialGraphProject" isimli repository oluşturuldu. | 17.04.2026 |
| 2 | Projenin amacını, ekip bilgilerini, mimari yapısını ve çalıştırma talimatlarını içeren `README.md` hazırlandı. | 17.04.2026 |
| 3 | .NET (`bin/`, `obj/`) ve Node.js (`node_modules/`) patternlarını içeren `.gitignore` dosyası oluşturuldu. | 17.04.2026 |
| 4 | Proje dökümantasyonu için `Markdowns/` klasör yapısı kuruldu (Roadmaps, Sprints, Project, Personal_Reports). | 17.04.2026 |
| 5 | Süreç yönetimi için `TODO.md` ve `DONE.md` dosyaları oluşturuldu. | 17.04.2026 |
| 6 | Projenin 5 sprint'lik detaylı yol haritası (`Roadmap_Full.md`) ve tüm sprint detay dosyaları (Sprint 0–4) hazırlandı. | 17.04.2026 |

### Sprint 0.2: Mikroservis Klasor Yapilandirmasi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|...|...........|.......|
| 1 | `src/SocialGraph.API/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |
| 2 | `src/SocialGraph.AI/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |
| 3 | `src/SocialGraph.UI/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |

### Sprint 0.3: Branch Stratejisi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|...|...........|.......|
| 1 | `develop` ana entegrasyon branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 2 | `feature/batuhan-core` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 3 | `feature/ozcan-algorithms` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 4 | `feature/sude-frontend` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 5 | `feature/furkan-infrastructure` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 6 | `feature/isra-optimization` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 7 | `git branch -r` ile tum branch'lerin remote'ta gorundugu dogrulandi. | 17.04.2026 |
| 8 | Ekip uyelerine branch erişim bilgisi iletildi. | 17.04.2026 |

### Sprint 0.4: Teknik Dokumantasyon [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|...|...........|.......|
| 1 | Projenin 5 sprintlik genel plani `Roadmap_Full.md` dosyasinda detaylandirildi. | 17.04.2026 |
| 2 | Her sprint (0-4) icin alt gorevleri barindiran `Sprint_X_detailed.md` dosyalari olusturuldu. | 17.04.2026 |
| 3 | Universite ara rapor gereksinimi olan `Interim_Report.md` dosyasi taslak olarak hazirlandi. | 17.04.2026 |

### Sprint 0.5: Kopru Asamasi — GitHub Issues On Hazirligi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|...|...........|.......|
| 1 | HashTable çakışma (collision) yönetimi üzerine ekip içi teknik değerlendirme yapıldı. | 18.04.2026 |
| 2 | AI servisi ile API arasındaki asenkron veri iletişim protokolü kararlaştırıldı. | 18.04.2026 |
| 3 | Mimarideki temel karar alma süreçleri takım içi toplantıda karara bağlanarak kayıt altına alındı. | 18.04.2026 |

## SPRINT 1: Altyapı ve Çekirdek Veri Yapıları

### Sprint 1.1: Node/Edge Modelleri + Custom Hash Table [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|...|...........|.......|
| 1 | Standard C# Dictionary sınıfı yasaklanarak %100 sıfırdan Open Addressing (Linear Probing) özellikli `CustomHashTable<TKey, TValue>` yazıldı. | 18.04.2026 |
| 2 | Load Factor > %75 durumunda O(N) maliyetle resize işlemini tetikleyen dinamik `Rehash` sistemi kurgulandı ve eklendi. | 18.04.2026 |
| 3 | User, Photo ve Event türündeki property'si içerisinde Custom Hash Table barındıran `Node` nesnesi ve aralarındaki bağlantıları kuran `Edge` nesnesi tasarlandı. | 18.04.2026 |
| 4 | Terminal üzerinden 2500 adet rastgele elemanla yük testi (load-test) yapılarak O(1) maliyetli arama/ekleme ve Rehashing mekanizması valide edildi. | 18.04.2026 |
| 5 | Geliştirilen Sprint 1.1 kodları GitHub üzerinden PR (Pull Request) ile `develop` aktarımı için sunuldu. | 19.04.2026 |

## SPRINT 2: Property Graph Entegrasyonu ve API Servisleri

### Sprint 2.1: Adjacency List Tabanli Property Graph + DI Kaydi [TAMAMLANDI]
**Rol Hedefi:** Mevcut veri yapilarini (CustomHashTable, Node, Edge) birlestiren cekirdek PropertyGraph sinifini adjacency list tabaniyla olusturmak ve DI container'a kaydetmek.

| # | Tamamlanan Gorev Ozeti | Tarih |
|:---:|:---|:---:|
| 1 | `PropertyGraph` sinifi sifirdan implemente edildi. Adjacency list yapisi tamamen `CustomHashTable<string, CustomHashTable<string, Edge>>` ile kuruldu. Standart kutuphane yasagina tam uyum saglandi. | 26.04.2026 |
| 2 | 3 dugum turu (User, Photo, Event) ve 4 kenar turu (FRIEND, LIKES, POSTED, ATTENDS) tip dogrulama mekanizmasi ile desteklendi. Gecersiz tiplerde `ArgumentException` firlatilir. | 26.04.2026 |
| 3 | Yonsuz kenarlar (FRIEND) icin cift yonlu adjacency kaydi (A->B ve B->A), yonlu kenarlar (LIKES, POSTED, ATTENDS) icin tek yonlu kayit mekanizmasi implemente edildi. | 26.04.2026 |
| 4 | `ReaderWriterLockSlim` ile temel read/write lock altyapisi kuruldu. Okuma islemleri read lock, yazma islemleri write lock kullanir. (Context.md B.1 eszemanlilik gereksinimi) | 26.04.2026 |
| 5 | `AddNode`, `AddEdge`, `GetNode`, `GetNeighbors`, `GetEdgesByType`, `RemoveNode`, `RemoveEdge`, `GetAllNodes`, `GetAllEdges` operasyonlari Big-O XML yorum dokumantasyonu ile yazildi. | 26.04.2026 |
| 6 | PropertyGraph `Program.cs`'de Singleton olarak DI container'a kaydedildi. Sprint 1 kayitlari (CustomHashTable, CustomTrie) geri uyumluluk icin korundu. | 26.04.2026 |
| 7 | `dotnet build` 0 hata ve `dotnet test` 14/14 test basarili (regresyon yok) ile dogrulama tamamlandi. | 26.04.2026 |
| 8 | Projenin `Interim_Report.md` dosyasi genisletildi ve `README.md` ana sayfasina Ara Rapor durum bildirimi ile yonlendirme linki eklendi. | 26.04.2026 |
| 9 | `ReaderWriterLockSlim` ile thread-safety optimizasyonu yapildi, kilit kapsami daraltildi. | 27.04.2026 |
| 10 | `PropertyGraphConcurrencyTests.cs` ile 15+ thread yuk testi basariyla tamamlandi. | 27.04.2026 |
| 11 | `GetAllEdges` metodu O(E) maliyetinden lojik sayac kullanan optimize versiyona gecirildi. | 27.04.2026 |
