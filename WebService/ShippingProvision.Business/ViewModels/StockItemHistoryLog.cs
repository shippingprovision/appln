using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business.ViewModels
{
    public class StockItemHistoryLog
    {
        public int Sno { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal QtyIn { get; set; }
        public decimal QtyOut { get; set; }
        public DateTime TransDate { get; set; }
        public string TransUser { get; set; }
    }
}
