using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class StockMasterMap: ClassMap<StockMaster>
    {
        public StockMasterMap()
        {
            Table("POS_ITEM_MASTER");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("ITEM_ID");
            Map(x => x.ParentItemId).Column("PARENT_ITEM_ID");
            Map(x => x.ItemDescription).Column("ITEM_DESCRIPTION").Not.Nullable().Length(2000);
            Map(x => x.ItemType).Column("ITEM_TYPE").Length(100);
            Map(x => x.ItemCode).Column("ITEM_CODE");
            Map(x => x.CategoryId).Column("CATEGORY_ID");
            Map(x => x.CategoryName).Column("CATEGORY_NAME");
            Map(x => x.Unit).Column("UNIT");
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.IsStockItem).Column("IS_STOCK_ITEM");
            Map(x => x.IsActive).Column("ACTIVE_INACTIVE_E");

            Map(x => x.UnitPrice).Column("UNIT_PRICE");
            Map(x => x.PriceUpdatedDate).Column("PRICE_UPDATED_DATE");
            
            Map(x => x.Quantity).Formula(@"(SELECT 
                                            ISNULL(SUM(CASE sih.TRANS_TYPE
			                                            WHEN 3 THEN -1
                                                        WHEN 6 THEN -1
			                                            ELSE 1
		                                               END * sih.QUANTITY),0)
                                            FROM POS_STOCK_ITEM_HISTORY sih
                                            WHERE sih.STATUS_E = 1 and
	                                              sih.ITEM_ID = ITEM_ID)");

            Map(x => x.BookedQuantity).Formula(@"(SELECT ISNULL(SUM(b.BOOKED_QUANTITY),0) 
                                                    FROM POS_ITEM_BOOKINGS b
                                                    WHERE b.ITEM_ID = ITEM_ID)");

            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
