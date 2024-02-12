using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business
{
    public class AuthToken
    {
        public string SessionId { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public long OrganizationId { get; set; }
        public int UserGroupId { get; set; }
        public bool IsAuthenicated { get { return this.UserId != 0; } }
    }
}
