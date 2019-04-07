namespace OW.Experts.WebUI.ViewModels.Expert
{
    public class RelationViewModel
    {
        public string StraightRelationid { get; set; }
        public string ReverseRelationId { get; set; }

        public string FirstNodeNotion { get; set; }
        public string FirstNodeType { get; set; }

        public string SecondNodeNotion { get; set; }
        public string SecondNodeType { get; set; }

        public string DoesRelationExist { get; set; }

        public bool IsStraightTaxonym { get; set; }
        public bool IsStraightMeronym { get; set; }
        public bool IsReverseTaxonym { get; set; }
        public bool IsReverseMeronym { get; set; }
    }
}