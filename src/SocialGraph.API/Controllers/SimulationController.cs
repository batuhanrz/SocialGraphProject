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
        private static ConcurrentQueue<SimulationAction> _actions = new ConcurrentQueue<SimulationAction>();
        private const int MaxActions = 50;

        // Simulasyon duraklatma bayragi
        public static bool IsPaused { get; set; } = false;

        [HttpGet("actions")]
        public ActionResult<IEnumerable<SimulationAction>> GetRecentActions()
        {
            return Ok(_actions.OrderByDescending(a => a.Timestamp).ToList());
        }

        [HttpPost("actions")]
        public ActionResult AddAction([FromBody] SimulationAction action)
        {
            if (IsPaused) return Ok(); // Duraklatilmissa aksiyon kaydetme

            _actions.Enqueue(action);
            
            // Liste cok buyurse en eskiyi cikar
            while (_actions.Count > MaxActions)
            {
                _actions.TryDequeue(out _);
            }

            return Ok();
        }

        [HttpGet("status")]
        public ActionResult GetStatus()
        {
            return Ok(new { IsPaused });
        }

        [HttpPost("pause")]
        public ActionResult Pause()
        {
            IsPaused = true;
            return Ok(new { Message = "Simulasyon duraklatildi." });
        }

        [HttpPost("resume")]
        public ActionResult Resume()
        {
            IsPaused = false;
            return Ok(new { Message = "Simulasyon devam ettiriliyor." });
        }

        [HttpDelete("reset")]
        public ActionResult Reset()
        {
            _actions = new ConcurrentQueue<SimulationAction>();
            return Ok(new { Message = "Aksiyon akisi temizlendi." });
        }
    }
}
