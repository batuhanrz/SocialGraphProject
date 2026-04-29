using System.Diagnostics;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;
using SocialGraph.API.Algorithms;
using Xunit.Abstractions;

namespace SocialGraph.Tests
{
    public class LoadTests
    {
        private readonly ITestOutputHelper _output;

        public LoadTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(5000)]
        public void Measure_Performance_Across_Scales(int nodeCount)
        {
            var graph = new PropertyGraph();
            var trie = new CustomTrie();
            var sw = new Stopwatch();

            _output.WriteLine($"--- Scale Test: {nodeCount} Nodes ---");

            // 1. Measure Batch Insertion (Nodes)
            sw.Start();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node($"u{i}", "User");
                node.Properties.Put("Name", $"User {i}");
                graph.AddNode(node);
                trie.Insert($"User {i}", node.Id);
            }
            sw.Stop();
            _output.WriteLine($"Insertion (Nodes + Trie): {sw.ElapsedMilliseconds} ms");
            var insertionTime = sw.ElapsedMilliseconds;

            // 2. Measure Batch Insertion (Edges - Sparse: 2 edges per node)
            sw.Restart();
            for (int i = 0; i < nodeCount - 1; i++)
            {
                var edge = new Edge($"e{i}", $"u{i}", $"u{i+1}", "FRIEND", false);
                graph.AddEdge(edge);
            }
            sw.Stop();
            _output.WriteLine($"Insertion (Edges): {sw.ElapsedMilliseconds} ms");

            // 3. Measure Trie Autocomplete
            sw.Restart();
            var results = trie.AutoComplete("User 4"); // Matches User 4, User 40, User 400 etc.
            sw.Stop();
            _output.WriteLine($"Trie Autocomplete ('User 4'): {sw.ElapsedMilliseconds} ms (Found: {results.Length})");

            // 4. Measure BFS (Full traversal in chain)
            sw.Restart();
            var pathList = new List<string>();
            GraphTraversal.BFS(graph, "u0", node => pathList.Add(node.Id));
            sw.Stop();
            _output.WriteLine($"BFS Traversal: {sw.ElapsedMilliseconds} ms");

            Assert.True(insertionTime < 2000, "Insertion is too slow!");
        }
    }
}
