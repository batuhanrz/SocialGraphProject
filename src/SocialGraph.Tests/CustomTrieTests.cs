using Xunit;
using SocialGraph.API.DataStructures;

namespace SocialGraph.Tests
{
    /// <summary>
    /// CustomTrie (Onek Agaci) birim testleri.
    /// Isra tarafindan Sprint 1.5'te yazilan veri yapisinin dogrulanmasi.
    /// </summary>
    public class CustomTrieTests
    {
        [Fact]
        public void Insert_And_Search_ShouldFindExactWord()
        {
            var trie = new CustomTrie();

            trie.Insert("hello");
            trie.Insert("world");

            Assert.True(trie.Search("hello"));
            Assert.True(trie.Search("world"));
            Assert.Equal(2, trie.Count);
        }

        [Fact]
        public void Search_NonExistentWord_ShouldReturnFalse()
        {
            var trie = new CustomTrie();

            trie.Insert("apple");

            Assert.False(trie.Search("app"));
            Assert.False(trie.Search("banana"));
            Assert.False(trie.Search(""));
        }

        [Fact]
        public void StartsWith_ShouldDetectValidPrefix()
        {
            var trie = new CustomTrie();

            trie.Insert("algorithm");
            trie.Insert("alpha");
            trie.Insert("beta");

            Assert.True(trie.StartsWith("al"));
            Assert.True(trie.StartsWith("alg"));
            Assert.True(trie.StartsWith("bet"));
            Assert.False(trie.StartsWith("gamma"));
        }

        [Fact]
        public void CaseInsensitive_ShouldMatchRegardlessOfCase()
        {
            var trie = new CustomTrie();

            trie.Insert("Batuhan");
            trie.Insert("OZCAN");

            Assert.True(trie.Search("batuhan"));
            Assert.True(trie.Search("BATUHAN"));
            Assert.True(trie.Search("ozcan"));
            Assert.True(trie.Search("Ozcan"));
            Assert.Equal(2, trie.Count);
        }

        [Fact]
        public void AutoComplete_ShouldReturnMatchingWords()
        {
            var trie = new CustomTrie();

            trie.Insert("graph");
            trie.Insert("gravity");
            trie.Insert("green");
            trie.Insert("grid");
            trie.Insert("blue");

            string[] results = trie.AutoComplete("gr", 10);

            Assert.True(results.Length >= 3);
            Assert.Contains("graph", results);
            Assert.Contains("gravity", results);
            Assert.Contains("green", results);
            Assert.DoesNotContain("blue", results);
        }

        [Fact]
        public void AutoComplete_With100PlusWords_ShouldReturnCorrectResults()
        {
            var trie = new CustomTrie();

            // 120 kelime ekle: user_0, user_1, ..., user_119
            for (int i = 0; i < 120; i++)
            {
                trie.Insert($"user_{i}");
            }

            // Farkli oneklerle kelimeler ekle
            trie.Insert("admin_panel");
            trie.Insert("admin_dashboard");

            Assert.Equal(122, trie.Count);

            // "user_1" oneki ile baslayan kelimeleri getir (user_1, user_10, user_11, ..., user_119)
            string[] userResults = trie.AutoComplete("user_1", 50);
            Assert.True(userResults.Length >= 2);

            // "admin" oneki ile baslayan kelimeleri getir
            string[] adminResults = trie.AutoComplete("admin", 10);
            Assert.Equal(2, adminResults.Length);

            // Olmayan onek
            string[] emptyResults = trie.AutoComplete("xyz", 10);
            Assert.Empty(emptyResults);
        }

        [Fact]
        public void DuplicateInsert_ShouldNotIncrementCount()
        {
            var trie = new CustomTrie();

            trie.Insert("test");
            trie.Insert("test");
            trie.Insert("TEST");

            Assert.Equal(1, trie.Count);
        }
    }
}
