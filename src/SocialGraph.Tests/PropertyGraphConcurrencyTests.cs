using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;
using Xunit;
using Xunit.Abstractions;

namespace SocialGraph.Tests
{
    public class PropertyGraphConcurrencyTests
    {
        private readonly ITestOutputHelper _output;

        public PropertyGraphConcurrencyTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Concurrent_ReadWrite_ShouldNotDeadlock()
        {
            // Arrange
            var graph = new PropertyGraph();
            int readerCount = 15;
            int writerCount = 2;
            int operationsPerThread = 100;
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            // Seed data
            graph.AddNode(new Node("root", "User"));

            // Writer Tasks
            var writers = Enumerable.Range(0, writerCount).Select(i => Task.Run(() =>
            {
                for (int j = 0; j < operationsPerThread; j++)
                {
                    if (cts.Token.IsCancellationRequested) break;
                    
                    string nodeId = $"node_{i}_{j}";
                    graph.AddNode(new Node(nodeId, "User"));
                    graph.AddEdge(new Edge($"edge_{i}_{j}", "root", nodeId, "FRIEND", false));
                    
                    Thread.Sleep(5); // AI Worker simulasyonu
                }
            })).ToArray();

            // Reader Tasks
            var readerResults = new List<long>();
            var readers = Enumerable.Range(0, readerCount).Select(i => Task.Run(() =>
            {
                var sw = new Stopwatch();
                int successfulReads = 0;
                
                for (int j = 0; j < operationsPerThread; j++)
                {
                    if (cts.Token.IsCancellationRequested) break;

                    sw.Start();
                    var nodes = graph.GetAllNodes();
                    var edges = graph.GetAllEdges();
                    sw.Stop();

                    if (nodes.Length > 0) successfulReads++;
                    Thread.Sleep(2);
                }
                
                lock (readerResults)
                {
                    readerResults.Add(sw.ElapsedMilliseconds);
                }
            })).ToArray();

            // Act
            await Task.WhenAll(writers.Concat(readers));

            // Assert
            Assert.False(cts.Token.IsCancellationRequested, "Test timed out due to possible deadlock.");
            
            double avgReadTime = readerResults.Average();
            _output.WriteLine($"Eszamanli Okuma Basarili. Ortalama Okuma Suresi: {avgReadTime:F2} ms");
            
            Assert.True(graph.NodeCount > 0);
            Assert.True(graph.EdgeCount > 0);
        }

        [Fact]
        public async Task Massive_Parallel_Writes_Consistency_Check()
        {
            var graph = new PropertyGraph();
            int threadCount = 10;
            int nodesPerThread = 100;

            var tasks = Enumerable.Range(0, threadCount).Select(t => Task.Run(() =>
            {
                for (int i = 0; i < nodesPerThread; i++)
                {
                    string id = $"t{t}_n{i}";
                    graph.AddNode(new Node(id, "User"));
                }
            })).ToArray();

            await Task.WhenAll(tasks);

            Assert.Equal(threadCount * nodesPerThread, graph.NodeCount);
        }
    }
}
