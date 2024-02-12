using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class Company : HistoryEntity<long>
    {
        public virtual String CompanyCode { get; set; }
        public virtual String CompanyName { get; set; }
    }
}
