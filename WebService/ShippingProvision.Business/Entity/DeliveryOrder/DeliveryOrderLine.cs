using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class DeliveryOrderLine : HistoryEntity<long>
    {
        public DeliveryOrderLine() { }

        public string SNo { get; set; }
        public string LineIdentifier {get;set;}

        public long SalesOrderId { get; set; }
        public long SalesOrderLineId { get; set; }

        public System.Nullable<long> QuoteId { get; set; }
        public System.Nullable<long> QuoteLineId { get; set; }

        public System.Nullable<long> ItemId { get; set; }
        public string Description { get; set; }
        public string PltNo { get; set; }
        public string Unit { get; set; }
        public System.Nullable<decimal> Quantity { get; set; }
        
        public System.Nullable<decimal> SellingPrice { get; set; }
        
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }        
        public string Remarks { get; set; }        
        
    }
}
