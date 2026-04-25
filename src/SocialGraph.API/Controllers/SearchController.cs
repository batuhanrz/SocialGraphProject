using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.DTOs;

namespace SocialGraph.API.Controllers
{
    /// <summary>
    /// Metin tabanli arama islemleri icin API endpoint'leri.
    /// Sprint 2'de Trie entegrasyonu yapilacaktir.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        /// <summary>
        /// Metin tabanli dugum arama.
        /// GET /api/search?query=Fatma&maxResults=10
        /// </summary>
        [HttpGet]
        public ActionResult<List<NodeDto>> Search([FromQuery] string query = "", [FromQuery] int maxResults = 10)
        {
            // Placeholder: Sprint 2'de Trie + Hash Table entegrasyonu yapilacak
            var results = new List<NodeDto>
            {
                new NodeDto
                {
                    Id = "search-result-1",
                    Type = "User",
                    Properties = new Dictionary<string, object>
                    {
                        { "Name", $"Arama sonucu: '{query}'" },
                        { "Info", "Bu bir placeholder sonuctur. Sprint 2'de gercek arama aktif olacaktir." }
                    }
                }
            };

            return Ok(results);
        }
    }
}
