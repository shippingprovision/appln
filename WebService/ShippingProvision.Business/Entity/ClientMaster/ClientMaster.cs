using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class ClientMaster : HistoryEntity<long>
    {
        public virtual string ClientCode { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string Marking { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual string Address { get; set; }
        public virtual string Telephone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Email { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string BillingAddress { get; set; }
    }
}
