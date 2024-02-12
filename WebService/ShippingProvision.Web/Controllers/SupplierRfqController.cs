
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;
using ShippingProvision.TemplateEngine;
using ShippingProvision.TemplateEngine.Generator;
using System.IO;
using System.Net.Http.Headers;
using ShippingProvision.Web.Helpers;
using System.Net.Mail;

namespace ShippingProvision.Web.Controllers
{
    [CustomAuthorize]
    public class SupplierRfqController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierRfqs(long quotationId)
        {
            var supplierRfqBO = BOFactory.GetBO<SupplierRfqBO>();
            var rfqs = supplierRfqBO.GetSupplierRFQs(quotationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { Items = rfqs });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSupplierRfqLines(long supplierRfqId)
        {
            var supplierRfqBO = BOFactory.GetBO<SupplierRfqBO>();
            var lines = supplierRfqBO.GetSupplierRFQLines(supplierRfqId);
            return Request.CreateResponse(HttpStatusCode.OK, new { Items = lines });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSupplierRfqs(ListRequestVM<SupplierRfq> request)
        {
            var supplierRfqBO = BOFactory.GetBO<SupplierRfqBO>();
            supplierRfqBO.UpdateSupplierRfqs(request);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "SUCCESS" });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSupplierRfqLines(ListRequestVM<SupplierRfqLine> request)
        {
            var supplierRfqBO = BOFactory.GetBO<SupplierRfqBO>();
            supplierRfqBO.UpdateSupplierRfqLines(request);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "SUCCESS" });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage InitSendSupplierRFQ(long objectId)
        {
            var rfqs = BOFactory.GetBO<SupplierRfqBO>().Items;
            var suppliers = BOFactory.GetBO<SupplierMasterBO>().Items;
            var info = (from rfq in rfqs
                        from supplier in suppliers
                        where rfq.SupplierId == supplier.Id
                        where rfq.Id == objectId
                        select new
                        {
                            Name = supplier.SupplierName,
                            ContactPerson = supplier.ContactPerson,
                            EmailId = supplier.Email,
                            QuoteId = rfq.QuoteId,
                            Identifier = rfq.SupplierRfqMarking
                        }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.QuoteId);
            var model = new { Document = document, UserInfo = user };
            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = "Reg: Quotation - " + info.Identifier,
                Body = EmailHelper.GetContent("supplierrfq-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SendSupplierRFQ(MailData mailData)
        {
            var model = GetPrintSupplierRfqModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("supplierrfq", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Supplier RFQ.pdf"));

            var client = new SmtpClient();
            client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
            client.SendAsync(message, new { Id = mailData.ObjectId });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }


        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintSupplierRfq(long supplierRfqId)
        {
            var model = GetPrintSupplierRfqModel(supplierRfqId);
            return PdfHelper.GetPDFResponse("supplierrfq", model);
        }

        private dynamic GetPrintSupplierRfqModel(long supplierRfqId)
        {
            var supplierRfqBO = BOFactory.GetBO<SupplierRfqBO>();
            var header = supplierRfqBO.GetById(supplierRfqId);
            var lines = supplierRfqBO.GetSupplierRFQLines(supplierRfqId);

            var quoteId = header.QuoteId;
            var user = GetInchargeInfo(quoteId);

            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var company = soDocumentBO.GetCompanyInfoByQuoteId(header.QuoteId);

            return new { Header = header, Items = lines, UserInfo = user, CompanyInfo = company };
        }

        private dynamic GetInchargeInfo(long quoteId)
        {
            var userInfo = BOFactory.GetBO<QuotationBO>()
                                    .Items
                                    .Where(q => q.Id == quoteId)
                                    .Select(q => new { Id = q.PersonInchargeId })
                                    .FirstOrDefault();
            var user = BOFactory.GetBO<UserBO>()
                .Items
                .Where(u => u.Id == userInfo.Id)
                .Select(u => new
                {
                    PersonIncharge = u.FullName,
                    Role = u.Role,
                    Email = u.Email,
                    Phone = u.Phone,
                    Fax = u.Fax,
                    Address = u.Address,
                    Mobile = u.Mobile
                }).FirstOrDefault();

            return user;
        }
    }
}
