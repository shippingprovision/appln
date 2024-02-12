
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
    public class SupplierCategoryMapVM
    {
        public List<SupplierCategoryMap> Suppliercategorymap { get; set; }
    }

    [CustomAuthorize]
    public class SupplierCategoryMapController : ApiController
    {    
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteSupplierCategoryMap(long supplierId, long categoryId)
        {
            var supplierCategoryMapBO = BOFactory.GetBO<SupplierCategoryMapBO>();
            var supplierCategoryMapid = supplierCategoryMapBO.DeleteSupplierCategoryMap(supplierId, categoryId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = supplierCategoryMapid });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSupplierCategoryMaps(SupplierCategoryMapVM supplierCategoryMapVM)
        {
            string result = null;
            var supplierCategoryMapBO = BOFactory.GetBO<SupplierCategoryMapBO>();
            if (supplierCategoryMapVM != null)
            {
                result = supplierCategoryMapBO.UpdateSupplierCategoryMaps(supplierCategoryMapVM.Suppliercategorymap);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SaveSupplierCategoryMaps(SupplierCategoryMapVM supplierCategoryMapVM)
        {
            string result = null;
            var supplierCategoryMapBO = BOFactory.GetBO<SupplierCategoryMapBO>();
            if (supplierCategoryMapVM != null)
            {
                result = supplierCategoryMapBO.SaveSupplierCategoryMaps(supplierCategoryMapVM.Suppliercategorymap);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierCategoryMapsBySupplierId(long supplierId)
        {
            var supplierCategoryMapBO = BOFactory.GetBO<SupplierCategoryMapBO>();
            var supplierCategoryMap = supplierCategoryMapBO.GetSupplierCategoryMapsBySupplierId(supplierId);

            return Request.CreateResponse(HttpStatusCode.OK, supplierCategoryMap);
        }
        
    }
}
