using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;


namespace ShippingProvision.Business
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("POS_USERS");
            LazyLoad();
            Id(x => x.Id, "UserId").GeneratedBy.Identity();
            //Version(x => x.Rev).Column("Rev").Not.Nullable();

            Map(x => x.UserGroupId).Column("UserGroupId");
            Map(x => x.LoginName).Column("LoginName").Not.Nullable();
            Map(x => x.LoginPassword).Column("LoginPassword");
            Map(x => x.IsActive).Column("IsActive");
            Map(x => x.Status).Column("Status");

            Map(x => x.CompanyCode).Column("CompanyCode");
            Map(x => x.CompanyId).Column("CompanyId");

            Map(x => x.FullName).Column("FullName");
            Map(x => x.Gender).Column("Gender");
            Map(x => x.Role).Column("Role");
            Map(x => x.Email).Column("Email");
            Map(x => x.Phone).Column("Phone");
            Map(x => x.Mobile).Column("Mobile");
            Map(x => x.Fax).Column("Fax");
            Map(x => x.Address).Column("Address");

            Map(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.CreatedDate).Column("CreatedDate");
            Map(x => x.ModifiedBy).Column("ModifiedBy");
            Map(x => x.ModifiedDate).Column("ModifiedDate");
        }
    }
}
