using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class SupplierCategoryMapMap: ClassMap<SupplierCategoryMap>
    {
        public SupplierCategoryMapMap()
        {
            Table("POS_SUPPLIER_CATEGORY_MAP");
            LazyLoad();
            CompositeId()
                .KeyProperty(x => x.CategoryId, "CATEGORY_ID")
                .KeyProperty(x => x.SupplierId, "SUPPLIER_ID");                            
        }
    }
}
