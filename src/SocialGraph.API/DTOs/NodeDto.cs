namespace SocialGraph.API.DTOs
{
    /// <summary>
    /// Dugum (Node) veri transfer nesnesi.
    /// Frontend INode interface'i ile birebir uyumludur.
    /// </summary>
    public class NodeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
