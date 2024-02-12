using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class OperationSheetLine : HistoryEntity<long>
    {
        public OperationSheetLine() { }

        public string SNo { get; set; }
        public long SupplierId { get; set; }
        public String SupplierName { get; set; }
        public long LineCount { get; set; }
        public string DeliveryInstruction { get; set; }
    }
}
