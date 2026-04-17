# SocialGraph.AI

Veri simulasyonunu asenkron olarak yurutecek, ana API servisinden bagimsiz calisan mikroservis.

## Sorumluluk
- BackgroundService / IHostedService tabanli worker
- Her 15 saniyede sentetik veri uretimi
- API servisi ile HTTP uzerinden haberlesme
- Graceful shutdown ve hata yonetimi

## Teknoloji
- .NET BackgroundService
- C#
