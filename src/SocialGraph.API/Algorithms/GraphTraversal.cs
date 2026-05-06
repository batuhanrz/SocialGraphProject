using System;
using System.Collections.Generic;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.API.Algorithms
{
    public static class GraphTraversal
    {
        public static void BFS(PropertyGraph graph, string startNodeId, Action<string> onVisit, Func<Node, bool> nodeFilter = null, Func<Edge, bool> edgeFilter = null)
        {
            var queue = new CustomQueue<string>();
            var visited = new CustomHashTable<string, bool>();

            queue.Enqueue(startNodeId);
            visited.Put(startNodeId, true);

            while (!queue.IsEmpty)
            {
                string currentId = queue.Dequeue();
                var node = graph.GetNode(currentId);

                if (nodeFilter != null && !nodeFilter(node)) continue;
                onVisit(currentId);

                Edge[] edges = graph.GetEdges(currentId);
                foreach (var edge in edges)
                {
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

        public static void DFS(PropertyGraph graph, string startNodeId, Action<string> onVisit, Func<Node, bool> nodeFilter = null, Func<Edge, bool> edgeFilter = null)
        {
            var visited = new CustomHashTable<string, bool>();
            DFS_Recursive(graph, startNodeId, visited, onVisit, nodeFilter, edgeFilter);
        }

        private static void DFS_Recursive(PropertyGraph graph, string currentId, CustomHashTable<string, bool> visited, Action<string> onVisit, Func<Node, bool> nodeFilter, Func<Edge, bool> edgeFilter)
        {
            visited.Put(currentId, true);
            var node = graph.GetNode(currentId);

            if (nodeFilter != null && !nodeFilter(node)) return;
            onVisit(currentId);

            Edge[] edges = graph.GetEdges(currentId);
            foreach (var edge in edges)
            {
                if (edgeFilter != null && !edgeFilter(edge)) continue;
                string neighborId = edge.DestinationId;
                if (!visited.ContainsKey(neighborId))
                {
                    DFS_Recursive(graph, neighborId, visited, onVisit, nodeFilter, edgeFilter);
                }
            }
        }

        public static PathStep[] ShortestPath(PropertyGraph graph, string startNodeId, string targetNodeId, Func<Edge, bool> edgeFilter = null)
        {
            if (startNodeId == targetNodeId)
                return new[] { new PathStep { NodeId = startNodeId, Relation = "Baslangic" } };

            var queue = new CustomQueue<string>();
            var visited = new CustomHashTable<string, bool>();
            var parents = new CustomHashTable<string, string>();
            var edgeRelations = new CustomHashTable<string, string>();

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
                foreach (var edge in edges)
                {
                    if (edgeFilter != null && !edgeFilter(edge)) continue;
                    string neighborId = edge.DestinationId;
                    if (!visited.ContainsKey(neighborId))
                    {
                        visited.Put(neighborId, true);
                        parents.Put(neighborId, currentId);
                        edgeRelations.Put(neighborId, edge.RelationType);
                        queue.Enqueue(neighborId);
                    }
                }
            }

            if (!found) return Array.Empty<PathStep>();

            var pathList = new List<PathStep>();
            string curr = targetNodeId;
            while (curr != startNodeId)
            {
                pathList.Add(new PathStep { NodeId = curr, Relation = edgeRelations.Get(curr) });
                curr = parents.Get(curr);
            }
            pathList.Add(new PathStep { NodeId = startNodeId, Relation = "Baslangic" });
            pathList.Reverse();
            return pathList.ToArray();
        }

        public static PathStep[] DFS_Path(PropertyGraph graph, string startNodeId, string targetNodeId, Func<Edge, bool> edgeFilter = null)
        {
            if (startNodeId == targetNodeId)
                return new[] { new PathStep { NodeId = startNodeId, Relation = "Baslangic" } };

            var visited = new CustomHashTable<string, bool>();
            var parents = new CustomHashTable<string, string>();
            var edgeRelations = new CustomHashTable<string, string>();

            if (DFS_Path_Recursive(graph, startNodeId, targetNodeId, visited, parents, edgeRelations, edgeFilter))
            {
                var pathList = new List<PathStep>();
                string curr = targetNodeId;
                while (curr != startNodeId)
                {
                    pathList.Add(new PathStep { NodeId = curr, Relation = edgeRelations.Get(curr) });
                    curr = parents.Get(curr);
                }
                pathList.Add(new PathStep { NodeId = startNodeId, Relation = "Baslangic" });
                pathList.Reverse();
                return pathList.ToArray();
            }

            return Array.Empty<PathStep>();
        }

        private static bool DFS_Path_Recursive(PropertyGraph graph, string currentId, string targetId, 
            CustomHashTable<string, bool> visited, CustomHashTable<string, string> parents, CustomHashTable<string, string> edgeRelations, Func<Edge, bool> edgeFilter)
        {
            visited.Put(currentId, true);
            if (currentId == targetId) return true;

            Edge[] edges = graph.GetEdges(currentId);
            foreach (var edge in edges)
            {
                if (edgeFilter != null && !edgeFilter(edge)) continue;
                string neighborId = edge.DestinationId;
                if (!visited.ContainsKey(neighborId))
                {
                    parents.Put(neighborId, currentId);
                    edgeRelations.Put(neighborId, edge.RelationType);
                    if (DFS_Path_Recursive(graph, neighborId, targetId, visited, parents, edgeRelations, edgeFilter))
                        return true;
                }
            }
            return false;
        }
    }

    public struct PathStep
    {
        public string NodeId { get; set; }
        public string Relation { get; set; }
    }
}
