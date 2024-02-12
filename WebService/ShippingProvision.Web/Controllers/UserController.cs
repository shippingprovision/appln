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
    public class UserController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetUsers()
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var users = userBO.GetUsers();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage RegisterUser(User user)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            userBO.RegisterUser(user);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = user.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateUser(User user)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            userBO.UpdateUser(user);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = user.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteUser(long id)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var userid = userBO.DeleteUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = userid });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetUser(long userId)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var user = userBO.GetById(userId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                user = user
            });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ResetPassword(long userId, string password)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var userid = userBO.ResetPassword(userId, password);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = userid });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ChangePassword(long userId, string oldpassword, string newpassword)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var userid = userBO.ChangePassword(userId, oldpassword, newpassword);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = userid });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateMyProfile(User user)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            userBO.UpdateMyProfile(user);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = user.Id });
        }
    }
}
