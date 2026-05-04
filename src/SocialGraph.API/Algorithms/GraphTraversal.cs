using System;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Algorithms
{
    /// <summary>
    /// Graf arama algoritmalari (BFS, DFS ve ShortestPath).
    /// Gelistiren: Ozcan (Algorithm Master)
    /// Tamamen CustomQueue ve CustomHashTable kullanilarak PropertyGraph'a uyarlanmistir.
    /// </summary>
    public static class GraphTraversal
    {
        /// <summary>
        /// Genislik Oncelikli Arama (Katmanli Gezinme)
        /// Ozellikler: Dugum ve kenar bazli filtreleme destekler.
        /// Karmasiklik: O(V + E) (V: Dugum, E: Kenar)
        /// Uzay Karmasikligi: O(V)
        /// </summary>
        public static void BFS(
            PropertyGraph graph, 
            string startNodeId, 
            Action<Node> onVisit, 
            Func<Node, bool> nodeFilter = null, 
            Func<Edge, bool> edgeFilter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNodeId) || onVisit == null) return;

            Node startNode = graph.GetNode(startNodeId);
            if (startNode == null) return;

            var queue = new CustomQueue<string>();
            var visited = new CustomHashTable<string, bool>();

            queue.Enqueue(startNodeId);
            visited.Put(startNodeId, true);

            while (!queue.IsEmpty)
            {
                string currentId = queue.Dequeue();
                Node currentNode = graph.GetNode(currentId);

                if (currentNode != null)
                {
                    // Dugum filtresi varsa uygula, yoksa direkt ziyaret et
                    if (nodeFilter == null || nodeFilter(currentNode))
                    {
                        onVisit(currentNode);
                    }
                }

                Edge[] edges = graph.GetEdges(currentId);
                for (int i = 0; i < edges.Length; i++)
                {
                    Edge edge = edges[i];

                    // Kenar filtresi varsa uygula
                    if (edgeFilter != null && !edgeFilter(edge)) continue;

                    string neighborId = edge.DestinationId;
                    if (!visited.ContainsKey(neighborId))
                    {
                        visited.Put(neighborId, true);
                        queue.Enqueue(neighborId);
                    }
                }
            }
        }

        /// <summary>
        /// Derinlik Oncelikli Arama (Ozyinelemeli)
        /// Ozellikler: Dugum ve kenar bazli filtreleme destekler.
        /// Karmasiklik: O(V + E) (V: Dugum, E: Kenar)
        /// </summary>
        public static void DFS(
            PropertyGraph graph, 
            string startNodeId, 
            Action<Node> onVisit, 
            Func<Node, bool> nodeFilter = null, 
            Func<Edge, bool> edgeFilter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNodeId) || onVisit == null) return;

            var visited = new CustomHashTable<string, bool>();
            DFS_Recursive(graph, startNodeId, visited, onVisit, nodeFilter, edgeFilter);
        }

        private static void DFS_Recursive(
            PropertyGraph graph, 
            string currentId, 
            CustomHashTable<string, bool> visited, 
            Action<Node> onVisit, 
            Func<Node, bool> nodeFilter, 
            Func<Edge, bool> edgeFilter)
        {
            visited.Put(currentId, true);

            Node currentNode = graph.GetNode(currentId);
            if (currentNode != null)
            {
                if (nodeFilter == null || nodeFilter(currentNode))
                {
                    onVisit(currentNode);
                }
            }

            Edge[] edges = graph.GetEdges(currentId);
            for (int i = 0; i < edges.Length; i++)
            {
                Edge edge = edges[i];

                if (edgeFilter != null && !edgeFilter(edge)) continue;

                string neighborId = edge.DestinationId;
                if (!visited.ContainsKey(neighborId))
                {
                    DFS_Recursive(graph, neighborId, visited, onVisit, nodeFilter, edgeFilter);
                }
            }
        }

        /// <summary>
        /// Iki dugum arasindaki en kisa yolu (kenar sayisi bazinda) bulan BFS algoritmasi.
        /// Filtreleme kullanilarak belirli iliskiler (orn. sadece LIKES) uzerinden yol aranabilir.
        /// Karmasiklik: O(V + E)
        /// Uzay Karmasikligi: O(V)
        /// </summary>
        public static string[] ShortestPath(
            PropertyGraph graph, 
            string startNodeId, 
            string targetNodeId, 
            Func<Edge, bool> edgeFilter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNodeId) || string.IsNullOrEmpty(targetNodeId))
                return Array.Empty<string>();

            if (graph.GetNode(startNodeId) == null || graph.GetNode(targetNodeId) == null)
                return Array.Empty<string>();

            if (startNodeId == targetNodeId)
                return new string[] { startNodeId };

            var queue = new CustomQueue<string>();
            var visited = new CustomHashTable<string, bool>();
            
            // Yol takibi icin (ChildId -> ParentId)
            var parents = new CustomHashTable<string, string>();

            queue.Enqueue(startNodeId);
            visited.Put(startNodeId, true);

            bool found = false;

            while (!queue.IsEmpty)
            {
                string currentId = queue.Dequeue();

                if (currentId == targetNodeId)
                {
                    found = true;
                    break;
                }

                Edge[] edges = graph.GetEdges(currentId);
                for (int i = 0; i < edges.Length; i++)
                {
                    Edge edge = edges[i];

                    if (edgeFilter != null && !edgeFilter(edge)) continue;

                    string neighborId = edge.DestinationId;
                    if (!visited.ContainsKey(neighborId))
                    {
                        visited.Put(neighborId, true);
                        parents.Put(neighborId, currentId);
                        queue.Enqueue(neighborId);
                    }
                }
            }

            if (!found) return Array.Empty<string>();

            // Hedef bulundu, geriye dogru (backtrack) yolu insa et
            int pathLength = 1;
            string curr = targetNodeId;
            while (parents.ContainsKey(curr))
            {
                pathLength++;
                curr = parents.Get(curr);
            }

            string[] path = new string[pathLength];
            curr = targetNodeId;
            for (int i = pathLength - 1; i >= 0; i--)
            {
                path[i] = curr;
                if (i > 0) curr = parents.Get(curr);
            }

            return path;
        }
        /// <summary>
        /// Iki dugum arasindaki herhangi bir yolu (DFS algoritmasiyla) bulan metot.
        /// Filtreleme kullanilarak belirli iliskiler (orn. sadece LIKES) uzerinden yol aranabilir.
        /// Karmasiklik: O(V + E)
        /// Uzay Karmasikligi: O(V)
        /// </summary>
        public static string[] DFS_Path(
            PropertyGraph graph, 
            string startNodeId, 
            string targetNodeId, 
            Func<Edge, bool> edgeFilter = null)
        {
            if (graph == null || string.IsNullOrEmpty(startNodeId) || string.IsNullOrEmpty(targetNodeId))
                return Array.Empty<string>();

            if (graph.GetNode(startNodeId) == null || graph.GetNode(targetNodeId) == null)
                return Array.Empty<string>();

            if (startNodeId == targetNodeId)
                return new string[] { startNodeId };

            var visited = new CustomHashTable<string, bool>();
            var parents = new CustomHashTable<string, string>();
            
            bool found = DFS_Path_Recursive(graph, startNodeId, targetNodeId, visited, parents, edgeFilter);

            if (!found) return Array.Empty<string>();

            // Hedef bulundu, geriye dogru (backtrack) yolu insa et
            int pathLength = 1;
            string curr = targetNodeId;
            while (parents.ContainsKey(curr))
            {
                pathLength++;
                curr = parents.Get(curr);
            }

            string[] path = new string[pathLength];
            curr = targetNodeId;
            for (int i = pathLength - 1; i >= 0; i--)
            {
                path[i] = curr;
                if (i > 0) curr = parents.Get(curr);
            }

            return path;
        }

        private static bool DFS_Path_Recursive(
            PropertyGraph graph, 
            string currentId, 
            string targetNodeId,
            CustomHashTable<string, bool> visited, 
            CustomHashTable<string, string> parents,
            Func<Edge, bool> edgeFilter)
        {
            visited.Put(currentId, true);

            if (currentId == targetNodeId)
                return true;

            Edge[] edges = graph.GetEdges(currentId);
            for (int i = 0; i < edges.Length; i++)
            {
                Edge edge = edges[i];

                if (edgeFilter != null && !edgeFilter(edge)) continue;

                string neighborId = edge.DestinationId;
                if (!visited.ContainsKey(neighborId))
                {
                    parents.Put(neighborId, currentId);
                    if (DFS_Path_Recursive(graph, neighborId, targetNodeId, visited, parents, edgeFilter))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
