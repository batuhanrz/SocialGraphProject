# SocialGraphProject

## Proje Adı
Property Graph Tabanlı Sosyal Ağ Modelleme

## Amaç
Sosyal ağ sistemlerinde kullanılan property graph veri modelinin sadeleştirilmiş bir versiyonunu geliştirmek. Ağ üzerindeki varlıklar (kullanıcılar, fotoğraflar, etkinlikler) düğüm, aralarındaki ilişkiler (arkadaşlık, beğeni, katılım) kenar olarak modellenir. Sistem, sıfırdan implemente edilmiş veri yapıları (Hash Table, Trie, Queue) ile çok adımlı ilişkisel sorguları verimli şekilde gerçekleştirir.

## Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| **Backend (API)** | ASP.NET Core Web API, C# |
| **AI Simulation** | .NET BackgroundService (IHostedService) |
| **Frontend (UI)** | React, TypeScript, Vis-network |
| **Konteynerizasyon** | Docker, Docker Compose |

## Proje Mimarisi (Monorepo)

```
SocialGraphProject/
├── src/
│   ├── SocialGraph.API/      # Ana iş mantığı servisi (Property Graph + Veri Yapıları)
│   ├── SocialGraph.AI/       # Asenkron veri simülasyon mikroservisi
│   └── SocialGraph.UI/       # React tabanlı kullanıcı arayüzü
├── Markdowns/                # Proje dökümantasyonu
│   ├── Roadmaps/             # Yol haritası
│   ├── Sprints/              # Sprint detayları
│   ├── Project/              # Proje bağlamı ve raporlar
│   └── Personal_Reports/     # Kişisel katkı raporları
└── README.md
```

## Ekip

| Rol | Kişi | Sorumluluk |
|-----|------|------------|
| Core Data Engineer | Batuhan | Hash Table, Node/Edge, PropertyGraph, Thread-Safety |
| Algorithm Master | Özcan | Queue, BFS, DFS, Çok Adımlı Sorgular |
| Frontend Lead | Fatma Sude | React UI, Vis-network Görselleştirme |
| Architect & Infrastructure | Muhammed Furkan | ASP.NET Core API, AI Worker, Docker |
| Testing & Analysis | Isra | Trie, Sentetik Veri, Testler, Big-O Analizi |

## Çalıştırma

### Gereksinimler
- Docker & Docker Compose
- (Geliştirme için) .NET 8 SDK, Node.js 20+

### Hızlı Başlangıç
```bash
docker-compose up --build
```

| Servis | URL |
|--------|-----|
| Frontend (UI) | http://localhost:3000 |
| Backend (API) | http://localhost:5000 |
| Swagger Docs | http://localhost:5000/swagger |

### Geliştirme Modu
```bash
# API
cd src/SocialGraph.API
dotnet run

# UI
cd src/SocialGraph.UI
npm install
npm run dev
```

## Lisans
Bu proje bir üniversite ödevi kapsamında geliştirilmektedir.
