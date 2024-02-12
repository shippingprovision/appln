using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace ShippingProvision.TemplateEngine.Generator
{
    public class PdfGenerator
    {
        public static byte[] GetPdf(string html)
        {
            MemoryStream msOutput = new MemoryStream();
            FontFactory.RegisterDirectories();

            Document document = new Document();

            PdfWriter writer = PdfWriter.GetInstance(document, msOutput);
            writer.PageEvent = new DefaultPageEventHandler();
            document.Open();

            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);

            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            //htmlContext.setImageProvider(new AbstractImageProvider() {
            //    public String getImageRootPath() {
            //        return "src/main/resources/html/";
            //    }
            //});

            //htmlContext.setLinkProvider(new LinkProvider() {
            //    public String getLinkRoot() {
            //        return "http://tutorial.itextpdf.com/src/main/resources/html/";
            //    }
            //});

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
            var pipeline = new CssResolverPipeline(cssResolver,
                                new HtmlPipeline(htmlContext,
                                    new PdfWriterPipeline(document, writer)));
            
            XMLWorker worker = new XMLWorker(pipeline, true);
            XMLParser p = new XMLParser(worker);
            p.Parse(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(html)));
            document.Close();

            return msOutput.ToArray();
        }

        public class DefaultPageEventHandler : PdfPageEventHelper
        {
            public DefaultPageEventHandler()
            {
            }

            PdfContentByte cb;
            PdfTemplate template;
            BaseFont bf = null;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    template = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException)
                {
                }
                catch (System.IO.IOException)
                {
                }
            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
             
                int pageN = writer.PageNumber;
                String text = "Page " + pageN + " of ";
                float len = bf.GetWidthPoint(text, 8);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "", pageSize.GetRight(60), pageSize.GetBottom(30), 0);
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetRight(60) + len, pageSize.GetBottom(30));
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
                    string.Format("Printed On {0}", DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss")),
                    pageSize.GetLeft(40),
                    pageSize.GetBottom(30), 0);
                cb.EndText();
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf, 8);
                template.SetTextMatrix(0, 0);
                template.ShowText("" + (writer.PageNumber - 1));
                template.EndText();
            }
        }
    }
}
