using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain
{
    public interface IExpertCurrentSessionService : ICurrentSessionService
    {
        bool DoesExpertJoinSession([NotNull] string expertName);

        void JoinSession([NotNull] string expertName);

        void Associations([NotNull] IReadOnlyCollection<string> notions, [NotNull] string expertName);

        bool DoesExpertCompleteCurrentPhase([NotNull] string expertName);

        [NotNull]
        IReadOnlyCollection<Association> GetAssociationsByExpertName([NotNull] string expertName);

        [CanBeNull]
        Tuple<Relation, Relation> GetNextRelationByExpertName([NotNull] string expertName);

        void AssociationsTypes([NotNull] IReadOnlyCollection<AssociationDto> associations, [NotNull] string expertName);

        void Relations([NotNull] RelationTupleDto relationTupleDto, [NotNull] string expertName);

        void FinishCurrentPhase([NotNull] string expertName);
    }
}
