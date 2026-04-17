PROJE KONU3: 
Proje Adı: Property Graph Tabanlı Sosyal Ağ Modelleme 
Senaryo ve Temel Amaç: 
Bu projede, sosyal ağ sistemlerinde kullanılan property graph veri modelinin sadeleştirilmiş bir versiyonu 
geliştirilecektir. 
Ağ üzerindeki varlıklar (kullanıcılar, fotoğraflar, etkinlikler vb.) düğümler (vertex), bu varlıklar arasındaki ilişkiler 
ise (arkadaşlık, beğeni, katılım vb.) kenarlar (edge) olarak modellenir. Her düğüm ve kenar, ek özellikler 
(properties) taşıyabilir. 
Temel amaç, bu yapı üzerinde çok adımlı ilişkisel sorguları, verimli veri yapıları ve algoritmalar kullanarak 
gerçekleştirebilen bir sistem geliştirmektir. Proje, gerçek dünya sosyal ağlarının basitleştirilmiş bir modelini ele 
alarak graf veri yapıları ve arama algoritmalarını uygulamalı olarak incelemeyi hedefler. 
A.1. Zorunlu Veri Yapıları (Faz 1) 
Bu proje kapsamında aşağıdaki temel veri yapıları sıfırdan (from scratch) implemente edilecektir. (Arayüz ve 
yardımcı işlemler için standart kütüphaneler kullanılabilir.) 
Property Graph (Heterojen Graf): 
Graf yapısı, farklı türde düğümleri ve özellikli kenarları destekleyecek şekilde tasarlanacaktır. 
Düğüm türleri: User, Photo, Event vb. 
Her düğüm: 
• benzersiz ID 
• tür bilgisi (type) 
• özellikler (properties) 
Kenarlar: 
• yönlü veya yönsüz olabilir 
• ilişki türü (FRIEND, LIKES vb.) 
• ek özellikler (örn. tarih bilgisi) 
Graf yapısı, komşuluk listesi (adjacency list) tabanlı olarak implemente edilecektir. 
Karma Tablo (Hash Table): 
Tüm düğümler benzersiz ID ile saklanacaktır. 
Amaç: düğümlere hızlı erişim sağlamak 
Erişim süresi: ortalama O(1) 
Trie (Önek Ağacı) veya Ters Dizin (Inverted Index): 
Metin tabanlı arama işlemleri için kullanılacaktır: 
Trie: otomatik tamamlama (autocomplete) 
Ters dizin: anahtar kelimeye dayalı arama 
Bu yapılardan en az biri implemente edilecektir (diğeri opsiyonel olarak eklenebilir). 
Kuyruk (Queue): Graf üzerinde BFS (Genişlik Öncelikli Arama) algoritmasının uygulanması için kullanılacaktır. 
Kullanım alanları: 
• Bağlantı derecesi (degrees of separation) hesaplama 
• Ağı katmanlı şekilde tarama 
• En kısa yol (ağırlıksız graf için) bulma 
A.2. Algoritmalar ve Yardımcı AI Kullanımı (Faz 2) 
Zorunlu Algoritmalar: 
• BFS ve DFS implementasyonu 
• Filtreli graf traversal algoritmaları (çok adımlı sorgular için) 
• Düğüm ve ilişki bazlı arama işlemleri 
Ek olarak (opsiyonel): 
Triadic closure (arkadaş önerisi mantığı) 
Basit merkezilik ölçümleri (node importance) 
Algoritmaların zaman ve uzay karmaşıklıkları analiz edilecektir. 
Sorgu Modeli: Sistem, önceden tanımlanmış veya basitleştirilmiş bir sorgu yapısı üzerinden çalışacaktır. 
Örnek sorgu akışı: 
Kullanıcı → arkadaşlar → katıldığı etkinlikler → bu etkinliklerdeki fotoğraflar 
Bu sorgular, ardışık graf traversal işlemleri ile gerçekleştirilecektir. 
Sentetik Veri Üretimi (Opsiyonel): 
Test verileri: 
Programatik olarak üretilecektir 
İsteğe bağlı olarak GenAI araçları (ChatGPT, Gemini vb.) kullanılabilir 
Amaç: 
• farklı ağ yapılarında sistem davranışını gözlemlemek 
• performans ve doğruluk testleri yapmak 
A.3. Spesifik Arayüz Gereksinimleri (Faz 3) 
Arayüz, bir sosyal medya uygulamasından ziyade, bir graf veri sorgulama ve görselleştirme aracı olarak 
tasarlanacaktır. 
Graf Görselleştirme: 
Sorgu sonuçları, 2D node-link diyagramı olarak gösterilecektir 
Düğüm türleri farklı renk veya şekillerle temsil edilecektir 
Örn: Kullanıcı → daire, Fotoğraf → kare 
Etkileşim: 
Kullanıcı, belirli bir sorguyu çalıştırabilecektir 
Graf üzerinde herhangi bir düğüme tıklandığında: 
o düğümün özellikleri yan panelde gösterilecektir 
bu bilgiler Hash Table üzerinden hızlı erişimle elde edilecektir 
Performans Notu: 
Sistem, küçük ve orta ölçekli graflar (örneğin yüzlerce düğüm) üzerinde test edilecek şekilde tasarlanacaktır. 
Yoğun veri durumlarında görselleştirme karmaşıklığını azaltmak için filtreleme veya sınırlama yöntemleri 
uygulanabilir. 

PROJE DEĞERLENDİRME STANDARTLARI
(Tüm projeler değerlendirme kriterlerini sağlamalıdır.)
B.1. Takım Çalışması ve Teknolojik Altyapı [40 Puan]
Ekip olarak, bu projede aşağıdaki standartlara uymak zorunludur, uymayan ekip üyeleri bu bölümden
puan alamazlar.
* Eşzamanlılık ve Mikroservis Yaklaşımı: Ekip sayısının büyüklüğü göz önüne alınarak, yapay
zeka simülasyon motorunun, veri yapılarının tutulduğu ana bellekten bağımsız/asenkron
çalışacak bir yapıda (Thread-safe veya ayrı servisler halinde) tasarlanması beklenmektedir.
* Versiyon Kontrolü (Git): Geliştirme süreci GitHub/GitLab üzerinden yürütülecek, master/main
dalına doğrudan kod atılmayacak, branch ve Pull Request (PR) mekanizmaları kullanılacaktır.
Takım üyelerinin commit geçmişi notlandırmada kritik rol oynar.
B.2. Teslim Edilecekler [40 Puan]
* Ekip olarak, bu projede aşağıdaki standartların hepsine uymak zorunludur, uyulmadığı
takdirde bu bölümden ekipçe puan alınamaz.
* Kaynak Kod Deposu (Repository): README.md dosyasında projenin mimarisinin ve nasıl
ayağa kaldırılacağının detaylıca anlatıldığı sayfa.
* Docker Konfigürasyonları: Tüm sistemin (Frontend, Backend, AI Servisi) bağımlılık sorunu
yaşamadan tek bir docker-compose up komutuyla çalışmasını sağlayan Dockerfile ve docker-
compose.yml dosyaları.
* Proje Raporu ve Analiz: Projenin UML diyagramlarını, veri yapılarının zaman karmaşıklığı
(Big-O) analizlerini ve AI API'sine gönderilen prompt'ların dökümünü içeren kapsamlı rapor.
* Demo Videosu Linki: Tüm sistemin çalıştığını gösteren, en fazla 10 dakikalık video. (Arayüz
demosu, dinamik veri değişiminin gösterimi ve yazılan core veri yapılarının kod üzerinden
hızlıca anlatımı).
B.3. Teslim Kuralları ve Değerlendirme (Code Defense) [20 Puan]
İsimlendirme Şartı: Takım üyelerinin ad ve soyadları, veritabanı veya fonksiyonlar içine Türkçe karakter
içermeyen formatta yerleştirilmiş olmalıdır.
KOD SAVUNMASI (Jüri Sunumu): Teslim edilen projeler arasından gruplar canlı code defense'e
alınacaktır. Gruptaki herhangi bir öğrencinin, kendi yazmadığı bir modül dahi olsa projede kullanılan
temel veri yapılarının çalışma mantığını ve zaman karmaşıklığını açıklayabilmesi zorunludur. Başarı,
sadece kodun çalışmasına değil, ekibin konuya hakimiyetine bağlıdır. Bu sunuma katılmayan
öğrenciler proje değerlendirme notu alamazlar. Proje teslim edildiğinde tam fonksiyonel şekilde
çalışıyor olmalıdır.
Son Teslim: İlan edilecek tarihte sistem üzerinden yapılacaktır. Gecikmeler kabul edilmez (12. veya 13.
Hafta olması planlanıyo