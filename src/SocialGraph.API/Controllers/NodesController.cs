using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SocialGraph.API.DTOs;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Dugum (Node) ve Kenar (Edge) islemleri icin API endpoint'leri.
    /// PropertyGraph kullanilarak gercek verilere erisilir.
    /// Gelistiren: Batuhan (Core Logic)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly PropertyGraph _graph;
        private readonly CustomTrie _trie;

        public NodesController(PropertyGraph graph, CustomTrie trie)
        {
            _graph = graph;
            _trie = trie;
        }

        /// <summary>
        /// Tum dugumleri listeler.
        /// GET /api/nodes
        /// </summary>
        [HttpGet]
        public ActionResult<List<NodeDto>> GetAll()
        {
            var nodes = _graph.GetAllNodes();
            var result = new List<NodeDto>(nodes.Length);

            foreach (var node in nodes)
            {
                result.Add(MapToDto(node));
            }

            return Ok(result);
        }

        /// <summary>
        /// Belirli bir dugumu ID ile getirir.
        /// GET /api/nodes/{id}
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<NodeDto> GetById(string id)
        {
            var node = _graph.GetNode(id);
            if (node != null)
            {
                return Ok(MapToDto(node));
            }

            return NotFound(new { Message = $"Node '{id}' bulunamadi." });
        }

        /// <summary>
        /// Belirli bir dugumun kenarlarini listeler.
        /// GET /api/nodes/{id}/edges
        /// </summary>
        [HttpGet("{id}/edges")]
        public ActionResult<List<EdgeDto>> GetEdges(string id)
        {
            var node = _graph.GetNode(id);
            if (node == null)
            {
                return NotFound(new { Message = $"Node '{id}' bulunamadi." });
            }

            var edges = _graph.GetEdges(id);
            var result = new List<EdgeDto>(edges.Length);

            foreach (var edge in edges)
            {
                result.Add(MapToEdgeDto(edge));
            }

            return Ok(result);
        }

        /// <summary>
        /// Toplu sekilde dugum ekler (Seed Data icin).
        /// POST /api/nodes/batch
        /// </summary>
        [HttpPost("batch")]
        public ActionResult AddNodesBatch([FromBody] List<NodeDto> nodes)
        {
            int addedCount = 0;
            foreach (var dto in nodes)
            {
                var node = new Node(dto.Id, dto.Type);
                foreach (var prop in dto.Properties)
                {
                    node.Properties.Put(prop.Key, prop.Value);
                }

                _graph.AddNode(node);
                addedCount++;

                // Arama yapilabilmesi icin isim/basligi Trie'a ekle
                if (dto.Properties.TryGetValue("Name", out var nameVal) && nameVal != null)
                {
                    _trie.Insert(nameVal.ToString()!, node.Id);
                }
                else if (dto.Properties.TryGetValue("Title", out var titleVal) && titleVal != null)
                {
                    _trie.Insert(titleVal.ToString()!, node.Id);
                }
            }

            return Ok(new { Message = $"{addedCount} dugum basariyla eklendi." });
        }

        /// <summary>
        /// Toplu sekilde kenar ekler (Seed Data icin).
        /// POST /api/edges/batch
        /// </summary>
        [HttpPost("~/api/edges/batch")]
        public ActionResult AddEdgesBatch([FromBody] List<EdgeDto> edges)
        {
            int addedCount = 0;
            foreach (var dto in edges)
            {
                var edge = new Edge(dto.Id, dto.SourceId, dto.TargetId, dto.RelationType, dto.IsDirected);
                foreach (var prop in dto.Properties)
                {
                    edge.Properties.Put(prop.Key, prop.Value);
                }

                _graph.AddEdge(edge);
                addedCount++;
            }

            return Ok(new { Message = $"{addedCount} kenar basariyla eklendi." });
        }

        /// <summary>
        /// Tum kenarlari listeler.
        /// GET /api/edges
        /// </summary>
        [HttpGet("~/api/edges")]
        public ActionResult<List<EdgeDto>> GetAllEdges()
        {
            var edges = _graph.GetAllEdges();
            var result = new List<EdgeDto>(edges.Length);

            foreach (var edge in edges)
            {
                result.Add(MapToEdgeDto(edge));
            }

            return Ok(result);
        }

        /// <summary>
        /// Belirli bir kenari siler.
        /// DELETE /api/edges/{sourceId}/{targetId}
        /// </summary>
        [HttpDelete("~/api/edges/{sourceId}/{targetId}")]
        public ActionResult DeleteEdge(string sourceId, string targetId)
        {
            bool removed = _graph.RemoveEdge(sourceId, targetId);
            if (!removed) return NotFound("Kenar bulunamadi veya silinemedi.");
            return Ok(new { Message = "Kenar basariyla silindi." });
        }

        /// <summary>
        /// Tum sistemi (Graf ve Trie) sifirlar. Benchmark icin kullanilir.
        /// DELETE /api/nodes/reset
        /// </summary>
        [HttpDelete("reset")]
        public ActionResult Reset()
        {
            _graph.Reset();
            _trie.Clear();
            
            // Belleği zorla temizle (Benchmark stabilitesi için)
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();

            return Ok(new { Message = "Tum sistem basariyla sifirlandi." });
        }

        /// <summary>
        /// Sistemi sifirlar ve baslangic (Seed) verilerini yukler.
        /// POST /api/nodes/seed
        /// </summary>
        [HttpPost("seed")]
        public ActionResult Seed()
        {
            // 1. Sifirla
            _graph.Reset();
            _trie.Clear();
            
            // Sabit seed ile deterministik üretim (Benchmark için önemli)
            var rnd = new System.Random(42);
            
            // 1. DÜĞÜMLERİ OLUŞTUR (Önce Düğümler, Sonra İlişkiler - Hata almamak için)
            
            // 1.1. Fotoğraflar (30 Adet)
            string[] photoTitles = {
                "Hackathon Hatirasi", "Ofiste Ilk Gun", "Yapay Zeka Zirvesi", "Kahve Molasi", "Yeni Proje Toplantisi",
                "Kodlama Gecesi", "Bahar Senligi", "Mezuniyet Toreni", "Takim Yemegi", "Haftasonu Kacamagi",
                "Doga Yuruyusu", "Konferans Sunumu", "Sertifika Toreni", "Design Thinking Workshop", "Ofis Manzarasi",
                "Evden Calisma Modu", "Yaz Kampi", "Kis Tatili", "Sabah Kosusu", "Kitap ve Kahve",
                "Konser Coskusu", "Gala Gecesi", "Kod Inceleme (Code Review)", "Proje Lansmani", "Musteri Ziyareti",
                "Agile Sprint Planning", "Server Odasi", "Yeni Ofis Masam", "Kedim ve Kod", "Gun Batimi"
            };

            for (int i = 1; i <= photoTitles.Length; i++)
            {
                var node = new Node($"photo{i}", "Photo");
                string title = photoTitles[i - 1];
                node.Properties.Put("Title", title);
                node.Properties.Put("Name", title);
                _graph.AddNode(node);
                _trie.Insert(title, node.Id);
            }

            // 1.2. Etkinlikler (20 Adet)
            string[] eventNames = {
                "Global AI Summit 2026", "Web Summit Europe", "React Developer Conf", "DotNet Days", "Cloud Native Meetup",
                "Data Science Bootcamp", "UX/UI Masterclass", "Startup Weekend", "Cyber Security Expo", "Blockchain Workshop",
                "Tech Career Fair", "Open Source Festival", "Women in Tech", "Game Developers Conference", "IoT Innovators",
                "Fintech Revolution", "Agile Leadership Summit", "Mobile App Developers Meetup", "Deep Learning Symposium", "Tech Makers Hackathon"
            };

            for (int i = 1; i <= eventNames.Length; i++)
            {
                var node = new Node($"event{i}", "Event");
                string name = eventNames[i - 1];
                node.Properties.Put("Name", name);
                _graph.AddNode(node);
                _trie.Insert(name, node.Id);
            }

            // 1.3. Kullanicilar (50 Kisi - En son ekle ki üstte görünsünler)
            string[] userNames = {
                "Batuhan", "Fatma Sude", "Muhammed Furkan", "Isra", "Ozcan",
                "Ahmet Yilmaz", "Ayse Demir", "Mehmet Kaya", "Fatma Celik", "Mustafa Sahin",
                "Zeynep Koc", "Ali Ozturk", "Elif Aydin", "Huseyin Ozdemir", "Merve Arslan",
                "Hasan Dogan", "Esra Kilic", "Ibrahim Cetin", "Busra Gurbuz", "Halil Gok",
                "Burcu Tekin", "Kemal Polat", "Selin Tarhan", "Caner Bulut", "Eda Yildirim",
                "Tolga Coban", "Cemre Yildiz", "Emre Karaca", "Derya Cakir", "Sinan Tas",
                "Gozde Akin", "Turgut Aslan", "Isil Erdogan", "Oguzhan Gunes", "Tugce Yavuz",
                "Gokhan Kaplan", "Muge Celik", "Serkan Yucel", "Deniz Ozer", "Umut Ekinci",
                "Aslihan Kara", "Koray Avci", "Zehra Basar", "Orhan Veli", "Nazli Cam",
                "Eren Yalcin", "Yasemin Kurt", "Volkan Turk", "Ceren Yilmaz", "Kerem Koca"
            };
            string[] professions = { "Software Engineer", "Data Scientist", "UI/UX Designer", "Product Manager", "DevOps Engineer", "Marketing Specialist", "Graphic Designer", "Project Manager", "Business Analyst", "CEO" };

            for (int i = 1; i <= userNames.Length; i++)
            {
                string fullName = userNames[i - 1];
                var node = new Node($"user{i}", "User");
                node.Properties.Put("Name", fullName);
                node.Properties.Put("Profession", professions[rnd.Next(professions.Length)]);
                _graph.AddNode(node);
                _trie.Insert(fullName, node.Id);
            }

            // 2. İLİŞKİLERİ OLUŞTUR (Düğümler artık var, güvenle eklenebilir)

            // 2.1. Fotoğraf sahipliği (POSTED)
            for (int i = 1; i <= 30; i++)
            {
                string ownerId = $"user{rnd.Next(1, 51)}";
                _graph.AddEdge(new Edge($"ep{i}", ownerId, $"photo{i}", "POSTED", true));
            }

            // 2.2. Rastgele Arkadaşlıklar (FRIEND)
            for (int i = 1; i <= 50; i++)
            {
                int friendCount = rnd.Next(2, 5);
                for (int j = 0; j < friendCount; j++)
                {
                    int targetIdx = rnd.Next(1, 51);
                    if (i != targetIdx)
                    {
                        string sourceId = $"user{i}";
                        string targetId = $"user{targetIdx}";
                        _graph.AddEdge(new Edge($"e_{sourceId}_{targetId}", sourceId, targetId, "FRIEND", false));
                        _graph.AddEdge(new Edge($"e_{targetId}_{sourceId}", targetId, sourceId, "FRIEND", false));
                    }
                }
            }

            for (int i = 1; i <= 50; i++)
            {
                // Like ve Attends
                _graph.AddEdge(new Edge($"l_{i}", $"user{i}", $"photo{rnd.Next(1, 31)}", "LIKES", true));
                _graph.AddEdge(new Edge($"a_{i}", $"user{i}", $"event{rnd.Next(1, 21)}", "ATTENDS", true));
            }

            return Ok(new { Message = "Sistem basariyla KALABALIK seed verileriyle başlatildi (180+ Node/Interaction)." });
        }

        private static NodeDto MapToDto(Node node)
        {
            var props = new Dictionary<string, object>();
            foreach (var kvp in node.Properties)
            {
                props[kvp.Key] = kvp.Value;
            }

            return new NodeDto
            {
                Id = node.Id,
                Type = node.Type,
                Properties = props
            };
        }

        private static EdgeDto MapToEdgeDto(Edge edge)
        {
            var props = new Dictionary<string, object>();
            foreach (var kvp in edge.Properties)
            {
                props[kvp.Key] = kvp.Value;
            }

            return new EdgeDto
            {
                Id = edge.Id,
                SourceId = edge.SourceId,
                TargetId = edge.DestinationId,
                RelationType = edge.RelationType,
                IsDirected = edge.IsDirected,
                Properties = props
            };
        }
    }
}
