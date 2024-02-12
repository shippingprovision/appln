using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.TemplateEngine.Razor;

namespace ShippingProvision.TemplateEngine
{
    /// <summary>
    /// Factory provides the configured instance of TemplateEngine 
    /// i.e., Razor, Xslt
    /// </summary>
    public static class TemplateEngineFactory
    {
        public static ITemplateEngine GetTemplateEngine()
        {
            return new RazorTemplateEngine();
        }
    }
}
