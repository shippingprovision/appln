
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class ItemMaster : HistoryEntity<long>
    {
        public virtual System.Nullable<long> ParentItemId { get; set; }
        public virtual string ItemDescription { get; set; }
        public virtual int ItemType { get; set; }
        public virtual string ItemCode { get; set; }
        public virtual long? CategoryId { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual string Unit { get; set; }
        public virtual string Remarks { get; set; }
        public virtual bool IsStockItem { get; set; }
    }

    public class BondedItem 
    {
        public virtual long Id { get; set; }
        public virtual string ItemDescription { get; set; }
        public virtual int ItemType { get; set; }
        public virtual string ItemCode { get; set; }
        public virtual long? CategoryId { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual string Unit { get; set; }
        public virtual string Remarks { get; set; }
        public virtual bool IsStockItem { get; set; }    
    
        public virtual long SupplierId { get; set; }
        public virtual int Preference { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual string ItemRemarks { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual List<ItemSupplierMap> Suppliers { get; set; }
    }
}
