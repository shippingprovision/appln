using System.Collections.Generic;
using System.Text;
using System;
using ShippingProvision.Services;


namespace ShippingProvision.Business
{
    public class SupplierRfq : HistoryEntity<long>
    {
        public SupplierRfq() { }

        public virtual int SNo { get; set; }
        public virtual string Identifier { get; set; }

        public virtual long SupplierId { get; set; }
        public virtual String SupplierName { get; set; }
        public virtual long QuoteId { get; set; }
        public virtual string QuoteIdentifier { get; set; }
        public virtual string SupplierRfqMarking { get; set; }
        public virtual long LineCount { get; set; }

        public virtual System.Nullable<int> RfqStatus { get; set; }
        public virtual System.Nullable<System.DateTime> RfqSentDate { get; set; }

        private string _remarks = string.Empty;
        public virtual string Remarks
        {
            get { return _remarks; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks = value;
            }
        }

    }
}
