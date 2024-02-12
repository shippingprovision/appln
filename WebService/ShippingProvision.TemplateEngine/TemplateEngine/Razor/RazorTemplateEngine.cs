using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using ShippingProvision.TemplateEngine.Configuration;
using System.Configuration;

namespace ShippingProvision.TemplateEngine.Razor
{
    /// <summary>
    /// Razor Template Engine abstracts the RazorEngine implementation
    /// - Parse the template for the given model
    /// - Supports i18n with Resources
    /// </summary>
    public class RazorTemplateEngine : ITemplateEngine
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static readonly Dictionary<String, TemplateElement> TemplateSettings;
        private static readonly TemplateResolver TemplateResolver = new TemplateResolver();
        private static readonly TemplateServiceConfiguration templateConfig = new TemplateServiceConfiguration
        {
            Resolver = TemplateResolver
        };
        private static readonly TemplateService TemplateService = new TemplateService(templateConfig);

        static RazorTemplateEngine() 
        {
            TemplateSettings = TemplateConfiguration.Config.Templates
                                                           .OfType<TemplateElement>()
                                                           .ToDictionary(k => k.Key, v => v);
        }

        /// <summary>
        /// Parse template for the given key and model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="templateKey"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ParseTemplate<TModel>(string templateKey, TModel model)
        {
            if (TemplateSettings[templateKey] == null)
            {
                Log.ErrorFormat("Template key:{0} not found", templateKey);
                return string.Empty;
            }
            if (String.IsNullOrWhiteSpace(TemplateSettings[templateKey].Path)) 
            {
                Log.ErrorFormat("Template Path for key:{0} not found", templateKey);
                return string.Empty;
            }

            string template = TemplateResolver.Resolve(TemplateSettings[templateKey].Path);            
            var parsedTemplated = TemplateService.Parse(template, model, null, templateKey);
            return parsedTemplated;
        }
    }
}
