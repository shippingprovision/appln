
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
    public class ClientMasterController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetClientMasters()
        { 
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            var clientMasters = clientMasterBO.GetClientMasters();
            return Request.CreateResponse(HttpStatusCode.OK, clientMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetClientMastersByFilter(string name, int startIndex = 0, int pageSize = 25)
        {
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            var clientMasters = clientMasterBO.GetClientMastersByFilter(name, startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, clientMasters);
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddClientMaster(ClientMaster clientMaster)
        {
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            clientMasterBO.AddClientMaster(clientMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = clientMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateClientMaster(ClientMaster clientMaster)
        {
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            clientMasterBO.UpdateClientMaster(clientMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = clientMaster.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteClientMaster(long id)
        {
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            var clientMasterid = clientMasterBO.DeleteClientMaster(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = clientMasterid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetClientMaster(long clientMasterId)
        {
            var clientMasterBO = BOFactory.GetBO<ClientMasterBO>();
            var clientMaster = clientMasterBO.GetById(clientMasterId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                clientMaster = clientMaster                
            });
        }
    }
}
