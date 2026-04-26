# Ara Rapor (Interim Report) — 30.04.2026

> **Not:** Bu doküman, Veri Yapıları Projesi kapsamında 30.04.2026 tarihli Ara Rapor teslimi için hazırlanan temel iskeleti yansıtmaktadır. Mevcut haliyle ara rapor kriterleri karşılanmış olup, teslim gününe kadar geliştirme süreci devam ettiği için ekip üyeleri tarafından rapora yeni eklemeler/düzenlemeler gelebilir.

Bu dokuman, Veri Yapilari Projesi kapsaminda 30.04.2026 tarihli Ara Rapor (Interim Report) teslimi icin hazirlanmistir. 

Projenin baslangicindan bu gune kadar yapilan mimari tartismalar, teknoloji secimleri, is bolumu ve kod entegrasyonu detaylari asagida sunulmustur. Tum calismalar "master/develop" branch'i uzerinde toplanmakta olup, projeye ait repository github uzerinde aktif olarak kullanilmaktadir.

## 1. Proje Durumu ve GitHub Guncel Durumu

Projenin temel iskeleti, mikroservis mimarisine ve takim calismasina uygun olacak sekilde kurgulanmistir:
- **Repository:** Proje icin [SocialGraphProject] adli GitHub repository'si olusturuldu.
- **Branch Stratejisi:** Projede ana entegrasyon dali olarak `develop` ve `main` kullanilmaktadir. Her ekip uyesi (`Batuhan`, `Ozcan`, `Fatma Sude`, `Muhammed Furkan`, `Isra`) kendi `feature/*` dallarinda (branch) calismalarini yurutmektedir.
- **Pull Requests (PR) Stratejisi:** Projede profesyonel bir kod birleştirme hiyerarşisi kurgulanmıştır. Mini sprint adımlarında (örneğin Sprint 1.1) her ekip üyesi geliştirme yaptığı kendi `feature/*` branch'inden ortak `develop` branch'ine PR açar ve kodlar buraya entegre edilir. Bir sprintin tamamı bittiğinde (örneğin tüm Sprint 1 tamamlandığında) ise, `develop` branch'inden `main` (ana) branch'ine tek bir ana sürüm PR'ı açılarak release edilir.
- **Gorev Takibi:** `Markdowns/Sprints` ve `Markdowns/Roadmaps` klasorleri altinda projenin tüm fazlari, haftalik gorevleri ve yapilan isler (`TODO.md`, `DONE.md`) izlenmektedir.

## 2. Teknoloji Secim Kararlari (Tech Stack)

Ekip ici yapilan tartismalar ve projenin gereksinimleri dogrultusunda asagidaki teknolojiler secilmistir:
* **Backend:** `ASP.NET Core Web API`. Kapsamli dependency injection ve yuksek performansli sunucu yetenekleri nedeniyle tercih edildi.
* **Veri Yapilari:** `Custom Hash Table` (Linear Probing ile), `Custom Queue`, `Custom Trie` ve `PropertyGraph` modelleri **hicbir standart kutuphane kullanilmadan** sifirdan C# ile yazilmistir. Bellekte tek bir instance (Singleton) olarak yasar.
* **Frontend:** `React + TypeScript`. Kullanici arayuzu ve dinamik arama sonuclari icin secildi. Graf gorsellestirmesi Sprint 3'te `Vis-network` kutuphanesi ile saglanacaktir.
* **AI Worker:** Sentetik verilerin uretilmesi ana bellegi yormamasi icin `SocialGraph.AI` adli bagimsiz bir .NET BackgroundService mikroservisine ayrilmistir.

## 3. GitHub Issues - Hata, Bulgu ve Mimari Karar Kayitlari

Proje baslangicinda ve Sprint 1 sirasinda, ekibin karsilastigi mimari zorluklar GitHub "Issues" ve "Discussions" sekmelerinde tartisilarak karara baglanmistir:

1. **[Bulgu] Ozel HashTable Implementasyonunda Collision Yonetimi:** Dugumlerin (Nodes) ram uzerinde hizli bulunabilmesi icin Chaining yerine "Linear Probing" (Open Addressing) teknigi secildi. CPU Cache Miss oranini dusurmek icin GitHub'da tartisildi ve onaylandi. *(Acan: Batuhan)*
2. **[Teknik Tartisma] AI Simulasyon Servisi ile API Arasindaki Iletisim:** AI Worker ile ana API arasindaki asenkron iletisimde gRPC veya RabbitMQ yerine donanim/zaman maliyeti dusunulerek "Retry mekanizmali HTTP POST" kullanilmasi kararlastirildi. *(Acan: Furkan)*

## 4. Kod Entegrasyonu ve Gelecek Adimlar

Su ana kadar yapilan Sprint 1, Sprint 2.1 ve Sprint 2.2 entegrasyonlarinda:
- Cekirdek `Node` ve `Edge` siniflari baglandi.
- Grafin temel iskeleti olan `PropertyGraph` adjacency list tabanli olarak `develop` branch'ine basariyla merge edildi.
- `CustomQueue` kullanılarak BFS/DFS algoritmaları ve iki düğüm arası en kısa yolu bulan `ShortestPath` fonksiyonu esnek filtreleme yetenekleriyle birlikte `PropertyGraph` sistemine tam entegre edildi.
- API ve UI projeleri ayaga kaldirildi ve aralarindaki CORS yapilandirmalari tamamlandi.
- Frontend tarafında native `fetch` destekli API servis modülleri oluşturuldu; arama ve sonuç listeleme (Autocomplete UI) bileşenleri state yönetimleriyle birlikte backend etkileşimine hazır hale getirildi.

**Gelecek Adim (Sprint 2 Devami & Sprint 3):**
Mevcut PropertyGraph uzerine gercek zamanli sentetik veri akisinin baglanmasi (AI Worker tarafindan) ve frontend arayuzunden gelen arama (Trie tabanli) ve gezinti (BFS/DFS tabanli) isteklerinin REST API uzerinden PropertyGraph'a entegre edilmesidir. Eslik eden read/write lock (eszamanlilik) optimizasyonlari saglanacaktir.