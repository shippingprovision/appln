using System.Collections.Generic; 
using System.Text; 
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business{

    public class Quotation : HistoryEntity<long>
    {
        public Quotation() { }
        public virtual string QuoteIdentifier { get { return String.Format(Constants.QUOTATION_ID_FORMAT, this.Id); } set { } }
        public virtual  System.Nullable<long> ClientId { get; set; }
        public virtual  string ClientCode { get; set; }
        public virtual  string ClientName { get; set; }
        public virtual  System.Nullable<long> VesselId { get; set; }
        public virtual  string VesselCode { get; set; }
        public virtual  string VesselName { get; set; }        
        public virtual  int ProvisionStatus { get; set; }
        public virtual  int ProvisionType { get; set; }
        public virtual  int PayType { get; set; }
        public virtual  string PersonIncharge { get; set; }
        public virtual  long PersonInchargeId { get; set; }
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
    }
}
