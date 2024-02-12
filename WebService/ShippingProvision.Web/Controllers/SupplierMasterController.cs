
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
    public class SupplierMasterController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierMasters()
        { 
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var supplierMasters = supplierMasterBO.GetSupplierMasters();
            return Request.CreateResponse(HttpStatusCode.OK, supplierMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetStockSuppliers()
        { 
            //TODO : Get stock suppliers only
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var supplierMasters = supplierMasterBO.GetSupplierMasters();
            return Request.CreateResponse(HttpStatusCode.OK, supplierMasters);
        }
        
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierMastersByFilter(string code, string name, int supplierType)
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var supplierMasters = supplierMasterBO.GetSupplierMastersByFilter(code, name, supplierType);
            return Request.CreateResponse(HttpStatusCode.OK, supplierMasters);
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddSupplierMaster(SupplierMaster supplierMaster)
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            supplierMasterBO.AddSupplierMaster(supplierMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = supplierMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSupplierMaster(SupplierMaster supplierMaster)
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            supplierMasterBO.UpdateSupplierMaster(supplierMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = supplierMaster.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteSupplierMaster(long id)
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var supplierMasterid = supplierMasterBO.DeleteSupplierMaster(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = supplierMasterid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierMaster(long supplierMasterId)
        {
            var supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            var supplierMaster = supplierMasterBO.GetById(supplierMasterId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                supplierMaster = supplierMaster                
            });
        }
    }
}
