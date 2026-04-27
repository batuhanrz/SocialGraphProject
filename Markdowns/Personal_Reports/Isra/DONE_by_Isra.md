# Isra'nın Tamamladığı Görevler ve Katkılar

Bu döküman, projenin "Testing & Analysis Specialist" rolünü üstlenen Isra tarafından başarıyla tamamlanan teknik görevlerin özetini içerir.

---

## Sprint 1 Katkıları (Custom Trie + Test Altyapısı)
**Rol Hedefi:** Metin tabanlı arama ve otomatik tamamlama için Custom Trie veri yapısının sıfırdan yazılması ve proje genelinde birim test altyapısının kurulması.

### Yapılan İşlemler (Sprint 1.5)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | TrieNode sınıfı, çocuk düğüm yönetiminde projenin kendi CustomHashTable yapısı kullanılarak sıfırdan yazıldı. | 25.04.2026 |
| 2 | CustomTrie sınıfı Insert, Search, StartsWith ve AutoComplete operasyonlarıyla sıfırdan implemente edildi. Case-insensitive çalışır. | 25.04.2026 |
| 3 | xUnit test projesi (SocialGraph.Tests) oluşturuldu ve API projesine referans eklendi. | 25.04.2026 |
| 4 | CustomHashTable için 4, CustomQueue için 3, CustomTrie için 7 olmak üzere toplam 14 birim test senaryosu yazıldı. | 25.04.2026 |
| 5 | 120+ kelimelik AutoComplete yük testi dahil tüm testler dotnet test ile %100 başarıyla geçti. | 25.04.2026 |

---

## Sprint 2 Katkıları (Sentetik Veri Üretimi + Analiz)
**Rol Hedefi:** Grafa yüksek hacimli ve gerçekçi test verisi sağlayacak AI Worker simülasyon motorunun (DataGenerator) kodlanması ve test kapsamının genişletilmesi.

### Yapılan İşlemler (Sprint 2.5)
| No | Tamamlanan Görev Özeti | Tarih |
|:---:|:---|:---:|
| 1 | GenAI Stratejisi: GitHub Discussions üzerinden yapılan tartışma sonucu "Offline Prompting" stratejisi kararlaştırıldı. Kullanılan prompt [prompt.md](../../Prompts/prompt.md) dosyasında dökümante edildi. | 26.04.2026 |
| 2 | DataGenerator: Gemini 3.1 Pro çıktıları kullanılarak 100+ sofistike düğüm (User, Photo, Event) verisi koda statik olarak gömüldü. | 26.04.2026 |
| 3 | Topoloji Algoritmaları: Dense, Sparse, Star ve Chain tipi graf yapılarını sıfırdan üreten algoritmalar DataGenerator içinde implemente edildi. | 26.04.2026 |
| 4 | API Entegrasyonu: AI Worker'ın API'ye veri basabilmesi için batch POST uçları NodesController'a eklendi ve Trie indeksiyle bağlandı. | 26.04.2026 |
| 5 | Birim Testler: PropertyGraphTests sıfırdan yazıldı; HashTable ve Queue testleri genişletildi. Toplam test sayısı 23'e çıkarıldı (23/23 Passed). | 26.04.2026 |
| 6 | CustomTrie Optimizasyonu: Trie yapısına ID saklama yeteneği kazandırılarak arama sonuçlarının doğrudan Graf düğümlerine ulaşması sağlandı. | 26.04.2026 |

---

*(Diğer sprintler geldikçe eklenecektir...)*
