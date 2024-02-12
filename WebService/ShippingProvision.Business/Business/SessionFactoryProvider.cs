using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Configuration;

namespace ShippingProvision.Business
{
    public static class SessionFactoryProvider
    {
        public static ISessionFactory GetSessionFactory()
        {
            var sessionFactory = Fluently.Configure()
                      .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConfigurationManager.ConnectionStrings["DB"].ConnectionString)
                      .Raw("connection.isolation", "ReadCommitted")
                      .FormatSql()
                      .ShowSql())
                      .Mappings(m =>
                      {
                          m.FluentMappings.AddFromAssemblyOf<User>();
                          m.HbmMappings.AddFromAssemblyOf<User>();
                      })
                      .BuildSessionFactory();
            return sessionFactory;
        }
    }
}
