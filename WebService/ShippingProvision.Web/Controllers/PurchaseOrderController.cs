
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
    public class PurchaseOrderController : ApiController
    {
        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetPurchaseOrders(long salesOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            var purchaseOrders = purchaseOrderBO.GetPurchaseOrders(salesOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, purchaseOrders);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetPurchaseOrdersByFilter(long supplierId, string poIdentifier, DateTime? startDate, DateTime? endDate, int startIndex = 0, int pageSize = 25)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            var purchaseOrders = purchaseOrderBO.GetPurchaseOrdersByFilter(supplierId, poIdentifier, startDate, endDate, startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, purchaseOrders);
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetPurchaseOrdersByClient(long clientId, string poIdentifier, DateTime? startDate, DateTime? endDate, int startIndex = 0, int pageSize = 25)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            var purchaseOrders = purchaseOrderBO.GetPurchaseOrdersByClient(clientId, poIdentifier, startDate, endDate, startIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, purchaseOrders);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage CancelPurchaseOrder(long purchaseOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.CancelPurchaseOrder(purchaseOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = purchaseOrderId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ReceivePurchaseOrder(long purchaseOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.ReceivePurchaseOrder(purchaseOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = purchaseOrderId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage ReceivePurchaseOrderLine(long purchaseOrderLineId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.ReceivePurchaseOrderLine(purchaseOrderLineId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = purchaseOrderLineId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage IssuePurchaseOrder(long purchaseOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.IssuePurchaseOrder(purchaseOrderId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = purchaseOrderId });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage IssuePurchaseOrderLineItem(long lineItemId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.IssuePurchaseOrderLine(lineItemId);
            return Request.CreateResponse(HttpStatusCode.OK, new { ID = lineItemId });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdatePurchaseOrders(ListRequestVM<PurchaseOrder> request)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.UpdatePurchaseOrders(request);
            return Request.CreateResponse(HttpStatusCode.OK, new { Result = "SUCCESS" });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage GetPurchaseOrderLines(long purchaseOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            var purchaseOrder = purchaseOrderBO.GetById(purchaseOrderId);
            var lines = purchaseOrderBO.GetPurchaseOrderLines(purchaseOrderId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                PurchaseOrder = purchaseOrder,
                Items = lines
            });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage UpdatePurchaseOrderLines(ListRequestVM<PurchaseOrderLine> request)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            purchaseOrderBO.SavePurchaseOrderLines(request);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Result = "SUCCESS"
            });
        }

        [System.Web.Http.HttpGetAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage PrintSupplierPO(long purchaseOrderId)
        {
            var model = GetPrintSupplierPOModel(purchaseOrderId);
            return PdfHelper.GetPDFResponse("supplierpo", model);
        }

        private dynamic GetPrintSupplierPOModel(long purchaseOrderId)
        {
            var purchaseOrderBO = BOFactory.GetBO<PurchaseOrderBO>();
            var purchaseOrder = purchaseOrderBO.GetById(purchaseOrderId);
            purchaseOrder.PmPurchaseOrderLines = purchaseOrderBO.GetPurchaseOrderLines(purchaseOrderId);

            var personIncharge = BOFactory.GetBO<SalesOrderBO>().Items
                .Where(so => so.Id == purchaseOrder.SalesOrderId)
                .Select(so => new { PId = so.PersonInchargeId, PName = so.PersonIncharge, VesselId = so.VesselId })
                .FirstOrDefault();
            if (personIncharge != null)
            {
                purchaseOrder.PersonIncharge = personIncharge.PName;
                purchaseOrder.VesselCode = BOHelper.GetVesselCodeById(personIncharge.VesselId.Value);             
            }
            var user = GetInchargeInfo(purchaseOrder.SalesOrderId);

            var soDocumentBO = BOFactory.GetBO<SODocumentBO>();
            var company = soDocumentBO.GetCompanyInfo(purchaseOrder.SalesOrderId);

            var model = new { PurchaseOrder = purchaseOrder, UserInfo = user, CompanyInfo = company };
            return model;
        }

        private dynamic GetInchargeInfo(long salesOrderId)
        {
            var userInfo = BOFactory.GetBO<SalesOrderBO>()
                                    .Items
                                    .Where(so => so.Id == salesOrderId)
                                    .Select(so => new { Id = so.PersonInchargeId })
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
        public HttpResponseMessage InitSendSupplierPO(long objectId)
        {
            var pos = BOFactory.GetBO<PurchaseOrderBO>().Items;
            var suppliers = BOFactory.GetBO<SupplierMasterBO>().Items;
            var info = (from po in pos
                        from supplier in suppliers
                        where po.SupplierId == supplier.Id
                        where po.Id == objectId
                        select new
                        {
                            Name = supplier.SupplierName,
                            ContactPerson = supplier.ContactPerson,
                            EmailId = supplier.Email,
                            SoId = po.SalesOrderId,
                            Identifier = po.PurchaseOrderIdentifier
                        }).FirstOrDefault();

            var document = new { Recipient = string.IsNullOrEmpty(info.ContactPerson) ? info.Name : info.ContactPerson };
            var user = GetInchargeInfo(info.SoId);
            var model = new { Document = document, UserInfo = user };
            var mailData = new MailData()
            {
                ToAddress = info.EmailId,
                Subject = "Reg: Purchase Order - " + info.Identifier,
                Body = EmailHelper.GetContent("supplierpo-mail", model),
                CCAddress = user.Email
            };
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MailData = mailData
            });
        }

        [System.Web.Http.HttpPostAttribute]
        [System.Web.Http.HttpOptionsAttribute]
        public HttpResponseMessage SendSupplierPO(MailData mailData)
        {
            var model = GetPrintSupplierPOModel(mailData.ObjectId);
            var pdfBytes = PdfHelper.ApplyTemplate("supplierpo", model);

            var message = EmailHelper.ConstructMailMessage(mailData);
            message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Purchase Order.pdf"));

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
