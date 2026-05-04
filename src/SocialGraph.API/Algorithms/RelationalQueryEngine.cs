using System;
using System.Collections.Generic;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Algorithms
{
    /// <summary>
    /// Çok adımlı ilişkisel sorgular ve öneri sisteminden sorumlu motor.
    /// Geliştiren: Özcan (Algorithm Master)
    /// </summary>
    public class RelationalQueryEngine
    {
        private readonly PropertyGraph _graph;

        public RelationalQueryEngine(PropertyGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Belirli bir düğümden başlayarak verilen ilişki zincirini takip eder.
        /// Örn: User -> FRIEND -> User -> ATTENDS -> Event -> UPLOADED -> Photo
        /// Karmaşıklık: O(StepCount * AvgNodesPerStep * AvgEdgesPerNode)
        /// </summary>
        /// <param name="startNodeId">Başlangıç düğüm ID'si</param>
        /// <param name="relations">Takip edilecek ilişki türleri (örn: ["FRIEND", "ATTENDS"])</param>
        /// <returns>Zincirin sonundaki benzersiz düğümler</returns>
        public Node[] ExecuteChainQuery(string startNodeId, string[] relations)
        {
            if (string.IsNullOrEmpty(startNodeId) || relations == null || relations.Length == 0)
                return Array.Empty<Node>();

            // İlk adım: Başlangıç düğümünü sete ekle
            var currentNodes = new CustomHashTable<string, Node>();
            Node startNode = _graph.GetNode(startNodeId);
            if (startNode == null) return Array.Empty<Node>();
            
            currentNodes.Put(startNodeId, startNode);

            // Her bir ilişki adımı için genişleme yap
            foreach (string relation in relations)
            {
                var nextNodes = new CustomHashTable<string, Node>();

                foreach (var kvp in currentNodes)
                {
                    string currentNodeId = kvp.Key;
                    Edge[] edges = _graph.GetEdgesByType(currentNodeId, relation);

                    foreach (Edge edge in edges)
                    {
                        Node targetNode = _graph.GetNode(edge.DestinationId);
                        if (targetNode != null)
                        {
                            // Tekilleştirme CustomHashTable tarafından otomatik sağlanır
                            nextNodes.Put(targetNode.Id, targetNode);
                        }
                    }
                }

                // Eğer bu adımda hiç sonuç bulunamadıysa, zinciri burada kes ama 
                // elimizdeki mevcut (bir önceki adımdan kalan) düğümleri koru.
                if (nextNodes.Count == 0) break;

                currentNodes = nextNodes;
            }

            // Sonuç kümesini diziye çevir
            Node[] result = new Node[currentNodes.Count];
            int index = 0;
            foreach (var kvp in currentNodes)
            {
                result[index++] = kvp.Value;
            }

            return result;
        }

        /// <summary>
        /// Arkadaşın arkadaşı (Friend-of-a-Friend) mantığıyla öneri sunar.
        /// Skorlama: Ortak arkadaş sayısı (Mutual Friends).
        /// </summary>
        /// <param name="userId">Öneri yapılacak kullanıcı ID'si</param>
        /// <returns>Önerilen düğümler ve ortak arkadaş skorları (Node, Score)</returns>
        public (Node RecommendedNode, int Score)[] GetRecommendations(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Array.Empty<(Node, int)>();

            Node userNode = _graph.GetNode(userId);
            if (userNode == null || !string.Equals(userNode.Type, "User", StringComparison.OrdinalIgnoreCase))
                return Array.Empty<(Node, int)>();

            // Mevcut arkadaşları belirle (öneriden çıkarmak için)
            var currentFriends = new CustomHashTable<string, bool>();
            Edge[] friendEdges = _graph.GetEdgesByType(userId, "FRIEND");
            foreach (Edge edge in friendEdges)
            {
                currentFriends.Put(edge.DestinationId, true);
            }

            // Aday önerileri ve ortak arkadaş sayılarını tut (TargetUserId -> Score)
            var candidateScores = new CustomHashTable<string, int>();

            // Arkadaşların arkadaşlarına bak
            foreach (Edge edge in friendEdges)
            {
                string friendId = edge.DestinationId;
                Edge[] friendsOfFriend = _graph.GetEdgesByType(friendId, "FRIEND");

                foreach (Edge fofEdge in friendsOfFriend)
                {
                    string fofId = fofEdge.DestinationId;

                    // Kendisi veya zaten arkadaşıysa atla
                    if (fofId == userId || currentFriends.ContainsKey(fofId))
                        continue;

                    // Ortak arkadaş skorunu artır
                    int currentScore = candidateScores.ContainsKey(fofId) ? candidateScores.Get(fofId) : 0;
                    candidateScores.Put(fofId, currentScore + 1);
                }
            }

            // Sonuçları topla
            var result = new (Node RecommendedNode, int Score)[candidateScores.Count];
            int index = 0;
            foreach (var kvp in candidateScores)
            {
                Node recommended = _graph.GetNode(kvp.Key);
                if (recommended != null)
                {
                    result[index++] = (recommended, kvp.Value);
                }
            }

            // Basit bir sıralama (Skora göre azalan) - Manuel Sort (Standard Library yasak ama Array.Sort kullanılabilir mi?)
            // Not: Array.Sort kullanılabilir ancak daha güvenli olması için Özcan stili basit bir Selection Sort yapalım.
            for (int i = 0; i < result.Length - 1; i++)
            {
                for (int j = i + 1; j < result.Length; j++)
                {
                    if (result[j].Score > result[i].Score)
                    {
                        var temp = result[i];
                        result[i] = result[j];
                        result[j] = temp;
                    }
                }
            }

            return result;
        }
    }
}
