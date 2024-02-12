using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using System.Data;
using ShippingProvision.Services.Helpers;
using ShippingProvision.Business.Helpers;

namespace ShippingProvision.Business
{
    public class HistoryOrderBO : BaseBO<SalesOrder>
    {
        public BaseBO<SalesOrderLine> LineBO = BOFactory.GetBO<BaseBO<SalesOrderLine>>();
        
        public IList<SalesOrder> GetHistoryOrders()
        {
            var lsResult = this.Items
                               .Where(item => item.Status == Constants.STATUS_LIVE)                
                               .Where(item => item.ProvisionStatus == (int)SalesOrderStatus.Completed)                                
                               .ToList();
            return lsResult;
        }

        public ListResponseVM<SalesOrder> GetHistoryOrdersByFilter(DateTime? fromDate, DateTime? toDate, long clientId, long vesselId, int startIndex, int pageSize)
        {
            var queryable = this.Items
                            .Where(item => item.Status == Constants.STATUS_LIVE)
                            .Where(item => item.ProvisionStatus == (int)SalesOrderStatus.Completed);
            if (fromDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date <= toDate.Value.Date);
            }
            if (clientId > 0)
            {
                queryable = queryable.Where(item => item.ClientId == clientId);
            }
            if (vesselId > 0)
            {
                queryable = queryable.Where(item => item.VesselId == vesselId);
            }
            
            queryable = queryable.OrderByDescending(item => item.CreatedDate);

            var response = queryable.GetPagedResponse(startIndex, pageSize);
            return response;    
        }

        public long RevertSalesOrder(long historyOrderId)
        {
            var salesOrder = base.GetById(historyOrderId);
            salesOrder.ProvisionStatus = (int)SalesOrderStatus.Inprogress;
            this.SaveOrUpdate(salesOrder);
            return salesOrder.Id;
        }

        public long DeleteSalesOrder(long id)
        {
            var salesOrder = this.GetById(id);
            if (salesOrder == null)
            {
                throw new Exception("Non-existing SalesOrder.");
            }
            this.MarkAsDelete(salesOrder);
            return id;
        }

        public SalesOrder GetHistoryOrder(long historyOrderId)
        {
            SalesOrder salesOrder = base.GetById(historyOrderId);
            return salesOrder;
        }

        public IList<SalesOrderLine> GetHistoryOrderLines(long historyOrderId)
        {
            var lsResult = this.LineBO.Items
                                .Where(item => item.Status == Constants.STATUS_LIVE)
                                .Where(item => item.SalesOrderId == historyOrderId)
                                .ToList();
            return lsResult;
        }

        public HistoryOrderBO() { }
    }

}
