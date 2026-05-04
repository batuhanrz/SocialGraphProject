# Batuhan - Kisisel Katki Raporu

---

## SPRINT 0: Proje Baslatma ve Sistem Mimarisinin Kurulmasi

### Sprint 0.1: GitHub Repo Kurulumu ve Baslangic Yapilandirmasi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|---|-----------|-------|
| 1 | GitHub uzerinde "SocialGraphProject" isimli repository olusturuldu. | 17.04.2026 |
| 2 | Projenin amacini, ekip bilgilerini, mimari yapisini ve calistirma talimatlarini iceren `README.md` hazirlandi. | 17.04.2026 |
| 3 | .NET (`bin/`, `obj/`) ve Node.js (`node_modules/`) patternlarini iceren `.gitignore` dosyasi olusturuldu. | 17.04.2026 |
| 4 | Proje dokumantasyonu icin `Markdowns/` klasor yapisi kuruldu (Roadmaps, Sprints, Project, Personal_Reports). | 17.04.2026 |
| 5 | Surec yonetimi icin `TODO.md` ve `DONE.md` dosyalari olusturuldu. | 17.04.2026 |
| 6 | Projenin 5 sprint'lik detayli yol haritasi (`Roadmap_Full.md`) ve tum sprint detay dosyalari (Sprint 0-4) hazirlandi. | 17.04.2026 |

### Sprint 0.2: Mikroservis Klasor Yapilandirmasi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|---|-----------|-------|
| 1 | `src/SocialGraph.API/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |
| 2 | `src/SocialGraph.AI/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |
| 3 | `src/SocialGraph.UI/` dizini olusturuldu, servis sorumlulugunu anlatan README.md eklendi. | 17.04.2026 |

### Sprint 0.3: Branch Stratejisi [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|---|-----------|-------|
| 1 | `develop` ana entegrasyon branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 2 | `feature/batuhan-core` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 3 | `feature/ozcan-algorithms` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 4 | `feature/sude-frontend` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 5 | `feature/furkan-infrastructure` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |
| 6 | `feature/isra-optimization` branch'i olusturuldu ve remote'a push edildi. | 17.04.2026 |

---

## SPRINT 1: Altyapi ve Cekirdek Veri Yapilari

### Sprint 1.1: Node/Edge Modelleri + Custom Hash Table [TAMAMLANDI]
| # | Yapilan Is | Tarih |
|---|-----------|-------|
| 1 | Standard C# Dictionary sinifi yasaklanarak %100 sifirdan Open Addressing (Linear Probing) ozellikli `CustomHashTable<TKey, TValue>` yazildi. | 18.04.2026 |
| 2 | Load Factor > %75 durumunda O(N) maliyetle resize islemini tetikleyen dinamik `Rehash` sistemi kurgulandi ve eklendi. | 18.04.2026 |
| 3 | User, Photo ve Event turundeki property'si icerisinde Custom Hash Table barindiran `Node` nesnesi ve aralarindaki baglantilari kuran `Edge` nesnesi tasarlandi. | 18.04.2026 |
| 4 | Terminal uzerinden 2500 adet rastgele elemanla yuk testi (load-test) yapilarak O(1) maliyetli arama/ekleme ve Rehashing mekanizmasi valide edildi. | 18.04.2026 |

---

## SPRINT 2: Property Graph Entegrasyonu ve API Servisleri

### Sprint 2.1: Adjacency List Tabanli Property Graph + DI Kaydi [TAMAMLANDI]
| # | Tamamlanan Gorev Ozeti | Tarih |
|---|------------------------|-------|
| 1 | `PropertyGraph` sinifi sifirdan implemente edildi. Adjacency list yapisi tamamen `CustomHashTable<string, CustomHashTable<string, Edge>>` ile kuruldu. | 26.04.2026 |
| 2 | `ReaderWriterLockSlim` ile thread-safety optimizasyonu yapildi, kilit kapsami daraltildi. | 27.04.2026 |
| 3 | `GetAllEdges` metodu O(E) maliyetinden lojik sayac kullanan optimize versiyona gecirildi. | 27.04.2026 |

---

## SPRINT 4: Sistem Entegrasyonu ve Final Dokumantasyonu

### Sprint 4.1: UML Diyagramlari + B.3 Compliance Audit [TAMAMLANDI]
**Rol Hedefi:** Sistemin mimari semasini (UML) hazirlamak, kod kalitesini artirmak ve B.3 (Karakter Seti) sartina tam uyum saglamak.

| # | Tamamlanan Gorev Ozeti | Tarih |
|---|------------------------|-------|
| 1 | **B.3 Compliance Audit:** Tum kod tabani tarandi; Turkce karakterler temizlenerek juri savunmasina hazir hale getirildi. | 04.05.2026 |
| 2 | **"Life Illusion" Mekanizmasi:** Simulasyon dongusu fazlara bolunerek (0s - 7s) feed akisinin daha organik ve canli gorunmesi saglandi. | 04.05.2026 |
| 3 | **UI Name Resolution:** Feed uzerindeki teknik ID'ler (user8 vb.) yerine gercek isimlerin (Name/Title) gosterilmesi saglandi. | 04.05.2026 |
| 4 | **Interactive Edge Highlighting:** Feed uzerindeki aksiyonlara tiklandiginda grafikteki ilgili cizginin parlamasi (glow) saglandi. | 04.05.2026 |
| 5 | **Connectivity Protection Filter:** Iliski sayisi 3'un altinda olan dugumler yikici aksiyonlardan muaf tutularak grafik butunlugu korundu. | 04.05.2026 |
| 6 | **Dinamik Veri Temizligi:** Silinen kenarlarin grafikten anlik olarak yok olmasi icin `DataSet` senkronizasyon mantigi guncellendi. | 04.05.2026 |
| 7 | **Professional Sanitization:** Kod tabanindaki tum teknik standart disi yorumlar ve meta-metinler temizlendi. | 04.05.2026 |
| 8 | `UML_Diagrams.md`: Sistemin tum mimari yapisi (Class, Sequence, Component) finalize edildi. | 04.05.2026 |

---

**Batuhan R.**
*Project Lead & Core Data Engineer*
