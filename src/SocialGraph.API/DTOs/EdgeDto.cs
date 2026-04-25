namespace SocialGraph.API.DTOs
{
    /// <summary>
    /// Kenar (Edge) veri transfer nesnesi.
    /// Frontend IEdge interface'i ile birebir uyumludur.
    /// Not: Backend'deki DestinationId, frontend'de TargetId olarak adlandirilir.
    /// </summary>
    public class EdgeDto
    {
        public string Id { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string RelationType { get; set; } = string.Empty;
        public bool IsDirected { get; set; } = true;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
