using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DTOs;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Graf traversal (BFS/DFS) islemleri icin API endpoint'leri.
    /// Sprint 2'de gercek graf entegrasyonu yapilacaktir.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TraversalController : ControllerBase
    {
        /// <summary>
        /// BFS (Genislik Oncelikli Arama) calistirir.
        /// POST /api/traversal/bfs
        /// </summary>
        [HttpPost("bfs")]
        public ActionResult<TraversalResultDto> RunBfs([FromBody] TraversalRequestDto request)
        {
            // Placeholder: Sprint 2'de gercek Property Graph uzerinde calisacak
            var result = new TraversalResultDto
            {
                StartNodeId = request.StartNodeId,
                Algorithm = "BFS",
                VisitedNodeIds = new[] { request.StartNodeId, "placeholder-node-1", "placeholder-node-2" }
            };

            return Ok(result);
        }

        /// <summary>
        /// DFS (Derinlik Oncelikli Arama) calistirir.
        /// POST /api/traversal/dfs
        /// </summary>
        [HttpPost("dfs")]
        public ActionResult<TraversalResultDto> RunDfs([FromBody] TraversalRequestDto request)
        {
            // Placeholder: Sprint 2'de gercek Property Graph uzerinde calisacak
            var result = new TraversalResultDto
            {
                StartNodeId = request.StartNodeId,
                Algorithm = "DFS",
                VisitedNodeIds = new[] { request.StartNodeId, "placeholder-node-3", "placeholder-node-4" }
            };

            return Ok(result);
        }
    }

    /// <summary>
    /// Traversal istegi icin request modeli.
    /// </summary>
    public class TraversalRequestDto
    {
        public string StartNodeId { get; set; } = string.Empty;
    }
}
