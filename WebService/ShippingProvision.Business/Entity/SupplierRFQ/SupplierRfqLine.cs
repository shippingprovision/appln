using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;


namespace ShippingProvision.Business
{
    public class SupplierRfqLine: HistoryEntity<long>
    {
        public SupplierRfqLine() { }
        
        public virtual long SupplierRfqId { get; set; }
        public virtual long QuoteLineId { get; set; }

        public virtual string SNo { get; set; }
        public virtual string Identifier { get; set; }
        public virtual System.Nullable<long> ItemId { get; set; }
        public virtual string Description { get; set; }
        public virtual string Unit { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual string Remarks { get; set; }
        public virtual bool IsIncluded { get; set; }
    }
}
