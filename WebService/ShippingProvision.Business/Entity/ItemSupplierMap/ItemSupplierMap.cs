using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class ItemSupplierMap : CompositeEntity
    {
        public virtual long ItemId { get; set; }
        public virtual long SupplierId { get; set; }
        public virtual int Preference { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual string ItemRemarks { get; set; }

        public virtual string ItemDescription { get; set; }
        public virtual string ItemUnit { get; set; }
        
        public virtual string Category { get; set; }

        private int isNew = 0;
        public virtual int IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        public override bool CompositeEquals(object obj)
        {
            var temp = obj as ItemSupplierMap;
            if (temp == null)
            {
                return false;
            }
            return (temp.ItemId == this.ItemId &&
                    temp.SupplierId == this.SupplierId &&
                    temp.Preference == this.Preference);
        }

        public override int CompositeGetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string CompositeToString()
        {
            return string.Format("Item:{0}, Supplier:{1}, Preference:{2}",
               ItemId, SupplierId, Preference);
        }

        public override string GetName()
        {
            return "Item Supplier Mapping";
        }
    }
}
