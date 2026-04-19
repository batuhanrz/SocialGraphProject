using SocialGraph.API.DataStructures;

namespace SocialGraph.API.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string Type { get; set; }
        
        // Kendi CustomHashTable yapimiz kullanilmistir
        public CustomHashTable<string, object> Properties { get; set; }

        public Node(string id, string type)
        {
            Id = id;
            Type = type;
            Properties = new CustomHashTable<string, object>();
        }
    }
}
