using System.Collections.Generic; 
using System.Text; 
using System;
using ShippingProvision.Services;
using System.Linq;

namespace ShippingProvision.Business{

    public class SalesOrder : HistoryEntity<long>
    {
        public SalesOrder() { }

        public virtual string SalesOrderIdentifier { get { return String.Format(Constants.SALES_ORDER_ID_FORMAT, this.Id); } set { } }        
        public virtual  System.Nullable<System.DateTime> SalesOrderDate { get; set; }
        public virtual  long QuoteId { get; set; }
        public virtual string QuoteIdentifier { get { return String.Format(Constants.QUOTATION_ID_FORMAT, this.QuoteId); } set { } }        
        public virtual  System.Nullable<long> ClientId { get; set; }
        public virtual  string ClientCode { get; set; }
        public virtual  string ClientName { get; set; } //TODO: Compute and save this on save/update salesorder

        public virtual  System.Nullable<long> VesselId { get; set; }
        public virtual  string VesselCode { get; set; }
        public virtual  string VesselName { get; set; } //TODO: Compute and save this on save/update salesorder   

        public virtual  int ProvisionStatus { get; set; }
        public virtual  int ProvisionType { get; set; }
        public virtual  int PayType { get; set; }
        public virtual  string PersonIncharge { get; set; } //TODO: Compute and save this on save/update salesorder
        public virtual  long PersonInchargeId { get; set; }
        public virtual  System.Nullable<System.DateTime> EstimatedDeliveryDate { get; set; }
        
        public virtual  string PurcharseOrderIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> PurcharseOrderDate { get; set; }
        public virtual  string InvoiceIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> InvoiceDate { get; set; }
        public virtual  string DeliveryIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> DeliveryDate { get; set; }
        public virtual  string BillToAddress { get; set; }

        public virtual  decimal TotalDiscount { get; set; }
        public virtual  bool IncludeGST { get; set; }
        public virtual  decimal GST { get; set; }
        public virtual bool GSTZeroRated { get; set; }

        public virtual decimal InitialTotal
        {
            get;
            set;
        }

        public virtual  decimal DiscountAmount
        {
            get
            {
                var initialTotal = this.InitialTotal;
                var discountAmount = initialTotal * this.TotalDiscount / 100;
                return discountAmount;
            }
        }

        public virtual  decimal GSTAmount
        {
            get
            {
                var gstValue = (this.IncludeGST) ? this.GST : 0;
                var discountedAmount = this.InitialTotal - this.DiscountAmount;
                var gstAmount = discountedAmount * gstValue / 100;
                return gstAmount;
            }
        }

        public virtual  decimal TotalAmount
        {
            get
            {
                var totalAmount = this.InitialTotal - this.DiscountAmount + this.GSTAmount;
                return totalAmount;
            }
        }
    }
}
