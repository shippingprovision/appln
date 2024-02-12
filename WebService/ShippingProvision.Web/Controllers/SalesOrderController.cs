
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;
using System.Web;
using ShippingProvision.Web.Helpers;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;

namespace ShippingProvision.Web.Controllers
{
    [CustomAuthorize]
    public class SalesOrderController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSalesOrders()
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrders = salesOrderBO.GetSalesOrders();
            return Request.CreateResponse(HttpStatusCode.OK, salesOrders);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSalesOrdersByFilter(DateTime startDate, DateTime? endDate, long typeId, long clientId, long vesselId, int startIndex = 0, int pageSize = 25)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrders = salesOrderBO.GetSalesOrdersByFilter(startDate, endDate, typeId, clientId, vesselId,startIndex,pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, salesOrders);
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddSalesOrder(SalesOrder salesOrder)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            salesOrderBO.AddSalesOrder(salesOrder);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = salesOrder.Id });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSalesOrder(SalesOrder salesOrder)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            salesOrderBO.UpdateSalesOrder(salesOrder);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = salesOrder.Id });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage DeleteSalesOrder(long id)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrderId = salesOrderBO.DeleteSalesOrder(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = salesOrderId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage CheckInvoiceIdentifierExists(long soId, string invoiceNo)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var response = salesOrderBO.CheckInvoiceIdentifierExists(soId, invoiceNo);
            return Request.CreateResponse(HttpStatusCode.OK, new { msg = response });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSalesOrder(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrder = salesOrderBO.GetById(salesOrderId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                salesOrder = salesOrder
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetSalesOrderLines(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var lines = salesOrderBO.GetSalesOrderLines(salesOrderId);
            var salesOrder = salesOrderBO.GetById(salesOrderId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                SalesOrder = salesOrder,
                Items = lines
            });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage AddSalesOrderDetail(SalesOrderLine salesOrderLine)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var lineId = salesOrderBO.AddSalesOrderLine(salesOrderLine);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = lineId
            });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdateSalesOrderLines(ListRequestVM<SalesOrderLine> request)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();

            salesOrderBO.SaveSalesOrderLines(request);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GenerateDONumber(long id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = String.Format("{0}{1}{2}{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, id)
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GenerateInvoiceNumber(long id)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var result = salesOrderBO.GenerateInvoiceNumber(id);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = result
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GeneratePONumber(long id)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var result = salesOrderBO.GeneratePONumber(id);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = result
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage CompleteOrder(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            salesOrderBO.CompleteOrder(salesOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintInvoice(long salesOrderId)
        {
            var model = GetPrintInvoiceModel(salesOrderId);
            return PdfHelper.GetPDFResponse("invoice", model);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintInvoiceForVessel(long salesOrderId)
        {
            var model = GetPrintInvoiceModel(salesOrderId);
            return PdfHelper.GetPDFResponse("invoicefv", model);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintOperationSheet(long salesOrderId)
        {
            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var operationSheet = soDocumentBO.GetOperationSheet(salesOrderId);
            var company = soDocumentBO.GetCompanyInfo(salesOrderId);

            return PdfHelper.GetPDFResponse("operationsheet", new { OperationSheet = operationSheet, CompanyInfo = company });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintChecklist(long salesOrderId)
        {
            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var checklist = soDocumentBO.GetChecklist(salesOrderId);
            var company = soDocumentBO.GetCompanyInfo(salesOrderId);

            return PdfHelper.GetPDFResponse("checklist", new { Checklist = checklist, CompanyInfo = company });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintDeliverOrder(long salesOrderId)
        {
            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var deliveryOrder = soDocumentBO.GetDeliveryOrder(salesOrderId);
            var company = soDocumentBO.GetCompanyInfo(salesOrderId);

            return PdfHelper.GetPDFResponse("deliveryorder", new { DeliveryOrder = deliveryOrder, CompanyInfo = company });
        }

        //Mail related code
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage InitSendInvoice(long objectId)
        {
            var sos = BOFactory.GetBO<SalesOrderBO>().Items;
            var clients = BOFactory.GetBO<ClientMasterBO>().Items;

            var info = (from so in sos
                        from client in clients
                        where so.ClientId == client.Id
                        where so.Id == objectId
                        select new
                        {
                            Name = client.ClientName,
                            ContactPerson = client.ContactPerson,
                            EmailId = client.Email,
                            SoId = so.Id,
                            InchargeId = so.PersonInchargeId,
                            Identifier = so.SalesOrderIdentifier
                        }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.InchargeId);
            var model = new { Document = document, UserInfo = user };

            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = "Reg: Invoice - " + info.Identifier,
                Body = EmailHelper.GetContent("invoice-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage InitSendInvoiceForVessel(long objectId)
        {
            var sos = BOFactory.GetBO<SalesOrderBO>().Items;
            var clients = BOFactory.GetBO<ClientMasterBO>().Items;

            var info = (from so in sos
                        from client in clients
                        where so.ClientId == client.Id
                        where so.Id == objectId
                        select new
                        {
                            Name = client.ClientName,
                            ContactPerson = client.ContactPerson,
                            EmailId = client.Email,
                            SoId = so.Id,
                            InchargeId = so.PersonInchargeId,
                            Identifier = so.SalesOrderIdentifier
                        }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.InchargeId);
            var model = new { Document = document, UserInfo = user };

            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = "Reg: Invoice - " + info.Identifier,
                Body = EmailHelper.GetContent("invoicefv-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }


        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SendInvoice(MailData mailData)
        {
            var model = GetPrintInvoiceModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("invoice", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Invoice.pdf"));

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
        public HttpResponseMessage SendInvoiceForVessel(MailData mailData)
        {
            var model = GetPrintInvoiceModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("invoicefv", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Invoice.pdf"));

            var client = new SmtpClient();
            client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
            client.SendAsync(message, new { Id = mailData.ObjectId });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        private dynamic GetInchargeInfo(long inchargeId)
        {
            var user = BOFactory.GetBO<UserBO>()
                .Items
                .Where(u => u.Id == inchargeId)
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

        private dynamic GetPrintInvoiceModel(long salesOrderId)
        {
            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var invoice = soDocumentBO.GetInvoiceDocument(salesOrderId);
            var company = soDocumentBO.GetCompanyInfo(salesOrderId);

            return new { Invoice = invoice, CompanyInfo = company };
        }

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ValidateInvoice(long salesOrderId)
        {
            SalesOrderBO salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            string result = salesOrderBO.ValidateInvoice(salesOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = result
            });
        }
    }
}
