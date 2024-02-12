using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
//using System.Web.Optimization;
using System.Web.Routing;
using ShippingProvision.Business;
using ShippingProvision.Services;

namespace ShippingProvision.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            NHibernateSessionManager.Instance.SessionFactoryProvider = SessionFactoryProvider.GetSessionFactory;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.BeginTransactionOn("DB");
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.CommitTransactionOn("DB");
            NHibernateSessionManager.Instance.CloseSessionOn("DB");
        }
    }
}