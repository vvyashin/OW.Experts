using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public class SemanticNetworkReadModel
    {
        public SemanticNetworkReadModel([NotNull] IReadOnlyCollection<ConceptReadModel> concepts)
        {
            if (concepts == null) throw new ArgumentNullException(nameof(concepts));

            Concepts = concepts;
        }

        [NotNull]
        public IReadOnlyCollection<ConceptReadModel> Concepts { get; }
    }
}