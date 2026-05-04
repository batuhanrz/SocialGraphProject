using Microsoft.AspNetCore.Mvc;
using SocialGraph.API.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SocialGraph.API.Controllers
{
    [ApiController]
    [Route("api/simulation")]
    public class SimulationController : ControllerBase
    {
        // Bellekte son aksiyonlari tutan thread-safe liste
        private static readonly ConcurrentQueue<SimulationAction> _actions = new ConcurrentQueue<SimulationAction>();
        private const int MaxActions = 50;

        [HttpGet("actions")]
        public ActionResult<IEnumerable<SimulationAction>> GetRecentActions()
        {
            return Ok(_actions.OrderByDescending(a => a.Timestamp).ToList());
        }

        [HttpPost("actions")]
        public ActionResult AddAction([FromBody] SimulationAction action)
        {
            _actions.Enqueue(action);
            
            // Liste cok buyurse en eskiyi cikar
            while (_actions.Count > MaxActions)
            {
                _actions.TryDequeue(out _);
            }

            return Ok();
        }
    }
}
