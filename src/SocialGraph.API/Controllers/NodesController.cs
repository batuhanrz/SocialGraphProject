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

        public NodesController(PropertyGraph graph)
        {
            _graph = graph;
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
