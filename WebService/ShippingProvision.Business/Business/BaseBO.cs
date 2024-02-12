using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using ShippingProvision.Business.Helpers;

namespace ShippingProvision.Business
{
    public class BaseBO<TEntity> : NHibernateManagerBase<TEntity> where TEntity:class
    {
        protected void SessionEvict<TEntity>(TEntity entity)
        {
            if (entity != null)
            {
                this.Session.Evict(entity);
            }
        }

        protected void SessionEvict<TEntity>(List<TEntity> entities)
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    this.Session.Evict(entity);
                }
            }
        }

        protected override long GetUserId()
        {
            return SessionContext.UserId;
        }
 
    }
}
