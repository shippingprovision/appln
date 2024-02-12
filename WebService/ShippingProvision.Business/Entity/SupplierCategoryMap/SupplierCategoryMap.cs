using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class SupplierCategoryMap : CompositeEntity
    {
        public virtual long CategoryId { get; set; }
        public virtual long SupplierId { get; set; }
        
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
            return (temp.ItemId == this.CategoryId &&
                    temp.SupplierId == this.SupplierId);
        }

        public override int CompositeGetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string CompositeToString()
        {
            return string.Format("Category:{0}, Supplier:{1}",
               CategoryId, SupplierId);
        }

        public override string GetName()
        {
            return "Supplier Category Mapping";
        }
    }
}
