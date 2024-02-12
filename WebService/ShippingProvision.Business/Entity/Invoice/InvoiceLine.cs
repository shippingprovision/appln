using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;
using System.Linq;

namespace ShippingProvision.Business
{
    public class InvoiceLine : HistoryEntity<long>
    {
        public InvoiceLine() { }
        public string SNo { get; set; }
        public string InvoiceLineIdentifier { get; set; }
        public long QuoteLineId { get; set; }
        public long SalesOrderLineId { get; set; }
        public System.Nullable<long> ItemId { get; set; }
        public string Description { get; set; }
        public System.Nullable<decimal> Quantity { get; set; }
        public string Unit { get; set; }
        public long SupplierId { get; set; }
        public String SupplierName { get; set; }
        public System.Nullable<decimal> BuyingPrice { get; set; }
        public System.Nullable<decimal> Markup { get; set; }
        public System.Nullable<decimal> Discount { get; set; }
        public System.Nullable<decimal> UnitSellingPrice { get; set; }
        public System.Nullable<decimal> SellingPrice { get; set; }
        public string Remarks { get; set; }
    }
}
