using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class StockMaster : HistoryEntity<long>
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

        public virtual decimal Quantity { get; set; }
        public virtual decimal BookedQuantity { get; set; }
        
        public virtual decimal? UnitPrice { get; set; }
        public virtual DateTime? PriceUpdatedDate { get; set; }

        public virtual decimal AvailableQuantity
        {
            get { return this.Quantity - this.BookedQuantity; }
            set { }
        }

        //This property only used in JS
        public virtual decimal AddAdjustQuantity { get; set; }        
    }
}
