using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business.Helpers
{
    public static class QueryableExtn
    {
        public static ListResponseVM<TEntity> GetPagedResponse<TEntity>(this IQueryable<TEntity> querable, int startIndex, int pageSize)
        {
            var response = new ListResponseVM<TEntity>()
            {
                Items = querable.Skip(startIndex).Take(pageSize).ToList(),
                Count = querable.Count()
            };
            return response;
        }
    }
}
