using Xunit;
using SocialGraph.API.DataStructures;

namespace SocialGraph.Tests
{
    /// <summary>
    /// CustomHashTable (Linear Probing) birim testleri.
    /// Batuhan tarafindan Sprint 1.1'de yazilan veri yapisinin dogrulanmasi.
    /// </summary>
    public class CustomHashTableTests
    {
        [Fact]
        public void Put_And_Get_ShouldStoreAndRetrieveValues()
        {
            var ht = new CustomHashTable<string, int>();

            ht.Put("A", 1);
            ht.Put("B", 2);
            ht.Put("C", 3);

            Assert.Equal(1, ht.Get("A"));
            Assert.Equal(2, ht.Get("B"));
            Assert.Equal(3, ht.Get("C"));
            Assert.Equal(3, ht.Count);
        }

        [Fact]
        public void Put_SameKey_ShouldUpdateValue()
        {
            var ht = new CustomHashTable<string, int>();

            ht.Put("X", 10);
            ht.Put("X", 99);

            Assert.Equal(99, ht.Get("X"));
            Assert.Equal(1, ht.Count);
        }

        [Fact]
        public void Remove_ShouldDeleteKeyAndDecrementCount()
        {
            var ht = new CustomHashTable<string, string>();

            ht.Put("key1", "val1");
            ht.Put("key2", "val2");

            bool removed = ht.Remove("key1");

            Assert.True(removed);
            Assert.False(ht.ContainsKey("key1"));
            Assert.Equal(1, ht.Count);
        }

        [Fact]
        public void Rehashing_ShouldHandleOver1000Elements()
        {
            var ht = new CustomHashTable<string, string>(16);

            for (int i = 0; i < 1500; i++)
            {
                ht.Put($"User_{i}", $"Data_{i}");
            }

            Assert.Equal(1500, ht.Count);
            Assert.True(ht.GetCapacity() > 16);
            Assert.Equal("Data_500", ht.Get("User_500"));
            Assert.Equal("Data_1499", ht.Get("User_1499"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var ht = new CustomHashTable<string, string>();
            Assert.Throws<KeyNotFoundException>(() => ht.Get("GhostKey"));
        }

        [Fact]
        public void Collision_ShouldBeHandledByLinearProbing()
        {
            // Ayni hashe sahip key'ler uretmek zordur, ama kapasiteyi cok kucuk tutup 
            // rehashing oncesi bolca eleman eklersek probing test edilmis olur.
            var ht = new CustomHashTable<int, string>(4);
            ht.Put(1, "A");
            ht.Put(5, "B"); // Muhtemel collision (1 % 4 == 1, 5 % 4 == 1)
            ht.Put(9, "C"); // Muhtemel collision (9 % 4 == 1)

            Assert.Equal("A", ht.Get(1));
            Assert.Equal("B", ht.Get(5));
            Assert.Equal("C", ht.Get(9));
        }
    }
}
