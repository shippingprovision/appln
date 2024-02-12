using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class VesselMasterMap: ClassMap<VesselMaster>
    {
        public VesselMasterMap()
        {
            Table("POS_VESSEL_MASTER");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("VESSEL_ID");
            Map(x => x.VesselCode).Column("VESSEL_CODE").Not.Nullable().Length(100);
            Map(x => x.VesselName).Column("VESSEL_NAME").Not.Nullable().Length(500);
            Map(x => x.ClientId).Column("CLIENT_ID");
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
