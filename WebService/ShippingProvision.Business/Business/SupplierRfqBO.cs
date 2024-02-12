using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;

namespace ShippingProvision.Business
{
    public class SupplierRfqBO : BaseBO<SupplierRfq>
    {
        public BaseBO<SupplierRfqLine> LineBO = BOFactory.GetBO<BaseBO<SupplierRfqLine>>();

        public List<SupplierRfq> GetSupplierRFQs(long quotationId)
        {
            bool recompute = false;
            var existingRfqs = this.Items.Where(item => item.QuoteId == quotationId).ToList();
            if (existingRfqs == null ||
                existingRfqs.Count == 0)
            {
                recompute = true;
            }
            if (recompute)
            {
                var userId = 0;
                this.ExecuteStoredProcedure("POS_CREATE_SUPPLIER_RFQS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId }, {"@USER_ID", userId} });
                existingRfqs = this.Items.Where(item => item.QuoteId == quotationId).ToList();
                this.SessionEvict(existingRfqs);
            }
            return existingRfqs;
        }

        public List<SupplierRfqLine> GetSupplierRFQLines(long supplierRfqId)
        {
            var existingRfqLines = this.LineBO.Items
                                              .Where(item => item.SupplierRfqId == supplierRfqId)
                                              .ToList();
            this.SessionEvict(existingRfqLines);
            return existingRfqLines;
        }

        public void UpdateSupplierRfqs(ListRequestVM<SupplierRfq> request)
        {
            if (request != null && request.ModifiedList != null)
            {
                request.ModifiedList.ForEach(item =>
                {
                    var updated = this.GetObjectForUpdate(item.Id, item.Rev);
                    updated.Remarks = item.Remarks;
                    this.SaveOrUpdate(updated);
                });
            }
        }


        public void UpdateSupplierRfqLines(ListRequestVM<SupplierRfqLine> request)
        {
            if (request != null && request.ModifiedList != null)
            {
                request.ModifiedList.ForEach(item =>
                {
                    var updated = this.LineBO.GetObjectForUpdate(item.Id, item.Rev);
                    updated.Remarks = item.Remarks;                    
                    this.LineBO.SaveOrUpdate(updated);

                    //TODO: update corresponding quotationline remarks
                });
            }
        }

        public SupplierRfqBO() { }



    }
}
