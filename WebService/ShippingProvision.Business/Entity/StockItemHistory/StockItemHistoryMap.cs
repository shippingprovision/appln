using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class StockItemHistoryMap: ClassMap<StockItemHistory>
    {
        public StockItemHistoryMap()
        {
            Table("POS_STOCK_ITEM_HISTORY");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("HISTORY_ID");
            Map(x => x.ItemId).Column("ITEM_ID");
            Map(x => x.TransType).Column("TRANS_TYPE");
            Map(x => x.TransIdentifier).Column("TRANS_IDENTIFIER");
            Map(x => x.TransDate).Column("TRANS_DATE");
            Map(x => x.Quantity).Column("QUANTITY");
            Map(x => x.Remarks).Column("REMARKS");
            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");            
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
