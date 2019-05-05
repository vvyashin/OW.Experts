using System;

namespace OW.Experts.Domain
{
    [Flags]
    public enum ExpertFetch
    {
        /// <summary>
        /// Fetching is disabled
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Need to fetch only associations sub-aggregate.
        /// </summary>
        Associations = 0x1,

        /// <summary>
        /// Need to fetch relation relations sub-aggregate.
        /// </summary>
        Relations = 0x2
    }
}