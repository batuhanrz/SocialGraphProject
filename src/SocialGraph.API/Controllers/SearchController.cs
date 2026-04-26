using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DTOs;
using SocialGraph.API.DataStructures;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Metin tabanli arama islemleri icin API endpoint'leri.
    /// CustomTrie kullanilarak PropertyGraph uzerinde arama yapilir.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly CustomTrie _trie;
        private readonly PropertyGraph _graph;

        public SearchController(CustomTrie trie, PropertyGraph graph)
        {
            _trie = trie;
            _graph = graph;
        }

        /// <summary>
        /// Trie uzerinde prefix (autocomplete) aramasi yapar.
        /// GET /api/search/autocomplete?query=...
        /// </summary>
        [HttpGet("autocomplete")]
        public ActionResult<List<SearchResultDto>> AutoComplete([FromQuery] string query = "")
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<SearchResultDto>());

            // Trie uzerinden autocomplete sonuclarini (ID dizisi) al
            var nodeIds = _trie.AutoComplete(query);
            
            var results = new List<SearchResultDto>();
            
            foreach (var id in nodeIds)
            {
                var node = _graph.GetNode(id);
                if (node != null)
                {
                    // Label icin 'Name' veya 'Title' ozelligini kullan, yoksa ID dondur
                    string label = id;
                    if (node.Properties.TryGetValue("Name", out var nameVal) && nameVal != null)
                        label = nameVal.ToString()!;
                    else if (node.Properties.TryGetValue("Title", out var titleVal) && titleVal != null)
                        label = titleVal.ToString()!;

                    results.Add(new SearchResultDto
                    {
                        NodeId = node.Id,
                        Type = node.Type,
                        Label = label
                    });
                }
            }

            return Ok(results);
        }
    }
}
