
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class MailData     
    {
        public virtual string FromAddress { get; set; }
        public virtual string CCAddress { get; set; }
        public virtual string ToAddress { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual string ObjectType { get; set; }
        public virtual long ObjectId { get; set; }
    }
}
