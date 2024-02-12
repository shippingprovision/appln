using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ShippingProvision.Services
{
    [Serializable()]
    public abstract class HistoryEntity<TId> : Entity<TId>
    {  
        public virtual long CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual long ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        #region Constructors

        protected HistoryEntity()
        {
        }

        #endregion
    }
}
