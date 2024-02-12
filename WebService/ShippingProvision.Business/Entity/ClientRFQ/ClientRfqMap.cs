using System;
using System.Collections.Generic;
using System.Text;
using ShippingProvision.Services;
using FluentNHibernate.Mapping;

namespace ShippingProvision.Business
{
    public class ClientRfqMap : ClassMap<ClientRfq>
    {
        public ClientRfqMap() {
            Table("POS_CLIENT_RFQS");
			LazyLoad();
			Id(x => x.Id).GeneratedBy.Identity().Column("CLIENT_RFQ_ID");
			Map(x => x.ClientRfqIdentifier).Column("CLIENT_RFQ_IDENTIFIER").Length(250);
			Map(x => x.QuoteId).Column("QUOTE_ID").Not.Nullable();
			Map(x => x.RfqStatus).Column("RFQ_STATUS");
			Map(x => x.RfqSentDate).Column("RFQ_SENT_DATE");
			Map(x => x.Remarks).Column("REMARKS").Length(1000);
            Map(x => x.TotalDiscount).Column("TOTAL_DISCOUNT");
            Map(x => x.IncludeGST).Column("INCLUDE_GST");
            Map(x => x.GST).Column("GST");
            Map(x => x.ClientName).Column("CLIENT_NAME");
            Map(x => x.VesselName).Column("VESSEL_NAME");
            Map(x => x.InitialTotal).Formula(@"(SELECT ISNULL(SUM(lines.SELLING_PRICE),0) 
                                                 FROM POS_CLIENT_RFQ_LINES lines
                                                 WHERE lines.CLIENT_RFQ_ID = CLIENT_RFQ_ID
                                                   AND lines.STATUS_E = 1
                                               )");

            Map(x => x.CreatedBy).Column("CREATED_BY");
			Map(x => x.CreatedDate).Column("CREATED_DATE");
			Map(x => x.ModifiedBy).Column("MODIFIED_BY");
			Map(x => x.ModifiedDate).Column("MODIFIED_DATE");
			Map(x => x.Status).Column("STATUS_E");
			Version(x => x.Rev).Column("REV").Not.Nullable();
        }
    }
}
