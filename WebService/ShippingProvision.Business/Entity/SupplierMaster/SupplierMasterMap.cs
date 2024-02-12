using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class SupplierMasterMap: ClassMap<SupplierMaster>
    {
        public SupplierMasterMap()
        {
            Table("POS_SUPPLIER_MASTER");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("SUPPLIER_ID");
            Map(x => x.SupplierCode).Column("SUPPLIER_CODE").Not.Nullable().Length(100);
            Map(x => x.SupplierName).Column("SUPPLIER_NAME").Not.Nullable().Length(500);
            Map(x => x.SupplierType).Column("SUPPLIER_TYPE").Not.Nullable();
            Map(x => x.ContactPerson).Column("CONTACT_PERSON").Length(500);
            Map(x => x.Address).Column("ADDRESS").Length(1000);
            Map(x => x.Telephone1).Column("TELEPHONE1").Length(100);
            Map(x => x.Telephone2).Column("TELEPHONE2").Length(100);
            Map(x => x.Fax).Column("FAX").Length(100);
            Map(x => x.Email).Column("EMAIL").Length(100);
            Map(x => x.Ranking).Column("RANKING");
            Map(x => x.RankingRemarks).Column("RANKING_REMARKS").Length(1000);
            Map(x => x.PaymentType).Column("PAYMENT_TYPE");
            Map(x => x.CreditTerms).Column("CREDIT_TERMS").Length(1000);
            Map(x => x.CreditLimit).Column("CREDIT_LIMIT").Length(1000);
            Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.Impano).Column("IMPANO").Length(1000);

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
