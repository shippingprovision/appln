using System; 
using System.Collections.Generic; 
using System.Text; 
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business {
    
    public class PurchaseOrderLineMap : ClassMap<PurchaseOrderLine> {

        public PurchaseOrderLineMap()
        {
            Table("POS_PURCHASE_ORDER_LINES");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("PURCHASE_ORDER_LINE_ID");
            Map(x => x.PurchaseOrderLineIdentifier).Column("PURCHASE_ORDER_LINE_IDENTIFIER").Length(250);
            Map(x => x.SNo).Column("SNO");
            Map(x => x.ItemId).Column("ITEM_ID");
            Map(x => x.Description).Column("DESCRIPTION").Length(2000);
            Map(x => x.Quantity).Column("QUANTITY");
            Map(x => x.Unit).Column("UNIT").Length(100);
            Map(x => x.BuyingPrice).Column("BUYING_PRICE");
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.IsIncluded).Column("IS_INCLUDED");
            Map(x => x.PurchaseOrderId).Column("PURCHASE_ORDER_ID").Not.Nullable();
            Map(x => x.SalesOrderLineId).Column("SALES_ORDER_LINE_ID").Not.Nullable();
            Map(x => x.POLineStatus).Column("PO_LINE_STATUS");

            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");        
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
