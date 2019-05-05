using System.Collections.Generic;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public interface IRelationRepository
    {
        /// <summary>
        /// Gets relations grouped by source and destination nodes and type.
        /// </summary>
        /// <param name="sessionOfExperts">Session of experts.</param>
        /// <returns>Read only collection of grouped relations.</returns>
        [NotNull]
        IReadOnlyCollection<GroupedRelation> GetGroupedRelations([NotNull] SessionOfExperts sessionOfExperts);
    }
}