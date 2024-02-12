using System.Collections.Generic; 
using System.Text; 
using System;
using ShippingProvision.Services; 

namespace ShippingProvision.Business {
    
    public class PurchaseOrderLine : HistoryEntity<long> {
        public PurchaseOrderLine() { }

        public virtual  string SNo { get; set; }
        public virtual  string PurchaseOrderLineIdentifier { get; set; }
        public virtual  long PurchaseOrderId { get; set; }
        public virtual  long SalesOrderLineId { get; set; }
        public virtual  System.Nullable<long> ItemId { get; set; }
        public virtual  string Description { get; set; }
        public virtual  decimal Quantity { get; set; }
        public virtual  string Unit { get; set; }
        public virtual  decimal BuyingPrice { get; set; }
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
        bool isIncluded = true;
        public virtual  bool IsIncluded
        {
            get { return isIncluded; }
            set { isIncluded = value; }
        }
        public virtual  string Remarks { get; set; }
        public virtual  decimal TotalPrice
        {
            get
            {
                decimal totalPrice = this.Quantity * this.BuyingPrice;
                return totalPrice;
            }
        }

        public virtual int POLineStatus { get; set; }
    }
}
