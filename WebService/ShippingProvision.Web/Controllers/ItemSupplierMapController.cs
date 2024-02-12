
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
    public class ItemSupplierMapVM
    {
        public List<ItemSupplierMap> Itemsuppliermap { get; set; }
    }

    [CustomAuthorize]
    public class ItemSupplierMapController : ApiController
    {    
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteItemSupplierMap(long itemId, long supplierId)
        {
            var itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            var itemSupplierMapid = itemSupplierMapBO.DeleteItemSupplierMap(itemId, supplierId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = itemSupplierMapid });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateItemSupplierMaps(ItemSupplierMapVM itemSupplierMapVM)
        {
            string result = null;
            var itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            if (itemSupplierMapVM != null)
            {
                result = itemSupplierMapBO.UpdateItemSupplierMaps(itemSupplierMapVM.Itemsuppliermap);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SaveItemSupplierMaps(ItemSupplierMapVM itemSupplierMapVM)
        {
            string result = null;
            var itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            if (itemSupplierMapVM != null)
            {
                result = itemSupplierMapBO.SaveItemSupplierMaps(itemSupplierMapVM.Itemsuppliermap);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemSupplierMapsBySupplierId(long supplierId)
        {
            var itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            var itemSupplierMap = itemSupplierMapBO.GetItemSupplierMapsBySupplierId(supplierId);

            return Request.CreateResponse(HttpStatusCode.OK, itemSupplierMap);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemSupplierMapsByItemId(long itemId)
        {
            var itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            var itemSupplierMap = itemSupplierMapBO.GetItemSupplierMapsByItemId(itemId);

            return Request.CreateResponse(HttpStatusCode.OK, itemSupplierMap);
        }
        
    }
}
