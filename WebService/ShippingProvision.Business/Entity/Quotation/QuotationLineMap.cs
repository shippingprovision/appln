using System; 
using System.Collections.Generic; 
using System.Text; 
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business {
    
    public class QuotationLineMap : ClassMap<QuotationLine> {
        
        public QuotationLineMap() {
            Table("POS_QUOTATION_LINES");
			LazyLoad();
			Id(x => x.Id).GeneratedBy.Identity().Column("Line_ID");
            Map(x => x.SNo).Column("SNO");
			Map(x => x.LineIdentifier).Column("LINE_IDENTIFIER").Length(250);
			Map(x => x.QuoteId).Column("QUOTE_ID");
			Map(x => x.ItemId).Column("ITEM_ID");
            Map(x => x.StockItemId).Column("STOCK_ITEM_ID");
			Map(x => x.Description).Column("DESCRIPTION").Length(250);
			Map(x => x.Unit).Column("UNIT").Length(100);
            Map(x => x.CategoryId).Formula("(SELECT TOP 1 ISNULL(im.CATEGORY_ID,0) FROM POS_ITEM_MASTER im WHERE im.ITEM_ID = ITEM_ID)");
			Map(x => x.Quantity).Column("QUANTITY");
			Map(x => x.SellingPrice).Column("SELLING_PRICE");
            Map(x => x.SupplierId1).Column("SUPPLIER_ID_1").Length(250);
			Map(x => x.Supplier1).Column("SUPPLIER_1").Length(250);
			Map(x => x.BuyingPrice1).Column("BUYING_PRICE_1");
            Map(x => x.Remarks1).Column("REMARKS_1");
			Map(x => x.Preferred1).Column("PREFERRED_1");
            Map(x => x.SupplierId2).Column("SUPPLIER_ID_2").Length(250);
			Map(x => x.Supplier2).Column("SUPPLIER_2").Length(250);
			Map(x => x.BuyingPrice2).Column("BUYING_PRICE_2");
            Map(x => x.Remarks2).Column("REMARKS_2");
            Map(x => x.Preferred2).Column("PREFERRED_2");
            Map(x => x.SupplierId3).Column("SUPPLIER_ID_3").Length(250);
            Map(x => x.Supplier3).Column("SUPPLIER_3").Length(250);
			Map(x => x.BuyingPrice3).Column("BUYING_PRICE_3");
            Map(x => x.Remarks3).Column("REMARKS_3");
            Map(x => x.Preferred3).Column("PREFERRED_3");
            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();

            Map(x => x.AvailableQuantity).Formula("(SELECT ISNULL(MAX(s.QTY) - SUM(b.BOOKED_QUANTITY),0) FROM POS_ITEM_BOOKINGS b INNER JOIN POS_STOCK_MASTER s on b.ITEM_ID = s.ITEM_ID where b.ITEM_ID = ITEM_ID)");
        }
    }
}
