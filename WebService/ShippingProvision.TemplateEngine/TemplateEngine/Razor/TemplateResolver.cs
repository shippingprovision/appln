using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShippingProvision.TemplateEngine.Utilities;
using RazorEngine.Templating;

namespace ShippingProvision.TemplateEngine.Razor
{
    internal class TemplateResolver : ITemplateResolver
    {
        public string Resolve(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            string path = CommonUtils.SafeGetMapPath(name);
            return File.ReadAllText(path, System.Text.Encoding.Default);
        }
    }
}
