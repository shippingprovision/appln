using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using NHibernate;
using NHibernate.Criterion;

namespace ShippingProvision.Business
{
    public class StockItemHistoryBO : BaseBO<StockItemHistory>
    {
        public long AddStockItemHistory(StockItemHistory stockItemHistory)
        {
            stockItemHistory.Status = Constants.STATUS_LIVE;
            this.SaveOrUpdate(stockItemHistory);
            return stockItemHistory.Id;
        }

        public StockItemHistoryBO() { }
    }
}
