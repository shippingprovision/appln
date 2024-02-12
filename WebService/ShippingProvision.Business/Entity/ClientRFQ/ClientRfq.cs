using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;
using System.Linq;

namespace ShippingProvision.Business
{
    public class ClientRfq : HistoryEntity<long>
    {
        public ClientRfq() { }
        public virtual  string ClientRfqIdentifier { get; set; }
        public virtual  long QuoteId { get; set; }
        public virtual string QuoteIdentifier { get { return String.Format("Q_{0}", this.QuoteId); } set { } }
        public virtual  string ClientName { get; set; } 
        public virtual  string VesselName { get; set; } 
        public virtual  System.Nullable<int> RfqStatus { get; set; }
        public virtual  System.Nullable<System.DateTime> RfqSentDate { get; set; }
        
        private string _remarks = string.Empty;
        public virtual string Remarks
        {
            get { return _remarks; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks = value;
            }
        }

        public virtual  decimal TotalDiscount { get; set; }
        public virtual  bool IncludeGST { get; set; }
        public virtual  decimal GST { get; set; }
        public virtual decimal InitialTotal { get; set; }
        
        public virtual decimal DiscountAmount
        {
            get
            {
                var initialTotal = this.InitialTotal;
                var discountAmount = initialTotal * this.TotalDiscount / 100;
                return discountAmount;
            }
            set { }
        }

        public virtual decimal GSTAmount
        {
            get
            {
                var gstValue = (this.IncludeGST) ? this.GST : 0;
                var discountedAmount = this.InitialTotal - this.DiscountAmount;
                var gstAmount = discountedAmount * gstValue / 100;
                return gstAmount;
            }
            set { }
        }

        public virtual decimal TotalAmount
        {
            get
            {
                var totalAmount = this.InitialTotal - this.DiscountAmount + this.GSTAmount;
                return totalAmount;
            }
            set { }
        }

        //QUOTATION DETAILS
        public virtual  System.Nullable<System.DateTime> EstimatedDeliveryDate { get; set; }
        public virtual  string SalesOrderIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> SalesOrderDate { get; set; }
        public virtual  string PurcharseOrderIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> PurcharseOrderDate { get; set; }
        public virtual  string InvoiceIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> InvoiceDate { get; set; }
        public virtual  string DeliveryIdentifier { get; set; }
        public virtual  System.Nullable<System.DateTime> DeliveryDate { get; set; }
        public virtual  string BillToAddress { get; set; }
        public virtual  string ProvisionType { get; set; }

        //Incharge Details
        //public virtual  string PersonIncharge { get; set; }
        //public virtual  string Role { get; set; }
        //public virtual  string Email { get; set; }
        //public virtual  string Phone { get; set; }
        //public virtual  string Mobile { get; set; }
        //public virtual  string Fax { get; set; }
        //public virtual  string Address { get; set; }
    }
}
