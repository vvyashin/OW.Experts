namespace OW.Experts.Domain
{
    /// <summary>
    /// Type of notions that represents as node of semantic network or association of experts.
    /// </summary>
    public class NotionType : TypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotionType"/> class.
        /// </summary>
        /// <param name="name">Name of type.</param>
        public NotionType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotionType"/> class.
        /// </summary>
        /// <remarks>Empty ctor can be used only by ORM.</remarks>
        protected NotionType()
        {
        }
    }
}