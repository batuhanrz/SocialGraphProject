using System;
using SocialGraph.API.DataStructures;

namespace SocialGraph.API.Algorithms
{
    /// <summary>
    /// Graf arama algoritmaları (BFS ve DFS).
    /// </summary>
    public static class GraphTraversal
    {
        /// <summary>
        /// Genişlik Öncelikli Arama (Katmanlı Gezinme)
        /// Karmaşıklık: O(V + E) (V: Düğüm, E: Kenar)
        /// </summary>
        public static void BFS(MockGraph graph, string startNode, Action<string> onVisit, Func<string, bool> filter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNode) || onVisit == null) return;

            var queue = new CustomQueue<string>();
            var visited = new CustomHashTable<string, bool>();

            queue.Enqueue(startNode);
            visited.Put(startNode, true);

            while (!queue.IsEmpty)
            {
                string current = queue.Dequeue();

                // Apply filter if exists
                if (filter == null || filter(current))
                {
                    onVisit(current);
                }

                string[] neighbors = graph.GetNeighbors(current);
                for (int i = 0; i < neighbors.Length; i++)
                {
                    string neighbor = neighbors[i];
                    if (!visited.ContainsKey(neighbor))
                    {
                        visited.Put(neighbor, true);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        /// <summary>
        /// Derinlik Öncelikli Arama (Özyinelemeli)
        /// Karmaşıklık: O(V + E) (V: Düğüm, E: Kenar)
        /// </summary>
        public static void DFS(MockGraph graph, string startNode, Action<string> onVisit, Func<string, bool> filter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNode) || onVisit == null) return;

            var visited = new CustomHashTable<string, bool>();
            DFS_Recursive(graph, startNode, visited, onVisit, filter);
        }

        private static void DFS_Recursive(MockGraph graph, string current, CustomHashTable<string, bool> visited, Action<string> onVisit, Func<string, bool> filter)
        {
            visited.Put(current, true);

            if (filter == null || filter(current))
            {
                onVisit(current);
            }

            string[] neighbors = graph.GetNeighbors(current);
            for (int i = 0; i < neighbors.Length; i++)
            {
                string neighbor = neighbors[i];
                if (!visited.ContainsKey(neighbor))
                {
                    DFS_Recursive(graph, neighbor, visited, onVisit, filter);
                }
            }
        }
    }
}
