using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class OptionItem
    {
        public virtual long Id { get; set; }
        public virtual string Text { get; set; }
        public virtual string Code { get; set; }

        private static OptionItem defaultOption = new OptionItem() { Id = -1, Code="", Text = "Please select..." };
        public static OptionItem Default { get { return defaultOption; } }
    }
}
