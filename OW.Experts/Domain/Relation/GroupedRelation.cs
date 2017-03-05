namespace Domain
{
    public class GroupedRelation
    {
        public Node Source { get; set; }

        public Node Destination { get; set; }

        public RelationType Type { get; set; }

        /// <summary>
        /// Number of experts that confirmed the relation
        /// </summary>
        public int ExpertCount { get; set; }

        /// <summary>
        /// Total number of experts in the session
        /// </summary>
        public int TotalExpectCount { get; set; }

        /// <summary>
        /// Percent of experts that confirmed the relation
        /// </summary>
        public double Percent => (double) ExpertCount/TotalExpectCount;
    }
}
