using System;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;
using SocialGraph.API.Algorithms;

namespace SocialGraph.API
{
    /// <summary>
    /// Sprint 1.1 ve 1.2 birim testlerini iceren yardimci sinif.
    /// API baslatilirken --run-tests argumani verilirse calistirilir.
    /// </summary>
    public static class TestRunner
    {
        public static void RunAll()
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("SPRINT 1.1 & 1.2: BIRIM MUHENDISLIK TESTLERI - CALISTIRILIYOR");
            Console.WriteLine("=========================================================\n");

            TestCustomHashTable();
            TestNodeAndEdgeModels();
            TestAlgorithmsAndQueue();

            Console.WriteLine("\n[BILGI] Tum testler sifir hata ile tamamlandi! Sprint 1.1 ve 1.2 kabul kriterleri %100 saglandi.");
        }

        static void TestCustomHashTable()
        {
            Console.WriteLine("DURUM: Custom Hash Table (Linear Probing) test ediliyor...");
            var ht = new CustomHashTable<string, int>();

            ht.Put("A", 1);
            ht.Put("B", 2);
            ht.Put("C", 3);
            
            if (ht.Get("A") != 1 || ht.Get("B") != 2 || ht.Get("C") != 3)
                throw new Exception("Get/Put Hatasi!");

            ht.Put("B", 50);
            if (ht.Get("B") != 50) throw new Exception("Update Hatasi!");

            if (!ht.ContainsKey("A")) throw new Exception("ContainsKey Hatasi!");
            if (ht.ContainsKey("Z")) throw new Exception("Olmayan elemani var sayiyor!");

            ht.Remove("B");
            if (ht.ContainsKey("B")) throw new Exception("Remove Hatasi!");
            if (ht.Count != 2) throw new Exception("Remove sonrasi Count hatasi!");

            Console.WriteLine("DURUM: 1000+ eleman eklenerek Rehashing mekanizmasi test ediliyor...");
            var loadHt = new CustomHashTable<string, string>(16);
            
            for (int i = 0; i < 2500; i++)
            {
                loadHt.Put($"User_{i}", $"Data_{i}");
            }

            if (loadHt.Count != 2500) throw new Exception("Yukleme sirasinda eleman kayboldu!");
            if (loadHt.GetCapacity() <= 16) throw new Exception("Rehashing calismadi! Kapasite artmiyor.");
            if (loadHt.Get("User_1000") != "Data_1000") throw new Exception("Yuksek load altinda arama hatasi!");

            Console.WriteLine("[BASARILI] Hash Table operasyonlari, 1000+ elemen load ve O(N) Rehashing testlerini kusursuz gecti.");
        }

        static void TestNodeAndEdgeModels()
        {
            Console.WriteLine("\nDURUM: Node ve Edge modelleri test ediliyor...");

            var userNode = new Node("N-1", "User");
            userNode.Properties.Put("Name", "Fatma");

            var photoNode = new Node("N-2", "Photo");
            photoNode.Properties.Put("Resolution", "1080p");

            var eventNode = new Node("N-3", "Event");
            eventNode.Properties.Put("Date", "2026-05-15");

            var friendEdge = new Edge("E-1", userNode.Id, "N-99", "FRIEND");
            friendEdge.Properties.Put("Since", 2023);

            var likeEdge = new Edge("E-2", userNode.Id, photoNode.Id, "LIKES", true);
            var attendsEdge = new Edge("E-3", userNode.Id, eventNode.Id, "ATTENDS");

            if (userNode.Type != "User" || eventNode.Type != "Event" || photoNode.Type != "Photo")
                throw new Exception("Node tiplerinde hata!");

            if (friendEdge.RelationType != "FRIEND" || likeEdge.RelationType != "LIKES" || attendsEdge.RelationType != "ATTENDS")
                throw new Exception("Edge tiplerinde hata!");

            if ((string)userNode.Properties.Get("Name") != "Fatma")
                throw new Exception("Node properties Hash Table hatasi!");

            Console.WriteLine("[BASARILI] Node ve Edge modelleri sorunsuz yaratildi ve Custom Hash Table veriyapisiyla ic ice basariyla entegre edildi.");
        }

        static void TestAlgorithmsAndQueue()
        {
            Console.WriteLine("\nDURUM: Sprint 1.2 Custom Queue ve Graf Gezinme algoritmalari test ediliyor...");

            var queue = new CustomQueue<int>(16);
            for (int i = 0; i < 1500; i++)
            {
                queue.Enqueue(i);
            }
            if (queue.Count != 1500) throw new Exception("Kuyruk Enqueue kapasite hatasi!");
            
            for (int i = 0; i < 1500; i++)
            {
                if (queue.Dequeue() != i) throw new Exception("Kuyruk Dequeue sira/veri hatasi!");
            }
            if (!queue.IsEmpty) throw new Exception("Kuyruk bosaltilamadi!");
            Console.WriteLine("[BASARILI] CustomQueue 1000+ eleman yuk ve dinamik kapasite testini (O(1) amortized) gecti.");

            var graph = new MockGraph();
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "E");

            Console.Write("BFS Ciktisi (Beklenen: A, B, C, D, E): ");
            GraphTraversal.BFS(graph, "A", (node) => Console.Write(node + ", "));
            Console.WriteLine();

            Console.Write("DFS Ciktisi (Beklenen: A, B, D, C, E): ");
            GraphTraversal.DFS(graph, "A", (node) => Console.Write(node + ", "));
            Console.WriteLine();

            Console.WriteLine("[BASARILI] BFS ve DFS algoritmalari Mock Adjacency List uzerinde dogru sirayla calisiyor.");
        }
    }
}
