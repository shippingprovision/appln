using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShippingProvision.Services;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using log4net;
using System.Reflection;

namespace ShippingProvision.Web
{
    public class HandleExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HandleExceptionFilterAttribute(){ }

        public override void OnException(HttpActionExecutedContext context)
        {
            NHibernateSessionManager.Instance.RollbackTransactionOn("DB");

            Log.ErrorFormat("Request:{0} Exception occurred. Message:{1} Stacktrace:{2}",
                context.Request.RequestUri,
                context.Exception.Message,
                context.Exception.StackTrace);  

            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception.Message);            
        }
    }
}