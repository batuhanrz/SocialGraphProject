# Ara Rapor (Interim Report) — SocialGraph Project

**Tarih:** 30.04.2026  
**Ekip Üyeleri:** Batuhan, Özcan, Fatma Sude, Muhammed Furkan, Isra

---

## 1. Giriş ve Amaç

Bu rapor, Veri Yapıları dersi kapsamında geliştirilen **Property Graph Tabanlı Sosyal Ağ Modelleme** projesinin 30.04.2026 tarihli ara rapor teslimidir. Projenin temel amacı, karmaşık sosyal ağ ilişkilerini (arkadaşlık, etkileşim, katılım) temsil edebilen, yüksek performanslı ve "from scratch" (sıfırdan) veri yapıları üzerine kurulu bir sistem inşa etmektir.

## 2. Takım İçi İletişim ve Karar Alma Süreci

Proje süresince tüm mimari kararlar ekip içi ortak tartışmalarla alınmıştır:
- **Veri Yapısı Seçimi:** Hızlı arama için `Trie` ve O(1) erişim için `Hash Table` kullanılmasına karar verildi. Standard kütüphane (`Dictionary`, `List`) kullanımının çekirdek mantıkta yasaklanması konusunda mutabık kalındı.
- **Asenkron Yapı:** AI Worker'ın ana servisten bağımsız çalışması (Microservice yaklaşımı), sistemin ölçeklenebilirliği için kritik bir karar olarak uygulanmıştır.
- **Tasarım Dili:** Kullanıcı deneyimi için "Premium Swiss Minimal" tasarım dili benimsenmiştir.

## 3. GitHub İş Akışı ve PR Geçmişi

Projede "Git Flow" benzeri bir yapı uygulanmaktadır:
- **Main Branch:** Sadece stabil ve test edilmiş sürümler barındırılır (Projenin ana gövdesi).
- **Develop Branch:** Ekip üyelerinin kodlarının birleştiği ana geliştirme dalı.
- **Feature Branches:** Ekip üyelerinin uzmanlık alanlarına göre özelleşmiş dallar:
    - `feature/batuhan-core`: Veri yapıları (HashTable, Graph).
    - `feature/ozcan-algorithms`: BFS, DFS ve Zincir sorgular.
    - `feature/sude-frontend`: UI ve Görselleştirme.
    - `feature/furkan-infrastructure`: API ve AI Worker.
    - `feature/isra-optimization`: Testler ve Optimizasyon.
- **PR Kontrolü:** Her PR en az bir ekip üyesi tarafından gözden geçirilmiş ve çakışmalar (conflict) manuel olarak çözülmüştür.

## 4. Teknik Gelişim Özeti

### 4.1. Veri Yapıları (Faz 1)
- **CustomHashTable:** Linear probing ile implemente edildi. Load factor kontrolü eklendi.
- **CustomTrie:** Autocomplete ve Prefix search yetenekleri eklendi.
- **CustomQueue:** BFS traversal için optimize edildi.

### 4.2. Algoritmalar (Faz 2)
- **Graph Traversal:** BFS ve DFS algoritmaları PropertyGraph üzerinde test edildi.
- **Relational Query Engine:** Çok adımlı sorgu zincirleri (User -> Friend -> Photo) implemente edildi.
- **Arkadaş Önerisi:** Ortak arkadaş sayısına dayalı Triadic Closure algoritması eklendi.

### 4.3. Görselleştirme (Faz 3)
- **2D Rendering:** Vis-network ile düğüm ve kenar tiplerine göre özelleşmiş görselleştirme.
- **Side Panel:** Tıklanan düğümün özelliklerine O(1) hızında erişim ve detay gösterimi.

## 5. Hata ve Bulgular Raporu (Bug Report)

| Bulgu Kimliği | Tanım | Çözüm Durumu |
|---------------|-------|--------------|
| BUG-001 | AI Worker yazarken API'nin veri okuyamaması. | **Çözüldü** (ReaderWriterLockSlim eklendi) |
| BUG-002 | React hydration hataları. | **Çözüldü** (Client-side rendering ayarlandı) |
| BUG-003 | Trie aramasında küçük/büyük harf duyarlılığı. | **Çözüldü** (Normalization eklendi) |
| BUG-004 | Vis-network'te yüksek düğüm sayısında kasılma. | **İyileştirildi** (Physics engine optimize edildi) |

## 6. Sonuç ve Gelecek Planı

Proje, 30.04.2026 tarihi itibariyle tüm zorunlu isterleri karşılamaktadır. Önümüzdeki final döneminde (12-13. Hafta):
- Big-O analizi daha detaylı hale getirilecek.
- UML diyagramları son haline getirilecek.
- 10 dakikalık final demo videosu çekilecektir.

---
**GitHub Repo Linki:** [https://github.com/batuhanrz/SocialGraphProject](https://github.com/batuhanrz/SocialGraphProject)
