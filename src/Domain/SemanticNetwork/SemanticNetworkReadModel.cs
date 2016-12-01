using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain
{
    public class SemanticNetworkReadModel
    {
        [NotNull]
        public IReadOnlyCollection<ConceptReadModel> Concepts { get; }

        public SemanticNetworkReadModel([NotNull] IReadOnlyCollection<ConceptReadModel> concepts)
        {
            if (concepts == null) throw new ArgumentNullException(nameof(concepts));

            Concepts = concepts;
        }
    }
}
