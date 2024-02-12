using ShippingProvision.TemplateEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ShippingProvision.TemplateEngine.Configuration
{
    /// <summary>
    /// Represents a collection of templates
    /// </summary>
    /// <remarks></remarks>
    public class TemplateCollection : ConfigurationElementCollection
    {
        /// <summary> The name of the templates configuration section. </summary>
        public const string SectionName = "templates";

        /// <summary> template configuration section. </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateElement();
        }

        /// <summary> template section key is Key attribute value. </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TemplateElement)element).Key;
        }        
    }

    /// <summary> the template configuration element </summary>
    public class TemplateElement : ConfigurationElement
    {
        private const string KeyAttributeName = "key";

        /// <summary> key for the template </summary>
        [ConfigurationProperty(KeyAttributeName, IsRequired = true)]
        public string Key
        {
            get { return this[KeyAttributeName] as string; }
        }

        private const string PathAttributeName = "path";
        private string _templatePath;
        /// <summary>
        /// The SchemaPath, valid SAML assertion xsd for validation, or set empty to skip validation
        /// physical xsd file and its depends have to be placed in relative to app path
        /// </summary>
        /// <remarks>Required. </remarks>
        [ConfigurationProperty(PathAttributeName, IsRequired = true, DefaultValue = "")]
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(_templatePath))
                {
                    var templatePath = this[PathAttributeName] as string;
                    if (!string.IsNullOrEmpty(templatePath))
                    {
                        _templatePath = CommonUtils.SafeGetMapPath(templatePath);
                    }
                }
                return _templatePath;
            }
        }
    }
    
}
