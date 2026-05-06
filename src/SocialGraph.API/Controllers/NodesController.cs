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

            var rnd = new System.Random();

            // 2. Kullanici Listesi (50 Kisiye sabitlendi - Worker ile ayni)
            string[] baseNames = {
                "Batuhan", "Fatma", "Muhammed", "Isra", "Ozcan", "Ahmet", "Ayse", "Mehmet", "Zeynep", "Ali",
                "Elif", "Huseyin", "Merve", "Hasan", "Esra", "Ibrahim", "Busra", "Halil", "Burcu", "Kemal"
            };
            string[] surnames = { "Yilmaz", "Kaya", "Celik", "Demir", "Sahin", "Koc", "Ozturk", "Aydin", "Ozdemir", "Arslan" };
            string[] professions = { "Software Engineer", "Data Scientist", "UI/UX Designer", "Product Manager", "DevOps" };

            // 50 Kullanici Olustur
            for (int i = 1; i <= 50; i++)
            {
                string fullName = $"{baseNames[rnd.Next(baseNames.Length)]} {surnames[rnd.Next(surnames.Length)]}";
                if (i <= 5) {
                   string[] teamNames = { "Batuhan Yilmaz", "Fatma Sude Kaya", "Muhammed Furkan Celik", "Isra Nur Demir", "Ozcan Sahin" };
                   fullName = teamNames[i-1];
                }

                var node = new Node($"user{i}", "User");
                node.Properties.Put("Name", fullName);
                node.Properties.Put("Profession", professions[rnd.Next(professions.Length)]);
                _graph.AddNode(node);
                _trie.Insert(fullName, node.Id);
            }

            // 3. Fotoğraflar (30 Adet)
            for (int i = 1; i <= 30; i++)
            {
                var node = new Node($"photo{i}", "Photo");
                node.Properties.Put("Title", $"Shared Moment {i}");
                _graph.AddNode(node);
                _trie.Insert($"Shared Moment {i}", node.Id);
                
                string ownerId = $"user{rnd.Next(1, 51)}";
                _graph.AddEdge(new Edge($"ep{i}", ownerId, node.Id, "POSTED", true));
            }

            // 4. Etkinlikler (20 Adet)
            string[] eventTypes = { "Summit", "Meetup", "Workshop", "Conference", "Hackathon" };
            for (int i = 1; i <= 20; i++)
            {
                var node = new Node($"event{i}", "Event");
                string eventName = $"{eventTypes[rnd.Next(eventTypes.Length)]} {i}";
                node.Properties.Put("Name", eventName);
                _graph.AddNode(node);
                _trie.Insert(eventName, node.Id);
            }

            // 5. İlişkiler
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
