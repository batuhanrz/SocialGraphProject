using Xunit;
using SocialGraph.API.Controllers;
using SocialGraph.API.DataStructures;
using SocialGraph.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SocialGraph.Tests
{
    public class IntegrationTests
    {
        private readonly PropertyGraph _graph;
        private readonly CustomTrie _trie;
        private readonly NodesController _nodesController;
        private readonly SearchController _searchController;
        private readonly TraversalController _traversalController;

        public IntegrationTests()
        {
            _graph = new PropertyGraph();
            _trie = new CustomTrie();
            _nodesController = new NodesController(_graph, _trie);
            _searchController = new SearchController(_trie, _graph);
            _traversalController = new TraversalController(_graph);
        }

        [Fact]
        public void FullFlow_WorkerToSearch_ShouldReturnCorrectNodes()
        {
            // 1. Simulate AI Worker pushing nodes
            var nodes = new List<NodeDto>
            {
                new NodeDto { Id = "u1", Type = "User", Properties = new Dictionary<string, object> { { "Name", "Isra" } } },
                new NodeDto { Id = "p1", Type = "Photo", Properties = new Dictionary<string, object> { { "Title", "Sunset" } } }
            };
            
            var result = _nodesController.AddNodesBatch(nodes);
            Assert.IsType<OkObjectResult>(result);

            // 2. Search for the added node via SearchController (Trie Integration)
            var searchResult = _searchController.AutoComplete("Is");
            var okSearch = Assert.IsType<OkObjectResult>(searchResult.Result);
            var searchData = Assert.IsType<List<SearchResultDto>>(okSearch.Value);
            
            Assert.Single(searchData);
            Assert.Equal("u1", searchData[0].NodeId);
            Assert.Equal("Isra", searchData[0].Label);
        }

        [Fact]
        public void GraphTraversal_WithRealEdges_ShouldWorkCorrectly()
        {
            // 1. Setup Graph
            _nodesController.AddNodesBatch(new List<NodeDto> {
                new NodeDto { Id = "A", Type = "User", Properties = new Dictionary<string, object> { { "Name", "A" } } },
                new NodeDto { Id = "B", Type = "User", Properties = new Dictionary<string, object> { { "Name", "B" } } },
                new NodeDto { Id = "C", Type = "User", Properties = new Dictionary<string, object> { { "Name", "C" } } }
            });

            _nodesController.AddEdgesBatch(new List<EdgeDto> {
                new EdgeDto { Id = "e1", SourceId = "A", TargetId = "B", RelationType = "FRIEND", IsDirected = false },
                new EdgeDto { Id = "e2", SourceId = "B", TargetId = "C", RelationType = "FRIEND", IsDirected = false }
            });

            // 2. Test BFS via TraversalController
            var bfsResult = _traversalController.RunBfs("A");
            var okBfs = Assert.IsType<OkObjectResult>(bfsResult.Result);
            var bfsPath = Assert.IsAssignableFrom<IEnumerable<string>>(okBfs.Value);
            var bfsArray = bfsPath.ToArray();

            Assert.Equal(3, bfsArray.Length);
            Assert.Equal("A", bfsArray[0]);
            Assert.Contains("B", bfsArray);
            Assert.Contains("C", bfsArray);
        }

        [Fact]
        public void ChainQuery_Integration_ShouldReturnEndNodes()
        {
             // A --FRIEND--> B --ATTENDS--> E1
            _nodesController.AddNodesBatch(new List<NodeDto> {
                new NodeDto { Id = "A", Type = "User", Properties = new Dictionary<string, object> { { "Name", "A" } } },
                new NodeDto { Id = "B", Type = "User", Properties = new Dictionary<string, object> { { "Name", "B" } } },
                new NodeDto { Id = "E1", Type = "Event", Properties = new Dictionary<string, object> { { "Name", "Global AI" } } }
            });

            _nodesController.AddEdgesBatch(new List<EdgeDto> {
                new EdgeDto { Id = "e1", SourceId = "A", TargetId = "B", RelationType = "FRIEND", IsDirected = false },
                new EdgeDto { Id = "e2", SourceId = "B", TargetId = "E1", RelationType = "ATTENDS", IsDirected = true }
            });

            // Run Chain Query: User(A) -> FRIEND -> User(B) -> ATTENDS -> Event(E1)
            var chainResult = _traversalController.RunChainQuery("A", new string[] { "FRIEND", "ATTENDS" });
            var okChain = Assert.IsType<OkObjectResult>(chainResult.Result);
            var chainNodes = Assert.IsType<List<NodeDto>>(okChain.Value);

            Assert.Single(chainNodes);
            Assert.Equal("E1", chainNodes[0].Id);
        }
    }
}
