
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
    [CustomAuthorize]
    public class QuotationController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetQuotations()
        { 
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var quotations = quotationBO.GetQuotations();
            return Request.CreateResponse(HttpStatusCode.OK, quotations);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetQuotationsByFilter(DateTime startDate, DateTime? endDate, long typeId, long clientId, long vesselId, int startIndex = 0, int pageSize = 25)
        { 
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var quotations = quotationBO.GetQuotationsByFilter(startDate, endDate, typeId, clientId, vesselId,startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, quotations);
        }
       
        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateQuotation(Quotation quotation)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            quotationBO.UpdateQuotation(quotation);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = quotation.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteQuotation(long id)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var quotationid = quotationBO.DeleteQuotation(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = quotationid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetQuotation(long quotationId)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var quotation = quotationBO.GetById(quotationId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                quotation = quotation                
            });
        }        

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetQuotationLines(long quotationId)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var lines = quotationBO.GetQuotationLines(quotationId);
            var quotation = quotationBO.GetById(quotationId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Quotation = quotation,
                Items = lines
            });
        }

        
        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddQuotationDetail(QuotationLine quotationLine)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var lineId = quotationBO.AddQuotationLine(quotationLine);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = lineId
            });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateQuotationLines(ListRequestVM<QuotationLine> request)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();

            quotationBO.SaveQuotationLines(request);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }
        
        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public Task<HttpResponseMessage> AddQuotation()
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
                        
                        var quotation = new Quotation()
                        {
                            ClientId = Convert.ToInt64(provider.FormData["clientid"]),
                            VesselId = Convert.ToInt64(provider.FormData["vesselid"]),
                            PersonInchargeId = Convert.ToInt64(provider.FormData["personinchargeid"]),
                            ProvisionType = Convert.ToInt32(provider.FormData["provisiontype"]),
                            PayType = Convert.ToInt32(provider.FormData["paytype"]),
                            ProvisionStatus = Convert.ToInt32(provider.FormData["provisionstatus"])                            
                        };

                        DateTime estimatedDeliveryDate;
                        if(DateTime.TryParse(provider.FormData["estimateddeliverydate"], out estimatedDeliveryDate))
                        {
                            quotation.EstimatedDeliveryDate = estimatedDeliveryDate; 
                        }

                        var info = provider.FileData.Select(i => new FileInfo(i.LocalFileName)).FirstOrDefault();                        
                        var manager = new ImportManager();
                        var quotationId = manager.ImportQuotationFromCsv(quotation, info);

                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            ID = quotationId
                        });    
                    });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public Task<HttpResponseMessage> AddSalesOrder()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string fullPath = HttpContext.Current.Server.MapPath("~/uploads");
                var streamProvider = new FileFormDataStreamProvider(fullPath);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider)
                    .ContinueWith((providerTask) =>
                    {
                        var t = providerTask;
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);

                        var provider = t.Result;

                        var quotation = new Quotation()
                        {
                            ClientId = Convert.ToInt64(provider.FormData["clientid"]),
                            VesselId = Convert.ToInt64(provider.FormData["vesselid"]),
                            PersonInchargeId = Convert.ToInt64(provider.FormData["personinchargeid"]),
                            ProvisionType = Convert.ToInt32(provider.FormData["provisiontype"]),
                            PayType = Convert.ToInt32(provider.FormData["paytype"]),
                            ProvisionStatus = Convert.ToInt32(provider.FormData["provisionstatus"])
                        };

                        DateTime estimatedDeliveryDate;
                        if (DateTime.TryParse(provider.FormData["estimateddeliverydate"], out estimatedDeliveryDate))
                        {
                            quotation.EstimatedDeliveryDate = estimatedDeliveryDate;
                        }

                        var info = provider.FileData.Select(i => new FileInfo(i.LocalFileName)).FirstOrDefault();
                        var manager = new ImportManager();
                        var quotationId = manager.ImportSalesOrderFromCsv(quotation, info);

                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            ID = quotationId
                        });
                    });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public Task<HttpResponseMessage> AddTechnicalStores()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string fullPath = HttpContext.Current.Server.MapPath("~/uploads");
                var streamProvider = new FileFormDataStreamProvider(fullPath);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider)
                    .ContinueWith((providerTask) =>
                    {
                        var t = providerTask;
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);

                        var provider = t.Result;

                        var quotation = new Quotation()
                        {
                            ClientId = Convert.ToInt64(provider.FormData["clientid"]),
                            VesselId = Convert.ToInt64(provider.FormData["vesselid"]),
                            PersonInchargeId = Convert.ToInt64(provider.FormData["personinchargeid"]),
                            ProvisionType = Convert.ToInt32(provider.FormData["provisiontype"]),
                            PayType = Convert.ToInt32(provider.FormData["paytype"]),
                            ProvisionStatus = Convert.ToInt32(provider.FormData["provisionstatus"])
                        };

                        DateTime estimatedDeliveryDate;
                        if (DateTime.TryParse(provider.FormData["estimateddeliverydate"], out estimatedDeliveryDate))
                        {
                            quotation.EstimatedDeliveryDate = estimatedDeliveryDate;
                        }

                        var info = provider.FileData.Select(i => new FileInfo(i.LocalFileName)).FirstOrDefault();
                        var manager = new ImportManager();
                        var quotationId = manager.ImportStoreOrderFromCsv(quotation, info);

                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            ID = quotationId
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
        public HttpResponseMessage GetSalesOrder(long quotationId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrderId = salesOrderBO.GetSalesOrder(quotationId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = quotationId
            });
        }
    }
}
