# Muhammed Furkan'in Tamamladigi Gorevler ve Katkilar

> Bu dokuman, projenin "Architect & Infrastructure" rolunu ustlenen Muhammed Furkan tarafindan basariyla tamamlanan teknik gorevlerin ozetini icerir.

---

## Sprint 1 Katkilari (Backend Mimarisi ve API Sozlesmesi)
**Rol Hedefi:** Sosyal ag verilerini dis dunyaya acacak ASP.NET Core Web API altyapisinin kurulmasi, DTO sozlesmelerinin tanimlanmasi ve frontend/AI servisleriyle entegrasyona hazir hale getirilmesi.

### Yapilan Islemler (Sprint 1.4)
| No | Tamamlanan Gorev Ozeti | Tarih |
|:---:|:---|:---:|
| 1 | Mevcut Console Application, ASP.NET Core Web API projesine donusturuldu (SDK degisimi, Swashbuckle entegrasyonu). | 25.04.2026 |
| 2 | Program.cs uzerinde CORS, Swagger/OpenAPI, JSON Serialization ve Singleton DI yapilandirmasi tamamlandi. | 25.04.2026 |
| 3 | Frontend interface'leriyle (INode, IEdge) birebir uyumlu DTO modelleri (NodeDto, EdgeDto, SearchRequestDto, TraversalResultDto) olusturuldu. | 25.04.2026 |
| 4 | NodesController, SearchController ve TraversalController ile toplam 5 placeholder endpoint Swagger uzerinde dokumante edildi. | 25.04.2026 |
| 5 | Sprint 1.1/1.2 test kodlari TestRunner.cs dosyasina tasinarak mevcut islevsellik korundu. | 25.04.2026 |

---

## Sprint 2 Katkıları (API Entegrasyonu ve Worker Altyapısı)

### Yapılan İşlemler (Sprint 2.4)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | `NodesController.cs` güncellendi: Tüm düğümler ve kenarlar sahte veri yerine `PropertyGraph` üzerinden getirilmeye başlandı. | 26.04.2026 |
| 2 | `SearchController.cs` güncellendi: Frontend ile tam uyum için `/api/search/autocomplete` ucu yazılarak `CustomTrie` aramasına bağlandı. | 26.04.2026 |
| 3 | `TraversalController.cs` güncellendi: `BFS`, `DFS` ve `ShortestPath` uçları `POST` yerine frontend dostu `GET` yapısıyla `GraphTraversal` algoritmasına bağlandı. | 26.04.2026 |
| 4 | `SocialGraph.AI` projesi `.NET Worker Service` formatında oluşturularak her 15 saniyede bir log atan iskelet mekanizması (`BackgroundService`) kuruldu. | 26.04.2026 |

*(Diger sprintler geldikce eklenecektir...)*
