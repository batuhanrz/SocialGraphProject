namespace SocialGraph.API.DTOs
{
    public class RecommendationDto
    {
        public NodeDto Node { get; set; } = new();
        public int MutualFriendsCount { get; set; }
    }
}
