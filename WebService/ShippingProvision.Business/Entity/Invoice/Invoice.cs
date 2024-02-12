using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;
using System.Linq;

namespace ShippingProvision.Business
{
    public class Invoice : HistoryEntity<long>
    {
        public Invoice() { }
        
        public long QuoteId { get; set; }
        public string QuoteIdentifier { get; set; }
        public string ClientName { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public string Remarks { get; set; }
        public decimal TotalDiscount { get; set; }
        public bool IncludeGST { get; set; }
        public bool GSTZeroRated { get; set; }
        public decimal GST { get; set; }

        public decimal InitialTotal
        {
            get
            {
                var initialTotal = (this.PmInvoiceLines != null) ? this.PmInvoiceLines.Sum(c => c.SellingPrice) : default(decimal);
                return initialTotal.HasValue ? initialTotal.Value : default(decimal);
            }
        }
        
        public decimal DiscountAmount
        {
            get
            {
                var initialTotal = this.InitialTotal;
                var discountAmount = initialTotal * this.TotalDiscount / 100;
                return discountAmount;
            }
        }

        public decimal GSTAmount
        {
            get
            {
                var gstValue = (this.IncludeGST) ? this.GST : 0;
                var discountedAmount = this.InitialTotal - this.DiscountAmount;
                var gstAmount = discountedAmount * gstValue / 100;
                return gstAmount;
            }
        }

        public decimal TotalAmount
        {
            get
            {
                var totalAmount = this.InitialTotal - this.DiscountAmount + this.GSTAmount;
                return totalAmount;
            }
        }

        public List<InvoiceLine> PmInvoiceLines { get; set; }

        //SALES ORDER INFO
        public System.Nullable<System.DateTime> EstimatedDeliveryDate { get; set; }
        public string SalesOrderIdentifier { get; set; }
        public System.Nullable<System.DateTime> SalesOrderDate { get; set; }
        public string PurcharseOrderIdentifier { get; set; }
        public System.Nullable<System.DateTime> PurcharseOrderDate { get; set; }
        public string InvoiceIdentifier { get; set; }
        public System.Nullable<System.DateTime> InvoiceDate { get; set; }
        public string DeliveryIdentifier { get; set; }
        public System.Nullable<System.DateTime> DeliveryDate { get; set; }
        public string BillToAddress { get; set; }
        public string ProvisionType { get; set; }
        public string PayType { get; set; }
        public string ClientBillingAddress { get; set; }

        public string TotalAmountInWords
        {
            get
            {
                return BOHelper.CurrencyInWords(TotalAmount.ToString("0.00"), " Dollars ", " Cents ");
            }
       }
       
        public string InitialTotalAmountInWords
       {
            get
            {
                return BOHelper.CurrencyInWords(InitialTotal.ToString("0.00"), " Dollars ", " Cents ");
            }
        }
    }
}
