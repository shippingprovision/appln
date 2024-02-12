
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;
using System.Configuration;

namespace ShippingProvision.Web.Controllers
{
    [CustomAuthorize]
    public class OptionsController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Clients()
        { 
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            var clients = clientMasterBO.GetOptions();
            clients.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, clients);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage VesselsForClient(long clientId)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vessels = vesselMasterBO.GetOptions(clientId);
            vessels.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, vessels);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ItemCategories()
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var categories = categoryBO.GetOptions();
            categories.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, categories);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage TechItemCategories()
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var categories = categoryBO.GetTechOptions();
            categories.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, categories);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Suppliers()
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var suppliers = supplierMasterBO.GetOptions();
            suppliers.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage TechSuppliers()
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var suppliers = supplierMasterBO.GetTechOptions();
            suppliers.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, suppliers);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage StockItems()
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            var stockItems = stockMasterBO.GetOptions();
            stockItems.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, stockItems);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Users()
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var users = userBO.GetOptions();
            users.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Companies()
        {
            var companyBO = BOFactory.GetBO<CompanyBO>();
            var companies = companyBO.GetOptions();
            companies.Insert(0, OptionItem.Default);
            return Request.CreateResponse(HttpStatusCode.OK, companies);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Items(string searchText = "", long clientId = 0, int start = 0, int count = 100)
        {
            if(clientId == Convert.ToInt64(ConfigurationManager.AppSettings["ETAClientId"]))
            {
                var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
                var stockItems = stockMasterBO.GetOptions(searchText, start, count);
                return Request.CreateResponse(HttpStatusCode.OK, new { Items = stockItems });
            }
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var items = itemMasterBO.GetOptions(searchText, start, count);
            return Request.CreateResponse(HttpStatusCode.OK, new { Items = items });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage TechItems(string searchText = "", long clientId = 0, int start = 0, int count = 100)
        {
            if (clientId == Convert.ToInt64(ConfigurationManager.AppSettings["ETAClientId"]))
            {
                var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
                var stockItems = stockMasterBO.GetOptions(searchText, start, count);
                return Request.CreateResponse(HttpStatusCode.OK, new { Items = stockItems });
            }
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var items = itemMasterBO.GetTechOptions(searchText, start, count);
            return Request.CreateResponse(HttpStatusCode.OK, new { Items = items });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage TechItemsByCode(string searchText = "", int start = 0, int count = 100)
        {
            var itemMasterBO = BOFactory.GetBO<ItemMasterBO>();
            var items = itemMasterBO.GetTechOptionsByCode(searchText, start, count);
            return Request.CreateResponse(HttpStatusCode.OK, new { Items = items });
        }

    }
}
