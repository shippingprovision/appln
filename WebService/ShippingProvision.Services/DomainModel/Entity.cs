using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ShippingProvision.Services
{
    [Serializable()]
    public abstract class Entity<TId>
    {
        #region IEntity

        #region Fields

        private TId _id = default(TId);
        private int _status = (int)ShippingProvision.Services.Enums.Status.Alive;

        #endregion

        #region Properties

        /// <summary>
        /// ID may be of type string, int, custom type, etc.
        /// </summary>
        /// <remarks>
        /// Setter is protected to allow unit tests to set this property via reflection and to allow 
        /// domain objects more flexibility in setting this for those objects with assigned IDs.
        /// </remarks>
        public virtual TId Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int Rev { get; set; }

        /// <summary>
        /// IsPersisted determines if object is persisted in data store (true) or is a transient object (false)
        /// </summary>
        public virtual Boolean IsPersisted
        {
            get { return this.Id.Equals(default(TId)) ? false : true; }
        }

        public virtual int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private bool isActive = true;
        public virtual bool IsActive { get { return isActive; } set { isActive = value; } }   

        #endregion

        #region Operations

        /// <summary>
        /// Must be provided to properly compare two objects
        /// </summary>
        public override int GetHashCode()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            return sb.ToString().GetHashCode();
        }

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj != null) &&                                                         // 1) Object is not null.
                   (obj.GetType() == this.GetType()) &&                                     // 2) Object is of same Type.
                   (this.MatchingIds((Entity<TId>)obj) || this.MatchingHashCodes(obj));    // 3) Ids or Hashcodes match.
        }

        /// <summary>
        /// Returns true if self and the provided persistent object have the same ID values 
        /// and the IDs are not of the default ID value
        /// </summary>
        private Boolean MatchingIds(Entity<TId> obj)
        {
            return (this.Id != null && !this.Id.Equals(default(TId))) &&    // 1a) this.Id is not null/default.
                   (obj.Id != null && !obj.Id.Equals(default(TId))) &&      // 1b) obj.Id is not null/default.
                   (this.Id.Equals(obj.Id));                                // 2) Ids match.
        }

        /// <summary>
        /// Check object business context
        /// </summary>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        private Boolean MatchingHashCodes(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());    // 1) Hashcodes match.
        }
        
        #endregion

        #endregion

        #region Constructors

        protected Entity()
        {
        }

        #endregion
    }
}
