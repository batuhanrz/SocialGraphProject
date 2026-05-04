using System;

namespace SocialGraph.API.Models
{
    public enum ActionType
    {
        NodeAdded,
        EdgeAdded,
        Unfriend,
        Unlike
    }

    public class SimulationAction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public ActionType Type { get; set; }
        public string SourceId { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
