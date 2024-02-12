using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.TemplateEngine
{
    /// <summary>
    /// Template engine parse the template and merge with the data model given
    /// </summary>
    public interface ITemplateEngine
    {
        String ParseTemplate<TModel>(String templateKey, TModel model);
    }
}
