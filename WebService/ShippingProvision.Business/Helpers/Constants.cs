using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business
{
    public class Constants
    {
        public const int STATUS_LIVE = 1;
        public const int STATUS_DELETED = 2; //TODO : Clarify

        public const string QUOTATION_ID_FORMAT = "Q_{0}";
        public const string SALES_ORDER_ID_FORMAT = "SO_{0}";
    }
}
