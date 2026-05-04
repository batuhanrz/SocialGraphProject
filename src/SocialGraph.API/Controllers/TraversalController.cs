using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Algorithms;
using SocialGraph.API.DTOs;
using SocialGraph.API.Models;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Graf traversal (BFS/DFS/ShortestPath) ve iliski tabanli sorgu islemleri icin API endpoint'leri.
    /// Gelistiren: Ozcan (Algorithm Master)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TraversalController : ControllerBase
    {
        private readonly PropertyGraph _graph;
        private readonly RelationalQueryEngine _queryEngine;

        public TraversalController(PropertyGraph graph)
        {
            _graph = graph;
            _queryEngine = new RelationalQueryEngine(graph);
        }

        /// <summary>
        /// BFS (Genislik Oncelikli Arama) calistirir.
        /// GET /api/traversal/bfs?startNodeId=...
        /// </summary>
        [HttpGet("bfs")]
        public ActionResult<IEnumerable<string>> RunBfs([FromQuery] string startNodeId)
        {
            if (string.IsNullOrWhiteSpace(startNodeId))
                return BadRequest("startNodeId gereklidir.");

            if (_graph.GetNode(startNodeId) == null)
                return NotFound($"Dugum bulunamadi: {startNodeId}");

            var visitedIds = new List<string>();
            GraphTraversal.BFS(_graph, startNodeId, node => visitedIds.Add(node.Id));

            return Ok(visitedIds);
        }

        /// <summary>
        /// DFS (Derinlik Oncelikli Arama) calistirir.
        /// GET /api/traversal/dfs?startNodeId=...
        /// </summary>
        [HttpGet("dfs")]
        public ActionResult<IEnumerable<string>> RunDfs([FromQuery] string startNodeId)
        {
            if (string.IsNullOrWhiteSpace(startNodeId))
                return BadRequest("startNodeId gereklidir.");

            if (_graph.GetNode(startNodeId) == null)
                return NotFound($"Dugum bulunamadi: {startNodeId}");

            var visitedIds = new List<string>();
            GraphTraversal.DFS(_graph, startNodeId, node => visitedIds.Add(node.Id));

            return Ok(visitedIds);
        }

        /// <summary>
        /// Iki dugum arasi yolu bulur. BFS ile en kisa yol, DFS ile herhangi bir yol bulunur.
        /// GET /api/traversal/shortestpath?startNodeId=...&targetNodeId=...&algorithm=BFS
        /// </summary>
        [HttpGet("shortestpath")]
        public ActionResult<IEnumerable<string>> RunShortestPath([FromQuery] string startNodeId, [FromQuery] string targetNodeId, [FromQuery] string algorithm = "BFS")
        {
            if (string.IsNullOrWhiteSpace(startNodeId) || string.IsNullOrWhiteSpace(targetNodeId))
                return BadRequest("startNodeId ve targetNodeId gereklidir.");

            if (_graph.GetNode(startNodeId) == null || _graph.GetNode(targetNodeId) == null)
                return NotFound("Kaynak veya hedef dugum bulunamadi.");

            string[] path;
            if (string.Equals(algorithm, "DFS", StringComparison.OrdinalIgnoreCase))
            {
                path = GraphTraversal.DFS_Path(_graph, startNodeId, targetNodeId);
            }
            else
            {
                path = GraphTraversal.ShortestPath(_graph, startNodeId, targetNodeId);
            }
            return Ok(path);
        }

        /// <summary>
        /// Cok adimli iliskisel zincir sorgusu calistirir.
        /// GET /api/traversal/chain?startNodeId=...&relations=FRIEND&relations=ATTENDS
        /// </summary>
        [HttpGet("chain")]
        public ActionResult<ChainResponseDto> RunChainQuery([FromQuery] string startNodeId, [FromQuery] string[] relations)
        {
            if (string.IsNullOrWhiteSpace(startNodeId) || relations == null || relations.Length == 0)
                return BadRequest("startNodeId ve en az bir iliski turu gereklidir.");

            var chainResult = _queryEngine.ExecuteChainQuery(startNodeId, relations);
            
            var response = new ChainResponseDto
            {
                Nodes = new List<NodeDto>(),
                Steps = new List<ChainStepDto>()
            };

            foreach (var node in chainResult.AllNodes)
            {
                response.Nodes.Add(MapToDto(node));
            }

            foreach (var step in chainResult.Steps)
            {
                response.Steps.Add(new ChainStepDto { Relation = step.Relation, Count = step.Count });
            }

            return Ok(response);
        }

        /// <summary>
        /// Belirli bir kullanici icin arkadas onerileri sunar (Ortak arkadas sayisina gore).
        /// GET /api/traversal/recommendations?userId=...
        /// </summary>
        [HttpGet("recommendations")]
        public ActionResult<List<RecommendationDto>> GetRecommendations([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId gereklidir.");

            var recommendations = _queryEngine.GetRecommendations(userId);
            var result = new List<RecommendationDto>(recommendations.Length);

            foreach (var rec in recommendations)
            {
                result.Add(new RecommendationDto
                {
                    Node = MapToDto(rec.RecommendedNode),
                    MutualFriendsCount = rec.Score
                });
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
    }
}
