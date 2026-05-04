using System;
using System.Collections.Generic;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Algorithms
{
    /// <summary>
    /// Cok adimli iliskisel sorgular ve oneri sisteminden sorumlu motor.
    /// Gelistiren: Ozcan (Algorithm Master)
    /// </summary>
    public class ChainStepResult
    {
        public string Relation { get; set; }
        public int Count { get; set; }
    }

    public class ChainQueryResult
    {
        public Node[] AllNodes { get; set; }
        public ChainStepResult[] Steps { get; set; }
    }

    public class RelationalQueryEngine
    {
        private readonly PropertyGraph _graph;

        public RelationalQueryEngine(PropertyGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Belirli bir dugumden baslayarak verilen iliski zincirini takip eder.
        /// Orn: User -> FRIEND -> User -> ATTENDS -> Event -> UPLOADED -> Photo
        /// Karmasiklik: O(StepCount * AvgNodesPerStep * AvgEdgesPerNode)
        /// </summary>
        /// <param name="startNodeId">Baslangic dugum ID'si</param>
        /// <param name="relations">Takip edilecek iliski turleri (orn: ["FRIEND", "ATTENDS"])</param>
        /// <returns>Zincirin sonundaki benzersiz dugumler</returns>
        public ChainQueryResult ExecuteChainQuery(string startNodeId, string[] relations)
        {
            if (string.IsNullOrEmpty(startNodeId) || relations == null || relations.Length == 0)
                return new ChainQueryResult { AllNodes = Array.Empty<Node>(), Steps = Array.Empty<ChainStepResult>() };

            // Orumcek Agi (Spider Web) gorsellestirmesi icin tum yolu takip eden bir kume
            var allVisitedNodes = new CustomHashTable<string, Node>();
            var currentNodes = new CustomHashTable<string, Node>();
            var steps = new List<ChainStepResult>();

            Node startNode = _graph.GetNode(startNodeId);
            if (startNode == null) return new ChainQueryResult { AllNodes = Array.Empty<Node>(), Steps = Array.Empty<ChainStepResult>() };
            
            currentNodes.Put(startNodeId, startNode);
            allVisitedNodes.Put(startNodeId, startNode);

            // Her bir iliski adimi icin genisleme yap
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
                            nextNodes.Put(targetNode.Id, targetNode);
                            allVisitedNodes.Put(targetNode.Id, targetNode);
                        }
                    }
                }

                // Adim sonucunu kaydet
                steps.Add(new ChainStepResult { Relation = relation, Count = nextNodes.Count });

                // Eger bu adimda hic sonuc bulunamadiysa, zinciri burada kes ama 
                // elimizdeki mevcut (bir onceki adimdan kalan) dugumleri koru.
                if (nextNodes.Count == 0) break;

                currentNodes = nextNodes;
            }

            // Sonuc kumesini (tum zinciri) diziye cevir
            Node[] result = new Node[allVisitedNodes.Count];
            int index = 0;
            foreach (var kvp in allVisitedNodes)
            {
                result[index++] = kvp.Value;
            }

            return new ChainQueryResult 
            { 
                AllNodes = result, 
                Steps = steps.ToArray() 
            };
        }

        /// <summary>
        /// Arkadasin arkadasi (Friend-of-a-Friend) mantigiyla oneri sunar.
        /// Skorlama: Ortak arkadas sayisi (Mutual Friends).
        /// </summary>
        /// <param name="userId">Oneri yapilacak kullanici ID'si</param>
        /// <returns>Onerilen dugumler ve ortak arkadas skorlari (Node, Score)</returns>
        public (Node RecommendedNode, int Score)[] GetRecommendations(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Array.Empty<(Node, int)>();

            Node userNode = _graph.GetNode(userId);
            if (userNode == null || !string.Equals(userNode.Type, "User", StringComparison.OrdinalIgnoreCase))
                return Array.Empty<(Node, int)>();

            // Mevcut arkadaslari belirle (oneriden cikarmak icin)
            var currentFriends = new CustomHashTable<string, bool>();
            Edge[] friendEdges = _graph.GetEdgesByType(userId, "FRIEND");
            foreach (Edge edge in friendEdges)
            {
                currentFriends.Put(edge.DestinationId, true);
            }

            // Aday onerileri ve ortak arkadas sayilarini tut (TargetUserId -> Score)
            var candidateScores = new CustomHashTable<string, int>();

            // Arkadaslarin arkadaslarina bak
            foreach (Edge edge in friendEdges)
            {
                string friendId = edge.DestinationId;
                Edge[] friendsOfFriend = _graph.GetEdgesByType(friendId, "FRIEND");

                foreach (Edge fofEdge in friendsOfFriend)
                {
                    string fofId = fofEdge.DestinationId;

                    // Kendisi veya zaten arkadasiysa atla
                    if (fofId == userId || currentFriends.ContainsKey(fofId))
                        continue;

                    // Ortak arkadas skorunu artir
                    int currentScore = candidateScores.ContainsKey(fofId) ? candidateScores.Get(fofId) : 0;
                    candidateScores.Put(fofId, currentScore + 1);
                }
            }

            // Sonuclari topla
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

            // Basit bir siralama (Skora gore azalan) - Manuel Sort (Standard Library yasak ama Array.Sort kullanilabilir mi?)
            // Not: Array.Sort kullanilabilir ancak daha guvenli olmasi icin Ozcan stili basit bir Selection Sort yapalim.
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
