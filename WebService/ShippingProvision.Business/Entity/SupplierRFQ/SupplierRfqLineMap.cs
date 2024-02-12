using System;
using System.Collections.Generic;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{

    public class SupplierRfqLineMap : ClassMap<SupplierRfqLine>
    {
        public SupplierRfqLineMap()
        {
            Table("POS_SUPPLIER_RFQ_LINES");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("SUPPLIER_RFQ_LINE_ID");

            Map(x => x.SupplierRfqId).Column("SUPPLIER_RFQ_ID");
            Map(x => x.QuoteLineId).Column("QUOTE_LINE_ID");
            
            Map(x => x.SNo).Column("SNO");
            Map(x => x.Identifier).Column("IDENTIFIER").Length(250);
            Map(x => x.ItemId).Column("ITEM_ID");
            Map(x => x.Description).Column("DESCRIPTION");
            Map(x => x.Unit).Column("UNIT");
            Map(x => x.Quantity).Column("QUANTITY");
            Map(x => x.BuyingPrice).Column("BUYING_PRICE");
            Map(x => x.Remarks).Column("REMARKS");
            Map(x => x.IsIncluded).Column("IS_INCLUDED");

            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS");

            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
