
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;

namespace ShippingProvision.Web.Controllers
{
    [CustomAuthorize]
    public class ItemMasterController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemMasters()
        { 
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasters = itemMasterBO.GetItemMasters();
            return Request.CreateResponse(HttpStatusCode.OK, itemMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemMastersByCategoryId(long categoryId)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasters = itemMasterBO.GetItemMastersByCategoryId(categoryId);
            return Request.CreateResponse(HttpStatusCode.OK, itemMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SearchItems(string searchText)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasters = itemMasterBO.GetItemMastersBySearchText(searchText);
            return Request.CreateResponse(HttpStatusCode.OK, itemMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemMastersByFilter(int itemType, long categoryId, string itemDesc, string itemCode, int start=0, int count=100)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasters = itemMasterBO.GetItemMastersByFilter(itemType, categoryId, itemDesc, itemCode, start, count);
            return Request.CreateResponse(HttpStatusCode.OK, itemMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetBondedItems(long categoryId, long supplierId)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var bondedItems = itemMasterBO.GetBondedItems(categoryId, supplierId);
            return Request.CreateResponse(HttpStatusCode.OK, bondedItems);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemMastersForMap(long categoryId, long supplierId)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasters = itemMasterBO.GetItemMastersForMap(categoryId, supplierId);
            return Request.CreateResponse(HttpStatusCode.OK, itemMasters);
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddItemMaster(ItemMaster itemMaster)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            itemMasterBO.AddItemMaster(itemMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = itemMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateItemMaster(ItemMaster itemMaster)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            itemMasterBO.UpdateItemMaster(itemMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = itemMaster.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteItemMaster(long id)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMasterid = itemMasterBO.DeleteItemMaster(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = itemMasterid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetItemMaster(long itemMasterId)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var itemMaster = itemMasterBO.GetById(itemMasterId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                itemMaster = itemMaster                
            });
        }
    }
}
