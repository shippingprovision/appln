using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class ItemSupplierMapMap: ClassMap<ItemSupplierMap>
    {
        public ItemSupplierMapMap()
        {
            Table("POS_ITEM_SUPPLIER_MAP");
            LazyLoad();
            CompositeId()
                .KeyProperty(x => x.ItemId, "ITEM_ID")
                .KeyProperty(x => x.SupplierId, "SUPPLIER_ID")
                .KeyProperty(x => x.Preference, "PREFERENCE");
            Map(x => x.BuyingPrice).Column("BUYING_PRICE");
            Map(x => x.ItemRemarks).Column("ITEM_REMARKS");
        }
    }
}
