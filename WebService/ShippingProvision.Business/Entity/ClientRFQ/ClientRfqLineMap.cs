using System;
using System.Collections.Generic;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class ClientRfqLineMap : ClassMap<ClientRfqLine>
    {
        public ClientRfqLineMap()
        {
            Table("POS_CLIENT_RFQ_LINES");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Assigned().Column("CLIENT_RFQ_LINE_ID");
            Map(x => x.SNo).Column("SNO").Not.Nullable();
            Map(x => x.ClientRfqLineIdentifier).Column("CLIENT_RFQ_LINE_IDENTIFIER").Length(250);
            Map(x => x.ClientRfqId).Column("CLIENT_RFQ_ID").Not.Nullable();
            Map(x => x.QuoteLineId).Column("QUOTE_LINE_ID").Not.Nullable();
            Map(x => x.ItemId).Column("ITEM_ID");
            Map(x => x.Description).Column("DESCRIPTION").Length(2000);
            Map(x => x.Quantity).Column("QUANTITY");
            Map(x => x.Unit).Column("UNIT").Length(100);
            Map(x => x.SupplierId).Column("SUPPLIER_ID").Not.Nullable();
            Map(x => x.SupplierName).Column("SUPPLIER_NAME");
            Map(x => x.BuyingPrice).Column("BUYING_PRICE");
            Map(x => x.Markup).Column("MARKUP");
            Map(x => x.Discount).Column("DISCOUNT");
            Map(x => x.UnitSellingPrice).Column("UNIT_SELLING_PRICE");
            Map(x => x.SellingPrice).Column("SELLING_PRICE");
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
