
using System.Configuration;

namespace ShippingProvision.TemplateEngine.Configuration
{
    /// <summary>
    /// The configuration for this component.
    /// </summary>
    public class TemplateConfiguration : ConfigurationSection
    {
        private const string SectionName = "templates.config";
        
        /// <summary>
        /// The instance of the configuration read from the application (or web) configuration.
        /// </summary>
        public static TemplateConfiguration Config
        {
            get { return ConfigurationManager.GetSection(SectionName) as TemplateConfiguration; }
        }

       
        /// <summary>
        /// Gets collection of templates
        /// </summary>
        /// <value>List of templates.</value>
        [ConfigurationProperty(TemplateCollection.SectionName, IsRequired = false)]
        [ConfigurationCollection(typeof(TemplateCollection), AddItemName = "template")]
        public TemplateCollection Templates
        {
            get { return this[TemplateCollection.SectionName] as TemplateCollection; }
        }
    }
}
