using System;

namespace OW.Experts.Domain
{
    public class NodeCandidate
    {
        public string Notion { get; set; }

        public Guid TypeId { get; set; }

        public string TypeName { get; set; }

        public int ExpertCount { get; set; }

        public int TotalExpert { get; set; }

        public double ExpertPercent => (double)ExpertCount / TotalExpert;

        public bool IsSaveAsNode { get; set; } = false;
    }
}
