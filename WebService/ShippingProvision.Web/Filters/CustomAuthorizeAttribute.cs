using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using ShippingProvision.Business;
using ShippingProvision.Business.Helpers;
using ShippingProvision.Services.Caching;

namespace ShippingProvision.Web
{
    public class CustomAuthorizeAttribute:System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string token = string.Empty;
            var authorized = base.IsAuthorized(actionContext);
            if (actionContext.Request.Headers.Authorization != null)
            {
                token = actionContext.Request.Headers.Authorization.Scheme;
            }
            if (String.IsNullOrEmpty(token))
            {
                var qs = actionContext.Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value);
                token = qs != null && qs.ContainsKey("sid") ? qs["sid"] : string.Empty;
            }
            if (!String.IsNullOrEmpty(token))
            {
                var authToken = ObjectCacheProvider.GetItem<AuthToken>(token);
                authorized = authToken != null && authToken.IsAuthenicated;
                if (authorized)
                {
                    SessionContext.SetRequestContext(authToken);
                }
            }
            return authorized;
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}