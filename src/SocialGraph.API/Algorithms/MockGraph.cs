using System;
using SocialGraph.API.DataStructures;

namespace SocialGraph.API.Algorithms
{
    /// <summary>
    /// BFS ve DFS testleri icin Mock Adjacency List.
    /// </summary>
    public class MockGraph
    {
        
        // Kendi Hash Table yapimizi kullaniyoruz (NodeID -> Komsu Node ID'leri)
        private CustomHashTable<string, string[]> _adjacencyList;

        public MockGraph()
        {
            _adjacencyList = new CustomHashTable<string, string[]>();
        }

        public void AddEdge(string fromNode, string toNode)
        {
            if (!_adjacencyList.ContainsKey(fromNode))
            {
                _adjacencyList.Put(fromNode, new string[0]);
            }

            var neighbors = _adjacencyList.Get(fromNode);
            var newNeighbors = new string[neighbors.Length + 1];
            Array.Copy(neighbors, newNeighbors, neighbors.Length);
            newNeighbors[neighbors.Length] = toNode;
            
            _adjacencyList.Put(fromNode, newNeighbors);

            // Ensure toNode also exists in the graph even if it has no outgoing edges
            if (!_adjacencyList.ContainsKey(toNode))
            {
                _adjacencyList.Put(toNode, new string[0]);
            }
        }

        public string[] GetNeighbors(string nodeId)
        {
            if (_adjacencyList.ContainsKey(nodeId))
            {
                return _adjacencyList.Get(nodeId);
            }
            return new string[0];
        }
    }
}
