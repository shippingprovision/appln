
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
    public class PMCategoryVM
    {
        public List<PMCategory> Catlist { get; set; }
    }

    [CustomAuthorize]
    public class CategoryController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetCategorys()
        { 
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var categorys = categoryBO.GetCategorys();
            return Request.CreateResponse(HttpStatusCode.OK, categorys);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetCategorysByFilter(string name, int type)
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var categorys = categoryBO.GetCategorysByFilter(name, type);
            return Request.CreateResponse(HttpStatusCode.OK, categorys);
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddCategory(PMCategory category)
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            categoryBO.AddCategory(category);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = category.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateCategory(PMCategory pmcategory)
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            categoryBO.UpdateCategory(pmcategory);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = pmcategory.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateCategorys(PMCategoryVM pmcategoryvm)
        {
            IList<long> lsResult = null;
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            if (pmcategoryvm != null)
            {
                lsResult = categoryBO.UpdateCategorys(pmcategoryvm.Catlist);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { ids = lsResult });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteCategory(long id)
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var categoryid = categoryBO.DeleteCategory(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = categoryid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetCategory(long categoryId)
        {
            var categoryBO = BOFactory.GetBO<CategoryBO>();
            var category = categoryBO.GetById(categoryId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                category = category                
            });
        }
    }
}
