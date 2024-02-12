using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class DeliveryOrder : HistoryEntity<long>
    {
        public DeliveryOrder() { }

        public string SalesOrderIdentifier { get; set; }
        public System.Nullable<System.DateTime> SalesOrderDate { get; set; }

        public long QuoteId { get; set; }
        public string QuoteIdentifier { get; set; }

        public System.Nullable<long> ClientId { get; set; }
        public string ClientName { get; set; }
        public System.Nullable<long> VesselId { get; set; }
        public string VesselCode { get; set; }    
        public string VesselName { get; set; }    

        public string ProvisionStatus { get; set; }
        public string ProvisionType { get; set; }
        public string PersonIncharge { get; set; }
        public System.Nullable<System.DateTime> EstimatedDeliveryDate { get; set; }
        
        public string PurcharseOrderIdentifier { get; set; }
        public System.Nullable<System.DateTime> PurcharseOrderDate { get; set; }
        public string InvoiceIdentifier { get; set; }
        public System.Nullable<System.DateTime> InvoiceDate { get; set; }
        public string DeliveryIdentifier { get; set; }
        public System.Nullable<System.DateTime> DeliveryDate { get; set; }
        public string BillToAddress { get; set; }

        public List<DeliveryOrderLine> Lines { get; set; }
    }
}
