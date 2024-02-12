using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business.ViewModels
{
    public class StockMasterRow
    {
        public int SNo { get; set; }
        public virtual string ItemCode { get; set; }
        public virtual string Description { get; set; }
        public virtual string Unit { get; set; }        
        public virtual decimal Quantity { get; set; }
        
        public virtual string SupplierCode { get; set; }
        public virtual decimal BuyingPrice { get; set; }
        public virtual string CategoryName { get; set; }

    }
}
