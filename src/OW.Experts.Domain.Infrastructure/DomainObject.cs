using System;

namespace OW.Experts.Domain.Infrastructure
{
    /// <summary>
    /// Base type for all domain objects.
    /// </summary>
    public abstract class DomainObject
    {
#pragma warning disable 649 // backing field _id is used for ORM.

        private Guid _id;

#pragma warning restore 649

        public virtual Guid Id
        {
            get { return _id; }
        }

        public static bool operator !=(DomainObject left, DomainObject right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(DomainObject left, DomainObject right)
        {
            return Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is DomainObject)) return false;

            return Equals((DomainObject)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        protected virtual bool Equals(DomainObject obj)
        {
            return Id == obj.Id;
        }
    }
}