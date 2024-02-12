using System;
using System.Collections.Generic;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{

    public class SupplierRfqMap : ClassMap<SupplierRfq>
    {
        public SupplierRfqMap()
        {
            Table("POS_SUPPLIER_RFQs");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("SUPPLIER_RFQ_ID");

            Map(x => x.SNo).Column("SNO");
            Map(x => x.Identifier).Column("IDENTIFIER").Length(250);
            Map(x => x.SupplierId).Column("SUPPLIER_ID");
            Map(x => x.SupplierName).Column("SUPPLIER_NAME");
            Map(x => x.QuoteId).Column("QUOTE_ID");
            Map(x => x.QuoteIdentifier).Column("QUOTE_IDENTIFIER");
            Map(x => x.SupplierRfqMarking).Column("SUPPLIER_RFQ_MARKING");
            Map(x => x.RfqStatus).Column("RFQ_STATUS");
            Map(x => x.RfqSentDate).Column("RFQ_SENT_DATE");
            Map(x => x.Remarks).Column("REMARKS");

            Map(x => x.LineCount).Formula("(SELECT COUNT(1) FROM POS_SUPPLIER_RFQ_LINES lines where lines.SUPPLIER_RFQ_ID = SUPPLIER_RFQ_ID and lines.STATUS = 1)");

            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS");

            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
