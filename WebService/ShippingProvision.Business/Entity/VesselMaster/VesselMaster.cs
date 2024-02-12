using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class VesselMaster : HistoryEntity<long>
    {
        public virtual string VesselCode { get; set; }
        public virtual string VesselName { get; set; }
        public virtual long ClientId { get; set; }
        public virtual string ClientName { get; set; }
    }
}
