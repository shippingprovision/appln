using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class StockItemHistory : HistoryEntity<long>
    {
        public virtual long ItemId { get; set; }
        public virtual int TransType { get; set; }
        public virtual string TransIdentifier { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual DateTime TransDate { get; set; }
        public virtual String Remarks { get; set; }
    }
}
