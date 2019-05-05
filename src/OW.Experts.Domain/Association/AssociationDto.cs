using System;

namespace OW.Experts.Domain
{
    public class AssociationDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets id of type chosen by the expert from the list of known types.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets type suggested by the expert to add to the list of known types.
        /// </summary>
        public string OfferType { get; set; }
    }
}