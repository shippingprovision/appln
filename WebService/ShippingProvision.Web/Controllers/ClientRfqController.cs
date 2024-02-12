
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
    public class ClientRfqController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetClientRfq(long quotationId)
        {
            var clientRfqBO = BOFactory.GetBO<ClientRfqBO>();
            var rfq = clientRfqBO.GetClientRfq(quotationId);
            var lines = clientRfqBO.GetClientRfqLines(rfq.Id);
            return Request.CreateResponse(HttpStatusCode.OK, new { Header = rfq, Lines = lines });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateClientRfq(RequestVM<ClientRfq, ClientRfqLine> request)
        {
            var clientRfqBO = BOFactory.GetBO<ClientRfqBO>();
            clientRfqBO.UpdateClientRfq(request);            
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "SUCCESS" });
        }
        
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintClientRfq(long clientRfqId)
        {
            var model = GetPrintClientRfqModel(clientRfqId);
            return PdfHelper.GetPDFResponse("clientrfq", model);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintClientRfqForVessel(long clientRfqId)
        {
            var model = GetPrintClientRfqModel(clientRfqId);
            return PdfHelper.GetPDFResponse("clientrfqfv", model);
        }

        private dynamic GetPrintClientRfqModel(long clientRfqId)
        {
            var clientRfqBO = BOFactory.GetBO<ClientRfqBO>();
            var header = clientRfqBO.GetById(clientRfqId);
            var lines = clientRfqBO.GetClientRfqLines(clientRfqId);

            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var company = soDocumentBO.GetCompanyInfoByQuoteId(header.QuoteId);

            return new { Header = header, Items = lines, CompanyInfo = company };
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

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage InitSendClientRFQ(long objectId)
        {
            var rfqs = BOFactory.GetBO<ClientRfqBO>().Items;
            var quotes = BOFactory.GetBO<QuotationBO>().Items;
            var clients = BOFactory.GetBO<ClientMasterBO>().Items;

            var info = (from rfq in rfqs
                         from quote in quotes
                         from client in clients
                         where quote.Id == rfq.QuoteId
                         where quote.ClientId == client.Id
                         where rfq.Id == objectId
                         select new {
                             Name = client.ClientName,
                             ContactPerson = client.ContactPerson,
                             EmailId = client.Email,
                             QuoteId = rfq.QuoteId,
                             Identifier = rfq.ClientRfqIdentifier
                         }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.QuoteId);
            var model = new { Document = document, UserInfo = user };
            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = string.Format("Reg: Quotation - Q_{0}", info.QuoteId),
                Body = EmailHelper.GetContent("clientrfq-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }
        
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage InitSendClientRFQForVessel(long objectId)
        {
            var rfqs = BOFactory.GetBO<ClientRfqBO>().Items;
            var quotes = BOFactory.GetBO<QuotationBO>().Items;
            var clients = BOFactory.GetBO<ClientMasterBO>().Items;

            var info = (from rfq in rfqs
                        from quote in quotes
                        from client in clients
                        where quote.Id == rfq.QuoteId
                        where quote.ClientId == client.Id
                        where rfq.Id == objectId
                        select new
                        {
                            Name = client.ClientName,
                            ContactPerson = client.ContactPerson,
                            EmailId = client.Email,
                            QuoteId = rfq.QuoteId,
                            Identifier = rfq.ClientRfqIdentifier
                        }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.QuoteId);
            var model = new { Document = document, UserInfo = user };
            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = string.Format("Reg: Quotation - Q_{0}", info.QuoteId),
                Body = EmailHelper.GetContent("clientrfqfv-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SendClientRFQ(MailData mailData)
        {
            var model = GetPrintClientRfqModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("clientrfq", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Client RFQ.pdf"));

            var client = new SmtpClient();
            client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
            client.SendAsync(message, new { Id = mailData.ObjectId });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SendClientRFQForVessel(MailData mailData)
        {
            var model = GetPrintClientRfqModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("clientrfqfv", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Client RFQ.pdf"));

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

    }
}
