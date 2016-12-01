namespace Domain
{
    public enum SessionPhase
    {
        MakingAssociations = 0,
        SpecifyingAssociationsTypes = 1,
        SelectingNodes = 2,
        SelectingAndSpecifyingRelations = 3,
        Ended = 10
    }
}