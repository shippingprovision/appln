using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace ShippingProvision.Business.Helpers
{
    public class SessionContext
    {
        private const string AuthTokenKey = "MVC_AUTH_TOKEN";

        public static long UserId
        {
            get
            {
                AuthToken authToken = GetContextToken();
                return (authToken != null) ? authToken.UserId : 0;
            }
        }

        public static long OrganizationId
        {
            get
            {
                AuthToken authToken = GetContextToken();
                return (authToken != null) ? authToken.OrganizationId : 0;
            }
        }


        public static void SetRequestContext(AuthToken authToken)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[AuthTokenKey] = authToken;
            }
            else {
                CallContext.SetData(AuthTokenKey, authToken);
            }
        }

        private static AuthToken GetContextToken()
        {
            AuthToken authToken = null;
            if (HttpContext.Current != null)
            {
                authToken = HttpContext.Current.Items[AuthTokenKey] as AuthToken;
            }
            else
            {
                authToken = CallContext.GetData(AuthTokenKey) as AuthToken;
            }
            return authToken;
        }

    }
}
