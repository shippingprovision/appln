
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;
using System.Web;
using ShippingProvision.Web.Helpers;
using System.Threading.Tasks;
using System.IO;

namespace ShippingProvision.Web.Controllers
{
    //- GetHistoryOrders()
    //   - GetHitoryOrdersByFilter(DateTime startDate, DateTime endDate, long clientId, long vesselId)
    //   - RevertSalesOrder(long id)
    //   - DeleteSalesOrder(long id)    
    //   - GetHistoryOrder(long historyOrderId)
    //   - GetHistoryOrderLines(long historyOrderId)
    [CustomAuthorize]
    public class HistoryOrderController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetHistoryOrders()
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var items = bo.GetHistoryOrders();
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetHitoryOrdersByFilter(DateTime startDate, DateTime endDate, long clientId, long vesselId, int startIndex = 0, int pageSize = 25)
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var items = bo.GetHistoryOrdersByFilter(startDate, endDate, clientId, vesselId,startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }


        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage RevertSalesOrder(long id)
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var soId = bo.RevertSalesOrder(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = soId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteSalesOrder(long id)
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var soId = bo.DeleteSalesOrder(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = soId });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetHistoryOrder(long historyOrderId)
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var item = bo.GetHistoryOrder(historyOrderId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                historyOrder = item
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetHistoryOrderLines(long historyOrderId)
        {
            var bo = BOFactory.GetBO<HistoryOrderBO>();
            var order = bo.GetHistoryOrder(historyOrderId);
            var lines = bo.GetHistoryOrderLines(historyOrderId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                HistoryOrder = order,
                Items = lines
            });
        }
    }
}
