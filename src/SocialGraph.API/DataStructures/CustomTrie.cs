using System;
using System.Collections.Generic;

namespace SocialGraph.API.DataStructures
{
    /// <summary>
    /// Sifirdan implemente edilmis Trie (Onek Agaci) veri yapisi.
    /// Metin tabanli arama ve otomatik tamamlama icin kullanilir.
    /// Cocuk dugum yonetiminde projenin kendi CustomHashTable yapisi kullanilmistir.
    /// Case-insensitive calisir.
    /// </summary>
    public class CustomTrie
    {
        private readonly TrieNode _root;
        private int _wordCount;

        /// <summary>
        /// Trie'deki toplam kelime sayisi.
        /// Karmasiklik: O(1)
        /// </summary>
        public int Count => _wordCount;

        public CustomTrie()
        {
            _root = new TrieNode('\0'); // Kok dugum
            _wordCount = 0;
        }

        /// <summary>
        /// Kelimeyi Trie'ye ekler. Buyuk/kucuk harf duyarsizdir.
        /// Karmasiklik: O(m) — m: kelime uzunlugu
        /// </summary>
        public void Insert(string word, string nodeId = null)
        {
            if (string.IsNullOrEmpty(word)) return;

            string normalized = word.ToLowerInvariant();
            TrieNode current = _root;

            for (int i = 0; i < normalized.Length; i++)
            {
                char ch = normalized[i];

                if (!current.Children.ContainsKey(ch))
                {
                    current.Children.Put(ch, new TrieNode(ch));
                }

                current = current.Children.Get(ch);
            }

            if (!current.IsEndOfWord)
            {
                current.IsEndOfWord = true;
                _wordCount++;
            }

            // Testlerin geriye donuk calismasi icin (Eger nodeId verilmemisse, kelimenin kendisini ID yap)
            if (string.IsNullOrEmpty(nodeId))
            {
                nodeId = word;
            }

            // Dugum ID'sini bu kelimeye bagla
            if (!current.NodeIds.ContainsKey(nodeId))
            {
                current.NodeIds.Put(nodeId, true);
            }
        }

        /// <summary>
        /// Kelimenin Trie'de tam olarak var olup olmadigini kontrol eder.
        /// Karmasiklik: O(m)
        /// </summary>
        public bool Search(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            TrieNode node = FindNode(word.ToLowerInvariant());
            return node != null && node.IsEndOfWord;
        }

        /// <summary>
        /// Belirtilen onek ile baslayan en az bir kelime olup olmadigini kontrol eder.
        /// Karmasiklik: O(m) — m: onek uzunlugu
        /// </summary>
        public bool StartsWith(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return false;

            return FindNode(prefix.ToLowerInvariant()) != null;
        }

        /// <summary>
        /// Belirtilen onek ile baslayan kelimeleri listeler.
        /// Karmasiklik: O(m + k) — m: onek uzunlugu, k: sonuc sayisi
        /// </summary>
        public string[] AutoComplete(string prefix, int maxResults = 10)
        {
            if (string.IsNullOrEmpty(prefix) || maxResults <= 0)
                return Array.Empty<string>();

            string normalizedPrefix = prefix.ToLowerInvariant();
            TrieNode prefixNode = FindNode(normalizedPrefix);

            if (prefixNode == null)
                return Array.Empty<string>();

            var results = new List<string>();
            CollectWords(prefixNode, normalizedPrefix, results, maxResults);

            return results.ToArray();
        }

        /// <summary>
        /// Verilen onek icin Trie'deki son dugumu bulur.
        /// Karmasiklik: O(m)
        /// </summary>
        private TrieNode? FindNode(string normalizedWord)
        {
            TrieNode current = _root;

            for (int i = 0; i < normalizedWord.Length; i++)
            {
                char ch = normalizedWord[i];

                if (!current.Children.ContainsKey(ch))
                {
                    return null;
                }

                current = current.Children.Get(ch);
            }

            return current;
        }

        /// <summary>
        /// Belirli bir dugumden baslayarak tum kelimeleri toplar (DFS).
        /// </summary>
        private void CollectWords(TrieNode node, string currentWord, List<string> results, int maxResults)
        {
            if (results.Count >= maxResults) return;

            if (node.IsEndOfWord)
            {
                // Artik kelimeleri degil, kelimenin isaret ettigi dugum ID'lerini donduruyoruz.
                foreach (var kvp in node.NodeIds)
                {
                    if (results.Count >= maxResults) break;
                    if (!results.Contains(kvp.Key))
                    {
                        results.Add(kvp.Key);
                    }
                }
            }

            foreach (var kvp in node.Children)
            {
                if (results.Count >= maxResults) return;
                CollectWords(kvp.Value, currentWord + kvp.Key, results, maxResults);
            }
        }

        /// <summary>
        /// Trie'deki tum verileri temizler.
        /// </summary>
        public void Clear()
        {
            _root.Children.Clear();
            _root.NodeIds.Clear();
            _wordCount = 0;
        }
    }
}
