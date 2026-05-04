using Xunit;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;

namespace SocialGraph.Tests
{
    /// <summary>
    /// PropertyGraph sinifi icin yazilan birim testleri.
    /// Sprint 2.5 kapsaminda Isra tarafindan yazilmistir.
    /// </summary>
    public class PropertyGraphTests
    {
        [Fact]
        public void AddNode_ShouldAddNodeToGraph()
        {
            var graph = new PropertyGraph();
            var node = new Node("1", "User");
            
            graph.AddNode(node);
            
            var retrieved = graph.GetNode("1");
            Assert.NotNull(retrieved);
            Assert.Equal("User", retrieved.Type);
        }

        [Fact]
        public void AddEdge_ShouldConnectNodesAndReturnNeighbors()
        {
            var graph = new PropertyGraph();
            graph.AddNode(new Node("A", "User"));
            graph.AddNode(new Node("B", "User"));
            
            // Directed edge (A -> B) - Test amacli LIKES kullaniyoruz (yonlu)
            var edge = new Edge("e1", "A", "B", "LIKES", true);
            graph.AddEdge(edge);
            
            var neighborsOfA = graph.GetNeighbors("A");
            Assert.Single(neighborsOfA);
            Assert.Equal("B", neighborsOfA[0].Id);
            
            var neighborsOfB = graph.GetNeighbors("B");
            Assert.Empty(neighborsOfB); // Yonlu oldugu icin B'nin komsusu A degildir
        }

        [Fact]
        public void AddEdge_Undirected_ShouldConnectBothWays()
        {
            var graph = new PropertyGraph();
            graph.AddNode(new Node("A", "User"));
            graph.AddNode(new Node("B", "User"));
            
            // Undirected edge (A <-> B)
            var edge = new Edge("e1", "A", "B", "FRIEND", false);
            graph.AddEdge(edge);
            
            var neighborsOfA = graph.GetNeighbors("A");
            Assert.Single(neighborsOfA);
            Assert.Equal("B", neighborsOfA[0].Id);
            
            var neighborsOfB = graph.GetNeighbors("B");
            Assert.Single(neighborsOfB);
            Assert.Equal("A", neighborsOfB[0].Id);
        }

        [Fact]
        public void RemoveNode_ShouldDeleteNodeAndItsEdges()
        {
            var graph = new PropertyGraph();
            graph.AddNode(new Node("A", "User"));
            graph.AddNode(new Node("B", "User"));
            graph.AddEdge(new Edge("e1", "A", "B", "FRIEND", false));
            
            graph.RemoveNode("A");
            
            var nodeA = graph.GetNode("A");
            Assert.Null(nodeA);
            
            var neighborsOfB = graph.GetNeighbors("B");
            Assert.Empty(neighborsOfB); // A silindigi icin B'nin komsusu kalmadi
        }

        [Fact]
        public void IsolatedNode_ShouldHaveNoNeighbors()
        {
            var graph = new PropertyGraph();
            graph.AddNode(new Node("Lonely", "User"));
            
            var neighbors = graph.GetNeighbors("Lonely");
            Assert.Empty(neighbors);
        }
    }
}
