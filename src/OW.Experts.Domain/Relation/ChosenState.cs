namespace OW.Experts.Domain
{
    public enum ChosenState
    {
        /// <summary>
        /// Marks that the expert have already made a choice what types the relation has.
        /// </summary>
        HadNotChosen = 0,

        /// <summary>
        /// Marks that the expert have not already made a choice what types the relation has.
        /// </summary>
        HadChosen = 1
    }
}