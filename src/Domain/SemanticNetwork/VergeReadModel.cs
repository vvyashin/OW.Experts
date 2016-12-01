using System;
using JetBrains.Annotations;

namespace Domain
{
    public class VergeReadModel
    {
        public VergeReadModel([NotNull] string sourceNotion, [NotNull] string sourceType, [NotNull] string destinationNotion,
            [NotNull] string destinationType, [NotNull] string relationType, int weight)
        {
            if (sourceNotion == null) throw new ArgumentNullException(nameof(sourceNotion));
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (destinationNotion == null) throw new ArgumentNullException(nameof(destinationNotion));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (relationType == null) throw new ArgumentNullException(nameof(relationType));
            
            SourceNotion = sourceNotion;
            SourceType = sourceType;
            DestinationNotion = destinationNotion;
            DestinationType = destinationType;
            RelationType = relationType;
            Weight = weight;
        }

        public string SourceNotion { get; }

        public string SourceType { get; }

        public string DestinationNotion { get; }

        public string DestinationType { get; }

        public string RelationType { get; }

        public int Weight { get; }
    }
}
