using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShippingProvision.TemplateEngine;
using System.Net.Mail;
using ShippingProvision.Business;
using System.Configuration;
using System.Net.Configuration;

namespace ShippingProvision.Web.Helpers
{
    public class EmailHelper
    {
        private static string fromAddress = string.Empty;
        public static string FromAddress
        {
            get
            {
                if (string.IsNullOrEmpty(fromAddress))
                {
                    var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                    fromAddress = smtpSection.Network.UserName;
                }
                return fromAddress;
            }
        }
        public static string GetContent(string templateKey, dynamic model)
        {
            ITemplateEngine engine = TemplateEngineFactory.GetTemplateEngine();
            var parsedTemplate = engine.ParseTemplate(templateKey, model);
            return parsedTemplate;
        }

        public static MailMessage ConstructMailMessage(MailData mailData)
        {
            var message = new MailMessage();

            string from = mailData.FromAddress;
            if (string.IsNullOrEmpty(from))
            {
                from = FromAddress;
            }
            message.From = new MailAddress(from);

            if (!String.IsNullOrEmpty(mailData.ToAddress))
            {
                var tos = mailData.ToAddress.Split(new char[] { ',', ';' });
                foreach (var to in tos)
                {
                    message.To.Add(new MailAddress(to));
                }
            }
            if (!string.IsNullOrEmpty(mailData.CCAddress))
            {
                var ccs = mailData.CCAddress.Split(new char[] { ',', ';' });
                foreach (var cc in ccs)
                {
                    message.CC.Add(new MailAddress(cc));
                }
            }
            message.Subject = mailData.Subject;
            message.Body = mailData.Body;
            return message;
        }
    }
}