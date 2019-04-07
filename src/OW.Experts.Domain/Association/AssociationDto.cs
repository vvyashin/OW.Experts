using System;

namespace Domain
{
    public class AssociationDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Id of type chosen by the expert from list of known types
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Type offered by the expert (It's not from list of known types).
        /// </summary>
        public string OfferType { get; set; }
    }
}
