using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{

    public class Checklist : HistoryEntity<long>
    {
        public Checklist() { }

        public string SalesOrderIdentifier { get; set; }
        public System.Nullable<System.DateTime> SalesOrderDate { get; set; }

        public long QuoteId { get; set; }
        public string QuoteIdentifier { get; set; }

        public System.Nullable<long> ClientId { get; set; }
        public string ClientName { get; set; }
        public System.Nullable<long> VesselId { get; set; }
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

        public decimal TotalQuantity
        {
            get
            {
                decimal totalQuantity = default(decimal);
                if (this.Lines != null)
                {
                    foreach (var line in this.Lines)
                    {
                        totalQuantity += line.Quantity.GetValueOrDefault();
                    }
                }
                return totalQuantity;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = default(decimal);
                if (this.Lines != null)
                {
                    foreach (var line in this.Lines)
                    {
                        totalPrice += line.TotalPrice;
                    }
                }
                return totalPrice;
            }
        }

        public List<ChecklistLine> Lines { get; set; }
    }
}
