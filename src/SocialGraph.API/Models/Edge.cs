using SocialGraph.API.DataStructures;

namespace SocialGraph.API.Models
{
    public class Edge
    {
        public string Id { get; set; }
        public string SourceId { get; set; }
        public string DestinationId { get; set; }
        public string RelationType { get; set; }
        public bool IsDirected { get; set; }
        
        // Kendi CustomHashTable yapimiz kullanilmistir
        public CustomHashTable<string, object> Properties { get; set; }

        public Edge(string id, string sourceId, string destinationId, string relationType, bool isDirected = true)
        {
            Id = id;
            SourceId = sourceId;
            DestinationId = destinationId;
            RelationType = relationType;
            IsDirected = isDirected;
            Properties = new CustomHashTable<string, object>();
        }
    }
}
