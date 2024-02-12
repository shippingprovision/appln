using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class ClientRfqLine: HistoryEntity<long>
    {
        public ClientRfqLine() { }
        
        public virtual  string SNo { get; set; }
        public virtual  string ClientRfqLineIdentifier { get; set; }        
        public virtual  long ClientRfqId { get; set; }
        public virtual  long QuoteLineId { get; set; }
        public virtual  System.Nullable<long> ItemId { get; set; }
        public virtual  string Description { get; set; }
        public virtual  System.Nullable<decimal> Quantity { get; set; }
        public virtual  string Unit { get; set; }
        public virtual  long SupplierId { get; set; }
        public virtual  String SupplierName { get; set; }
        public virtual  System.Nullable<decimal> BuyingPrice { get; set; }
        public virtual  System.Nullable<decimal> Markup { get; set; }
        public virtual  System.Nullable<decimal> Discount { get; set; }
        public virtual  System.Nullable<decimal> UnitSellingPrice { get; set; }
        public virtual  System.Nullable<decimal> SellingPrice { get; set; }
        public virtual  string Remarks { get; set; }
        public virtual long LineId
        {
            get
            {
                long lineId = 0;
                if (!Int64.TryParse(this.SNo, out lineId))
                {
                    lineId = this.Id;
                }
                return lineId;
            }
        }    
    }
}
