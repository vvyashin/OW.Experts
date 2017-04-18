namespace Domain
{
    /// <summary>
    /// Type of notions that represents as node of semantic network or association of experts
    /// </summary>
    public class NotionType : TypeBase
    {
        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        protected NotionType() { }

        /// <summary>
        /// Ctor for creating new object
        /// </summary>
        /// <param name="name">name of type</param>
        public NotionType(string name) : base(name) { }
    }
}