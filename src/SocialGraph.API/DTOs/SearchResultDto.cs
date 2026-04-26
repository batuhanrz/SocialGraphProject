namespace SocialGraph.API.DTOs
{
    /// <summary>
    /// Autocomplete arama sonuclari icin veri nesnesi.
    /// Frontend tarafindaki ISearchResult ile uyumludur.
    /// </summary>
    public class SearchResultDto
    {
        public string NodeId { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
