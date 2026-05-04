using System.Collections.Generic;

namespace SocialGraph.API.DTOs
{
    public class ChainStepDto
    {
        public string Relation { get; set; }
        public int Count { get; set; }
    }

    public class ChainResponseDto
    {
        public List<NodeDto> Nodes { get; set; }
        public List<ChainStepDto> Steps { get; set; }
    }
}
