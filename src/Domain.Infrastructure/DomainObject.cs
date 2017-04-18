using System;

namespace Domain.Infrastructure
{
    /// <summary>
    /// Base type for all domain objects
    /// </summary>
    public abstract class DomainObject
    {
        private Guid _id;

        public virtual Guid Id
        {
            get { return _id; }
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!( obj is DomainObject )) return false;

            return Equals((DomainObject)obj);
        }

        protected virtual bool Equals(DomainObject obj)
        {
            return this.Id == obj.Id;
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator== (DomainObject left, DomainObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DomainObject left, DomainObject right)
        {
            return !Equals(left, right);
        }
    }
}