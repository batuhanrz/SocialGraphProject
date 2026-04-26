namespace SocialGraph.API.DataStructures
{
    /// <summary>
    /// Trie (Onek Agaci) dugum sinifi.
    /// Cocuk dugumler icin projenin kendi CustomHashTable yapisi kullanilir.
    /// Karmasiklik: Her dugumde cocuk erisimi O(1) (Hash Table)
    /// </summary>
    public class TrieNode
    {
        public char Character { get; set; }

        // Standart Dictionary yerine kendi CustomHashTable yapilarimiz kullanilmistir
        public CustomHashTable<char, TrieNode> Children { get; set; }

        public bool IsEndOfWord { get; set; }

        public TrieNode(char character)
        {
            Character = character;
            Children = new CustomHashTable<char, TrieNode>();
            IsEndOfWord = false;
        }
    }
}
