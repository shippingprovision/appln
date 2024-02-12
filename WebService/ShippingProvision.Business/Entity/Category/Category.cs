
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class PMCategory : HistoryEntity<long>
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual int Type { get; set; }
    }
}
