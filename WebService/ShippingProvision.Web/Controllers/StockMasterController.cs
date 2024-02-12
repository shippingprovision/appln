
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
    public class StockMasterVM
    {
        public List<StockMaster> Stocklist { get; set; }
    }

    [CustomAuthorize]
    public class StockMasterController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetStockMastersByFilter(string searchText, int filterType, long categoryId)
        { 
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            var stockMasters = stockMasterBO.GetStockMastersByFilter(searchText, filterType, categoryId);
            return Request.CreateResponse(HttpStatusCode.OK, stockMasters);
        }
       

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddStockMaster(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.AddStockMaster(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateStockMaster(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.UpdateStockMaster(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateStockMasters(StockMasterVM stockMasterVM)
        {
            IList<long> result = null;
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            if (stockMasterVM != null)
            {
                result = stockMasterBO.UpdateStockMasters(stockMasterVM.Stocklist);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { ids = result });
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteStockMaster(long id)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            var stockMasterid = stockMasterBO.DeleteStockMaster(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMasterid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetStockMaster(long stockMasterId)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            var stockMaster = stockMasterBO.GetById(stockMasterId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                stockMaster = stockMaster                
            });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage CancelStock(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.CancelStockQuantity(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });            
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ReturnStock(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.ReturnStockQuantity(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddStockQuantity(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.AddStockQuantity(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AdjustStockQuantity(StockMaster stockMaster)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.AdjustStockQuantity(stockMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = stockMaster.Id });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public Task<HttpResponseMessage> Import()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string fullPath = HttpContext.Current.Server.MapPath("~/uploads");
                var streamProvider = new FileFormDataStreamProvider(fullPath);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider)
                    .ContinueWith((providerTask) => {
                        var t = providerTask;
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        var provider = t.Result;
                
                        var result = provider.FileData.Select(i =>
                        {
                            var info = new FileInfo(i.LocalFileName);
                            var manager = new ImportManager();
                            var itemsCount = manager.ImportStocksFromCsv(info);
                            return itemsCount;
                        }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Items = result
                        });    
                    });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetStockItemHistory(long itemId)
        {
            var stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            var items = stockMasterBO.GetStockItemHistory(itemId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Items = items
            });
        }

    }
}
