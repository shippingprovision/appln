using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using ShippingProvision.Services.Enums;

namespace ShippingProvision.Business
{
    public class ClientRfqBO : BaseBO<ClientRfq>
    {
        public BaseBO<ClientRfqLine> LineBO = BOFactory.GetBO<BaseBO<ClientRfqLine>>();

        public ClientRfq GetClientRfq(long quotationId)
        {
            bool recompute = false;
            var existingRfq = this.Items.Where(item => item.QuoteId == quotationId && item.Status == 1).FirstOrDefault();
            if (existingRfq == null)
            {
                recompute = true;
            }
            if (recompute)
            {
                var userId = 0;
                this.ExecuteStoredProcedure("POS_CREATE_CLIENT_RFQ", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId }, {"@USER_ID", userId} });
                existingRfq = this.Items.Where(item => item.QuoteId == quotationId && item.Status == 1).FirstOrDefault();
                this.SessionEvict(existingRfq);
            }
            return existingRfq;
        }

        public List<ClientRfqLine> GetClientRfqLines(long clientRfqId)
        {
            var existingRfqLines = this.LineBO.Items
                                              .Where(item => item.Status == Constants.STATUS_LIVE)
                                              .Where(item => item.ClientRfqId == clientRfqId)
                                              .ToList();
            this.SessionEvict(existingRfqLines);
            existingRfqLines = new List<ClientRfqLine>(existingRfqLines.OrderBy(l => l.LineId));
            return existingRfqLines;
        }

        public void UpdateClientRfq(RequestVM<ClientRfq, ClientRfqLine> request)
        {
            UpdateClientRfq(request.Header);
            UpdateClientRfqLines(request.Lines);
            this.Session.Flush();
            UpdateQuotation(request.Header.Id);
        }

        public void UpdateClientRfq(ClientRfq item)
        {
            var updated = this.GetObjectForUpdate(item.Id, item.Rev);
            updated.TotalDiscount = item.TotalDiscount;
            updated.IncludeGST = item.IncludeGST;
            updated.GST = item.GST;
            this.SaveOrUpdate(updated);
        }

        public void UpdateClientRfqLines(ListRequestVM<ClientRfqLine> request)
        {
            if (request.ModifiedList != null)
            {
                request.ModifiedList.ForEach(item =>
                {
                    var updated = this.LineBO.GetObjectForUpdate(item.Id, item.Rev);
                    updated.Markup = item.Markup;
                    updated.Discount = item.Discount;
                    updated.UnitSellingPrice = item.UnitSellingPrice;
                    updated.SellingPrice = item.SellingPrice;                    
                    this.LineBO.SaveOrUpdate(updated);
                });
            }
        }

        private void UpdateQuotation(long clientRfqId)
        {
            long userId = 0;
            this.ExecuteStoredProcedure("POS_UPDATE_QUOTE_FROM_CLIENT_RFQ", new Dictionary<String, object>() { { "@CLIENT_RFQ_ID", clientRfqId }, { "@USER_ID", userId } });
        }


        public ClientRfqBO() { }
    }
}
