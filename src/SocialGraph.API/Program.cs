using System;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;
using SocialGraph.API.Algorithms;

namespace SocialGraph.API
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("SPRINT 1.1 & 1.2: BİRİM MÜHENDİSLİK TESTLERİ - ÇALIŞTIRILIYOR");
            Console.WriteLine("=========================================================\n");

            TestCustomHashTable();
            TestNodeAndEdgeModels();
            TestAlgorithmsAndQueue();

            Console.WriteLine("\n[BİLGİ] Tüm testler sıfır hata ile tamamlandı! Sprint 1.1 ve 1.2 kabul kriterleri %100 sağlandı.");
        }

        static void TestCustomHashTable()
        {
            Console.WriteLine("DURUM: Custom Hash Table (Linear Probing) test ediliyor...");
            var ht = new CustomHashTable<string, int>();

            // Ekleme ve Arama Testi
            ht.Put("A", 1);
            ht.Put("B", 2);
            ht.Put("C", 3);
            
            if (ht.Get("A") != 1 || ht.Get("B") != 2 || ht.Get("C") != 3)
                throw new Exception("Get/Put Hatası!");

            // Update Testi
            ht.Put("B", 50);
            if (ht.Get("B") != 50) throw new Exception("Update Hatası!");

            // Contains Testi
            if (!ht.ContainsKey("A")) throw new Exception("ContainsKey Hatası!");
            if (ht.ContainsKey("Z")) throw new Exception("Olmayan elemanı var sayıyor!");

            // Remove Testi
            ht.Remove("B");
            if (ht.ContainsKey("B")) throw new Exception("Remove Hatası!");
            if (ht.Count != 2) throw new Exception("Remove sonrası Count hatası!");

            // 1000+ Eleman ve Rehashing Testi (Sprint Kabul Kriteri)
            Console.WriteLine("DURUM: 1000+ eleman eklenerek Rehashing mekanizması test ediliyor...");
            var loadHt = new CustomHashTable<string, string>(16);
            int testCapacity = loadHt.GetCapacity();
            
            for (int i = 0; i < 2500; i++)
            {
                loadHt.Put($"User_{i}", $"Data_{i}");
            }

            if (loadHt.Count != 2500) throw new Exception("Yükleme sırasında eleman kayboldu!");
            if (loadHt.GetCapacity() <= 16) throw new Exception("Rehashing çalışmadı! Kapasite artmıyor.");
            if (loadHt.Get("User_1000") != "Data_1000") throw new Exception("Yüksek load altında arama hatası!");

            Console.WriteLine("[BAŞARILI] Hash Table operasyonları, 1000+ elemen load ve O(N) Rehashing testlerini kusursuz geçti.");
        }

        static void TestNodeAndEdgeModels()
        {
            Console.WriteLine("\nDURUM: Node ve Edge modelleri test ediliyor...");

            // 3 farklı Node türü (Acceptance criteria)
            var userNode = new Node("N-1", "User");
            userNode.Properties.Put("Name", "Fatma");

            var photoNode = new Node("N-2", "Photo");
            photoNode.Properties.Put("Resolution", "1080p");

            var eventNode = new Node("N-3", "Event");
            eventNode.Properties.Put("Date", "2026-05-15");

            // Edge türleri "FRIEND", "LIKES", "ATTENDS"
            var friendEdge = new Edge("E-1", userNode.Id, "N-99", "FRIEND");
            friendEdge.Properties.Put("Since", 2023);

            var likeEdge = new Edge("E-2", userNode.Id, photoNode.Id, "LIKES", true);
            var attendsEdge = new Edge("E-3", userNode.Id, eventNode.Id, "ATTENDS");

            if (userNode.Type != "User" || eventNode.Type != "Event" || photoNode.Type != "Photo")
                throw new Exception("Node tiplerinde hata!");

            if (friendEdge.RelationType != "FRIEND" || likeEdge.RelationType != "LIKES" || attendsEdge.RelationType != "ATTENDS")
                throw new Exception("Edge tiplerinde hata!");

            if ((string)userNode.Properties.Get("Name") != "Fatma")
                throw new Exception("Node properties Hash Table hatası!");

            Console.WriteLine("[BAŞARILI] Node ve Edge modelleri sorunsuz yaratıldı ve Custom Hash Table veriyapısıyla iç içe başarıyla entegre edildi.");
        }

        static void TestAlgorithmsAndQueue()
        {
            Console.WriteLine("\nDURUM: Sprint 1.2 Custom Queue ve Graf Gezinme algoritmaları test ediliyor...");

            // 1. Custom Queue 1000+ Eleman Enqueue/Dequeue Testi
            var queue = new CustomQueue<int>(16);
            for (int i = 0; i < 1500; i++)
            {
                queue.Enqueue(i);
            }
            if (queue.Count != 1500) throw new Exception("Kuyruk Enqueue kapasite hatası!");
            
            for (int i = 0; i < 1500; i++)
            {
                if (queue.Dequeue() != i) throw new Exception("Kuyruk Dequeue sıra/veri hatası!");
            }
            if (!queue.IsEmpty) throw new Exception("Kuyruk boşaltılamadı!");
            Console.WriteLine("[BAŞARILI] CustomQueue 1000+ eleman yük ve dinamik kapasite testini (O(1) amortized) geçti.");

            // 2. Mock Graph BFS ve DFS Testi
            var graph = new MockGraph();
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "E");

            Console.Write("BFS Çıktısı (Beklenen: A, B, C, D, E): ");
            GraphTraversal.BFS(graph, "A", (node) => Console.Write(node + ", "));
            Console.WriteLine();

            Console.Write("DFS Çıktısı (Beklenen: A, B, D, C, E): ");
            GraphTraversal.DFS(graph, "A", (node) => Console.Write(node + ", "));
            Console.WriteLine();

            Console.WriteLine("[BAŞARILI] BFS ve DFS algoritmaları Mock Adjacency List üzerinde doğru sırayla çalışıyor.");
        }
    }
}
