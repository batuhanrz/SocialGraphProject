namespace SocialGraph.API.DTOs
{
    /// <summary>
    /// Metin tabanli arama istegi icin veri transfer nesnesi.
    /// </summary>
    public class SearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public int MaxResults { get; set; } = 10;
    }
}
