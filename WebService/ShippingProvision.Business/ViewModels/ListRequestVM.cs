using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShippingProvision.Business
{
    public class ListRequestVM<Tvo>
    {
        public List<Tvo> AddedList { get; set; }
        public List<Tvo> ModifiedList { get; set; }
        public List<long> DeletedList { get; set; }
    }

    public class RequestVM<THeader,TLine>
    {
        public THeader Header { get; set; }
        public ListRequestVM<TLine> Lines { get; set; }
    }
}