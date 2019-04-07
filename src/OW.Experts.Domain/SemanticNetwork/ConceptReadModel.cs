using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public class ConceptReadModel
    {
        public ConceptReadModel([NotNull] string notion, [NotNull] string typeName, 
            [NotNull] IReadOnlyCollection<VergeReadModel> incoming,
            [NotNull] IReadOnlyCollection<VergeReadModel> outgoing)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            if (outgoing == null) throw new ArgumentNullException(nameof(outgoing));
            if (notion == null) throw new ArgumentNullException(nameof(notion));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            Incoming = incoming;
            Outgoing = outgoing;
            Notion = notion;
            TypeName = typeName;
        }

        public IReadOnlyCollection<VergeReadModel> Incoming { get; }
        public IReadOnlyCollection<VergeReadModel> Outgoing { get; }

        public string Notion { get; }

        public string TypeName { get; }
    }
}
