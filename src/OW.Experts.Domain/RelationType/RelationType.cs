namespace OW.Experts.Domain
{
    /// <summary>
    /// Type of relation that represents as edges of semantic network or association of experts.
    /// </summary>
    public class RelationType : TypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationType"/> class.
        /// </summary>
        /// <param name="name">Name of type.</param>
        public RelationType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationType"/> class.
        /// </summary>
        protected RelationType()
        {
        }
    }
}