using System; 
using System.Collections.Generic; 
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{   
    public class SalesOrderMap : ClassMap<SalesOrder> {
        public SalesOrderMap()
        {
            Table("POS_SALES_ORDERS");
			LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("SALES_ORDER_ID");
            Map(x => x.SalesOrderIdentifier).Column("SALES_ORDER_IDENTIFIER").Length(250);
            Map(x => x.SalesOrderDate).Column("SALES_ORDER_DATE");
            Map(x => x.QuoteId).Column("QUOTE_ID");
            Map(x => x.ClientId).Column("CLIENT_ID");
            Map(x => x.ClientCode).Column("CLIENT_CODE").Length(100);
            Map(x => x.ClientName).Column("CLIENT_NAME");
            Map(x => x.VesselId).Column("VESSEL_ID");
            Map(x => x.VesselCode).Column("VESSEL_CODE").Length(250);
            Map(x => x.VesselName).Column("VESSEL_NAME");
            Map(x => x.ProvisionStatus).Column("PROVISION_STATUS");
            Map(x => x.ProvisionType).Column("PROVISION_TYPE");
            Map(x => x.PayType).Column("PAY_TYPE");
            Map(x => x.PersonIncharge).Column("PERSON_INCHARGE").Length(200);
            Map(x => x.PersonInchargeId).Column("PERSON_INCHARGE_ID");
            Map(x => x.EstimatedDeliveryDate).Column("ESTIMATED_DELIVERY_DATE");

            Map(x => x.PurcharseOrderIdentifier).Column("PURCHARSE_ORDER_IDENTIFIER").Length(250);
            Map(x => x.PurcharseOrderDate).Column("PURCHARSE_ORDER_DATE");
            Map(x => x.InvoiceIdentifier).Column("INVOICE_IDENTIFIER").Length(250);
            Map(x => x.InvoiceDate).Column("INVOICE_DATE");
            Map(x => x.DeliveryIdentifier).Column("DELIVERY_IDENTIFIER").Length(250);
            Map(x => x.DeliveryDate).Column("DELIVERY_DATE");
            Map(x => x.BillToAddress).Column("BILL_TO_ADDRESS").Length(1000);
            Map(x => x.InitialTotal).Formula("(select isnull(sum(lines.SELLING_PRICE),0) from POS_SALES_ORDER_LINES lines where lines.SALES_ORDER_ID = SALES_ORDER_ID)");
            Map(x => x.TotalDiscount).Column("TOTAL_DISCOUNT");
            Map(x => x.IncludeGST).Column("INCLUDE_GST");
            Map(x => x.GST).Column("GST");
            Map(x => x.GSTZeroRated).Column("GST_ZERO_RATED");

            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
