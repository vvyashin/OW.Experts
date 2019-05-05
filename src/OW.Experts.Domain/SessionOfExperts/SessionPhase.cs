namespace OW.Experts.Domain
{
    public enum SessionPhase
    {
        /// <summary>
        /// Phase when users suggest associations for base notion.
        /// </summary>
        MakingAssociations = 0,

        /// <summary>
        /// Phase when users suggest types for their own associations.
        /// </summary>
        SpecifyingAssociationsTypes = 1,

        /// <summary>
        /// Phase when users suggest which associations are more appropriate.
        /// </summary>
        SelectingNodes = 2,

        /// <summary>
        /// Phase when users choose relations between associations.
        /// </summary>
        SelectingAndSpecifyingRelations = 3,

        /// <summary>
        /// Session is completed.
        /// </summary>
        Ended = 10
    }
}