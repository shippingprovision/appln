using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class CategoryMap: ClassMap<PMCategory>
    {
        public CategoryMap()
        {
            Table("POS_CATEGORY_MASTER");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("CATEGORY_ID");
            Map(x => x.Code).Column("CODE");
            Map(x => x.Name).Column("NAME");
            Map(x => x.Description).Column("DESCRIPTION");
            Map(x => x.Type).Column("TYPE");
            Map(x => x.IsActive).Column("ACTIVE_INACTIVE_E");
            Map(x => x.CreatedBy).Column("CREATED_BY");
            Map(x => x.CreatedDate).Column("CREATED_DATE");
            Map(x => x.ModifiedBy).Column("MODIFIED_BY");
            Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
            Map(x => x.Status).Column("STATUS_E");
            Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
