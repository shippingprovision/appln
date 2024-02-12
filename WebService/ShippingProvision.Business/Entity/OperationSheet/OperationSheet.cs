using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class OperationSheet : HistoryEntity<long>
    {
        public OperationSheet() { }

        public string ClientName { get; set; }
        public string VesselName { get; set; }
        public string VesselCode { get; set; }
        public string SalesOrderIdentifier { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string PurcharseOrderIdentifier { get; set; }        
        public string ProvisionType { get; set; }

        public long VendorCount
        {
            get
            {
                return (this.Lines != null) ? this.Lines.Count : 0;
            }
        }

        public List<OperationSheetLine> Lines { get; set; }
    }
}
