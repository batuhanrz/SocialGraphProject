using Xunit;
using SocialGraph.API.DataStructures;
using SocialGraph.API.Models;
using SocialGraph.API.Algorithms;

namespace SocialGraph.Tests
{
    public class RelationalQueryTests
    {
        private readonly PropertyGraph _graph;
        private readonly RelationalQueryEngine _engine;

        public RelationalQueryTests()
        {
            _graph = new PropertyGraph();
            _engine = new RelationalQueryEngine(_graph);
            SetupMockData();
        }

        private void SetupMockData()
        {
            // Nodes
            var u1 = new Node("u1", "User");
            var u2 = new Node("u2", "User");
            var u3 = new Node("u3", "User");
            var u4 = new Node("u4", "User");
            
            var e1 = new Node("e1", "Event");
            var e2 = new Node("e2", "Event");
            
            var p1 = new Node("p1", "Photo");
            var p2 = new Node("p2", "Photo");

            _graph.AddNode(u1);
            _graph.AddNode(u2);
            _graph.AddNode(u3);
            _graph.AddNode(u4);
            _graph.AddNode(e1);
            _graph.AddNode(e2);
            _graph.AddNode(p1);
            _graph.AddNode(p2);

            // Edges
            // u1 -> u2 (FRIEND)
            // u1 -> u3 (FRIEND)
            // u2 -> u4 (FRIEND)
            // u3 -> u4 (FRIEND)
            _graph.AddEdge(new Edge("edge1", "u1", "u2", "FRIEND", false));
            _graph.AddEdge(new Edge("edge2", "u1", "u3", "FRIEND", false));
            _graph.AddEdge(new Edge("edge3", "u2", "u4", "FRIEND", false));
            _graph.AddEdge(new Edge("edge4", "u3", "u4", "FRIEND", false));

            // u2 -> e1 (ATTENDS)
            // u3 -> e1 (ATTENDS)
            // u4 -> e2 (ATTENDS)
            _graph.AddEdge(new Edge("edge5", "u2", "e1", "ATTENDS", true));
            _graph.AddEdge(new Edge("edge6", "u3", "e1", "ATTENDS", true));
            _graph.AddEdge(new Edge("edge7", "u4", "e2", "ATTENDS", true));

            // e1 -> p1 (UPLOADED)
            // e2 -> p2 (UPLOADED)
            _graph.AddEdge(new Edge("edge8", "e1", "p1", "UPLOADED", true));
            _graph.AddEdge(new Edge("edge9", "e2", "p2", "UPLOADED", true));
        }

        [Fact]
        public void ExecuteChainQuery_UserToFriends_ShouldReturnCorrectNodes()
        {
            // Act: u1 -> FRIEND
            var result = _engine.ExecuteChainQuery("u1", new[] { "FRIEND" });

            // Assert: u2 and u3
            Assert.Equal(2, result.Length);
            Assert.Contains(result, n => n.Id == "u2");
            Assert.Contains(result, n => n.Id == "u3");
        }

        [Fact]
        public void ExecuteChainQuery_ComplexChain_ShouldReturnTargetNodes()
        {
            // Act: u1 -> FRIEND -> ATTENDS -> UPLOADED
            // u1 friends are u2, u3.
            // u2 and u3 attend e1.
            // e1 has photo p1.
            var result = _engine.ExecuteChainQuery("u1", new[] { "FRIEND", "ATTENDS", "UPLOADED" });

            // Assert
            Assert.Single(result);
            Assert.Equal("p1", result[0].Id);
        }

        [Fact]
        public void GetRecommendations_ShouldReturnMutualFriendsSortedByScore()
        {
            // u1 is friends with u2 and u3.
            // u2 is friends with u1 and u4.
            // u3 is friends with u1 and u4.
            // u4 should be recommended to u1 with score 2 (mutual friends u2, u3).
            
            // Act
            var recs = _engine.GetRecommendations("u1");

            // Assert
            Assert.Single(recs);
            Assert.Equal("u4", recs[0].RecommendedNode.Id);
            Assert.Equal(2, recs[0].Score);
        }

        [Fact]
        public void ExecuteChainQuery_InvalidRelation_ShouldReturnEmpty()
        {
            // Act
            var result = _engine.ExecuteChainQuery("u1", new[] { "INVALID_REL" });

            // Assert
            Assert.Empty(result);
        }
    }
}
