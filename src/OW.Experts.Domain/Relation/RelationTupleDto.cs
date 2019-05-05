using System;

namespace OW.Experts.Domain
{
    public class RelationTupleDto
    {
        /// <summary>
        /// Gets or sets relation from left node to right node.
        /// </summary>
        public Guid StraightRelationId { get; set; }

        /// <summary>
        /// Gets or sets relation from right node to left node.
        /// </summary>
        public Guid ReverseRelationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relation exists between nodes.
        /// </summary>
        public bool DoesRelationExist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether straight relation has type IsA.
        /// </summary>
        public bool IsStraightTaxonym { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether straight relation has type PartOf.
        /// </summary>
        public bool IsStraightMeronym { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reverse relation has type IsA.
        /// </summary>
        public bool IsReverseTaxonym { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reverse relation has type PartOf.
        /// </summary>
        public bool IsReverseMeronym { get; set; }
    }
}