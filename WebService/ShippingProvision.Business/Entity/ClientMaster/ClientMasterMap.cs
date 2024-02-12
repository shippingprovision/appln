using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class ClientMasterMap: ClassMap<ClientMaster>
    {
        public ClientMasterMap()
        {
            Table("POS_CLIENT_MASTER");
            LazyLoad();
            //Id(x => x.Id, "EmployeeId").GeneratedBy.Identity();            
            Id(x => x.Id).GeneratedBy.Identity().Column("CLIENT_ID");
            Map(x => x.ClientCode).Column("CLIENT_CODE").Not.Nullable().Length(100);
            Map(x => x.ClientName).Column("CLIENT_NAME").Not.Nullable().Length(500);
            Map(x => x.Marking).Column("MARKING").Not.Nullable().Length(100);
            Map(x => x.ContactPerson).Column("CONTACT_PERSON").Length(500);
            Map(x => x.Address).Column("ADDRESS").Length(1000);
            Map(x => x.Telephone).Column("TELEPHONE").Length(100);
            Map(x => x.Fax).Column("FAX").Length(100);
            Map(x => x.Email).Column("EMAIL").Length(100);
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.BillingAddress).Column("BILLING_ADDRESS").Length(1000);
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
