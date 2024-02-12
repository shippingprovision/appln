using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using ShippingProvision.TemplateEngine;
using ShippingProvision.TemplateEngine.Generator;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;

namespace ShippingProvision.Web.Helpers
{
    public class PdfHelper
    {
        public static HttpResponseMessage GetPDFResponse(string templateKey, dynamic model)
        {
            var pdfBytes = ApplyTemplate(templateKey, model);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(pdfBytes);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", templateKey);
            return result;
        }

        public static byte[] ApplyTemplate(string templateKey, dynamic model)
        {
            ITemplateEngine engine = TemplateEngineFactory.GetTemplateEngine();
            var parsedTemplate = engine.ParseTemplate(templateKey, model);
            var pdfBytes = PdfGenerator.GetPdf(parsedTemplate);
            return pdfBytes;
        }
    }
}