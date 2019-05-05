namespace OW.Experts.Domain
{
    public class GroupedRelation
    {
        /// <summary>
        /// Gets percent of experts that confirmed the relation.
        /// </summary>
        public double Percent => (double)ExpertCount / TotalExpectCount;

        public Node Source { get; set; }

        public Node Destination { get; set; }

        public RelationType Type { get; set; }

        /// <summary>
        /// Gets or sets number of experts that confirmed the relation.
        /// </summary>
        public int ExpertCount { get; set; }

        /// <summary>
        /// Gets or sets total number of experts in the session.
        /// </summary>
        public int TotalExpectCount { get; set; }
    }
}