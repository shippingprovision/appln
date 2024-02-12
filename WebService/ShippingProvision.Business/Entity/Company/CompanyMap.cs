using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;


namespace ShippingProvision.Business
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Table("POS_COMPANIES");
            LazyLoad();
            Id(x => x.Id, "CompanyId").GeneratedBy.Identity();
            //Version(x => x.Rev).Column("Rev").Not.Nullable();

            Map(x => x.CompanyCode).Column("CompanyCode");
            Map(x => x.CompanyName).Column("CompanyName");
            Map(x => x.IsActive).Column("IsActive");
            Map(x => x.Status).Column("Status");

            Map(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.CreatedDate).Column("CreatedDate");
            Map(x => x.ModifiedBy).Column("ModifiedBy");
            Map(x => x.ModifiedDate).Column("ModifiedDate");
        }
    }
}
