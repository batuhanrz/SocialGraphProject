namespace SocialGraph.API.DTOs
{
    /// <summary>
    /// BFS/DFS traversal sonuclarini tasiyan veri transfer nesnesi.
    /// </summary>
    public class TraversalResultDto
    {
        public string StartNodeId { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty; // "BFS" veya "DFS"
        public string[] VisitedNodeIds { get; set; } = Array.Empty<string>();
    }
}
