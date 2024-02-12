using System; 
using System.Collections.Generic; 
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{   
    public class PurchaseOrderMap : ClassMap<PurchaseOrder> {
        public PurchaseOrderMap()
        {
            Table("POS_PURCHASE_ORDERS");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("PURCHASE_ORDER_ID");
            Map(x => x.SNo).Column("SNO");
            Map(x => x.PurchaseOrderIdentifier).Column("PURCHASE_ORDER_IDENTIFIER").Length(250);
            Map(x => x.PurchaseOrderMarking).Column("PURCHASE_ORDER_MARKING").Length(250);
            Map(x => x.DeliveryInstruction).Column("DELIVERY_INSTRUCTION");
            Map(x => x.Agent).Column("AGENT");
            Map(x => x.Place).Column("PLACE");
            Map(x => x.OrderStatus).Column("ORDER_STATUS");
            Map(x => x.OrderSentDate).Column("ORDER_SENT_DATE");
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.SalesOrderId).Column("SALES_ORDER_ID").Not.Nullable();
            Map(x => x.SupplierId).Column("SUPPLIER_ID").Not.Nullable();
            Map(x => x.SupplierName).Column("SUPPLIER_NAME");
            Map(x => x.POStatus).Column("PO_STATUS");
            Map(x => x.LineCount).Formula("(SELECT COUNT(1) FROM POS_PURCHASE_ORDER_LINES lines WHERE lines.STATUS_E = 1 and lines.PURCHASE_ORDER_ID = PURCHASE_ORDER_ID)");
            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();

            
        }
    }
}
