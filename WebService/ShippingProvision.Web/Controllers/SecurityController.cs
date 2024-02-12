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
    public class SecurityController : ApiController
    {
        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AuthenticateUser(User user)
        {
            var securityBO = BOFactory.GetBO<SecurityBO>();
            var authToken = securityBO.AuthenticateUser(user);

            if (authToken.IsAuthenicated)
            {
                System.Web.HttpContext.Current.Application[authToken.SessionId] = authToken;
            }
            var userVo = BOFactory.GetBO<UserBO>().GetById(authToken.UserId);
           
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                AuthToken = authToken,
                User = userVo
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage Logout()
        {
            System.Web.HttpContext.Current.Application.Remove(System.Web.HttpContext.Current.Request.Headers["Authorization"]);
            return Request.CreateResponse(HttpStatusCode.OK, new { });
        }
    }
}
