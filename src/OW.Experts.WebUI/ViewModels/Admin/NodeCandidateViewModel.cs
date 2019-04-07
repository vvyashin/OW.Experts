using System.Collections.Generic;

namespace OW.Experts.WebUI.ViewModels.Admin
{
    public class NodeCandidateListViewModel
    {
        public List<NodeCandidateViewModel> NodeCandidates { get; set; }
    }

    public class NodeCandidateViewModel
    {
        public string Notion { get; set; }
        public string TypeName { get; set; }
        public string TypeId { get; set; }
        public int ExpertCount { get; set; }
        public bool IsSaveAsNode { get; set; }
        public int TotalExpert { get; set; }

        public double ExpertPercent => (double) ExpertCount/TotalExpert;
    }
}