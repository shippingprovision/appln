using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class User : HistoryEntity<long>
    {
        public virtual int UserGroupId { get; set; }
        public virtual String LoginName { get; set; }
        public virtual String LoginPassword { get; set; }

        public virtual long CompanyId { get; set; }
        public virtual String CompanyCode { get; set; }

        public virtual string FullName { get; set; }        
        public virtual int Gender { get; set; }
        public virtual string Role { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Address { get; set; }
    }
}
