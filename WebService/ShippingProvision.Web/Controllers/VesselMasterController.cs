
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
    public class VesselMasterVM
    {
        public List<VesselMaster> Vessellist { get; set; }
    }

    [CustomAuthorize]
    public class VesselMasterController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetVesselMasters()
        { 
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vesselMasters = vesselMasterBO.GetVesselMasters();
            return Request.CreateResponse(HttpStatusCode.OK, vesselMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetVesselsByClientId(long clientId)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vesselMasters = vesselMasterBO.GetVesselsByClientId(clientId);
            return Request.CreateResponse(HttpStatusCode.OK, vesselMasters);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetVesselMastersByFilter(string searchText, int clientId, int startIndex = 0, int pageSize = 30)
        { 
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vesselMasters = vesselMasterBO.GetVesselMastersByFilter(searchText, clientId, startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, vesselMasters);
        }
       

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddVesselMaster(VesselMaster vesselMaster)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            vesselMasterBO.AddVesselMaster(vesselMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = vesselMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateVesselMaster(VesselMaster vesselMaster)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            vesselMasterBO.UpdateVesselMaster(vesselMaster);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = vesselMaster.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateVesselMasters(VesselMasterVM vesselMasterVM)
        {
            List<long> result = null;
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            if (vesselMasterVM != null)
            {
                result = vesselMasterBO.UpdateVesselMasters(vesselMasterVM.Vessellist);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { ids = result });
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteVesselMaster(long id)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vesselMasterid = vesselMasterBO.DeleteVesselMaster(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = vesselMasterid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetVesselMaster(long vesselMasterId)
        {
            var vesselMasterBO = BOFactory.GetBO<VesselMasterBO>();
            var vesselMaster = vesselMasterBO.GetById(vesselMasterId);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                vesselMaster = vesselMaster                
            });
        }
    }
}
