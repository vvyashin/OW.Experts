using System;

namespace Domain
{
    public class RelationTupleDto
    {
        /// <summary>
        /// Relation from left node to right node
        /// </summary>
        public Guid StraightRelationId { get; set; }
        
        /// <summary>
        /// Relation from right node to left node
        /// </summary>
        public Guid ReverseRelationId { get; set; }

        /// <summary>
        /// Flag of existence any relations between nodes
        /// </summary>
        public bool DoesRelationExist { get; set; }
        
        /// <summary>
        /// Straight relation has type IsA
        /// </summary>
        public bool IsStraightTaxonym { get; set; }
        
        /// <summary>
        /// Straight relation has type PartOf
        /// </summary>
        public bool IsStraightMeronym { get; set; }
        
        /// <summary>
        /// Reverse relation has type IsA
        /// </summary>
        public bool IsReverseTaxonym { get; set; }
        
        /// <summary>
        /// Reverse relation has type PartOf
        /// </summary>
        public bool IsReverseMeronym { get; set; }
    }
}
