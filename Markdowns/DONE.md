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

