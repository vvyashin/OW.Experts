namespace OW.Experts.Domain
{
    /// <summary>
    /// Type of relation that represents as edges of semantic network or association of experts
    /// </summary>
    public class RelationType : TypeBase
    {
        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        protected RelationType() { }

        /// <summary>
        /// Ctor for creating new object
        /// </summary>
        /// <param name="name">name of type</param>
        public RelationType(string name) : base(name) { }
    }
}
