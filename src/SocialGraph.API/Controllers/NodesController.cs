using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DTOs;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Dugum (Node) islemleri icin API endpoint'leri.
    /// Sprint 2'de gercek veri entegrasyonu yapilacaktir.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly CustomHashTable<string, Node> _nodeStore;

        public NodesController(CustomHashTable<string, Node> nodeStore)
        {
            _nodeStore = nodeStore;
        }

        /// <summary>
        /// Tum dugumleri listeler.
        /// GET /api/nodes
        /// </summary>
        [HttpGet]
        public ActionResult<List<NodeDto>> GetAll()
        {
            var result = new List<NodeDto>();

            foreach (var kvp in _nodeStore)
            {
                result.Add(MapToDto(kvp.Value));
            }

            // Eger store bos ise ornek veri don (placeholder)
            if (result.Count == 0)
            {
                result.Add(new NodeDto { Id = "placeholder-1", Type = "User", Properties = new Dictionary<string, object> { { "Name", "Ornek Kullanici" } } });
                result.Add(new NodeDto { Id = "placeholder-2", Type = "Event", Properties = new Dictionary<string, object> { { "Title", "Ornek Etkinlik" } } });
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
            if (_nodeStore.ContainsKey(id))
            {
                var node = _nodeStore.Get(id);
                return Ok(MapToDto(node));
            }

            return NotFound(new { Message = $"Node '{id}' bulunamadi." });
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
    }
}
