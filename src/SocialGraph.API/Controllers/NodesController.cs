using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DTOs;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Dugum (Node) ve Kenar (Edge) islemleri icin API endpoint'leri.
    /// PropertyGraph kullanilarak gercek verilere erisilir.
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
