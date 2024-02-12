using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShippingProvision.Business
{
    public class ListResponseVM<Tvo>
    {
        public int Count { get; set; }
        public List<Tvo> Items { get; set; }
    }
}