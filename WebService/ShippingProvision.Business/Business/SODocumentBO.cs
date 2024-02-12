using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using System.Data;
using ShippingProvision.Services.Helpers;

namespace ShippingProvision.Business
{    
    public class SODocumentBO : BaseBO<DocumentPrint>         
    {    
        #region Invoice Document

        public Invoice GetInvoiceDocument(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrder = salesOrderBO.GetById(salesOrderId);
            var salesOrderLines = salesOrderBO.GetSalesOrderLines(salesOrderId);
            
            long quoteId = salesOrder.QuoteId;                        

            Invoice invoice = null;            
            if (salesOrder != null)
            {
                invoice = GetInvoice(salesOrder);
                if (salesOrderLines != null)
                {
                    invoice.PmInvoiceLines = new List<InvoiceLine>();
                    foreach (var soline in salesOrderLines)
                    {
                        var invoiceLine = GetInvoiceLine(soline);
                        invoice.PmInvoiceLines.Add(invoiceLine);
                    }
                }
            }
            if (invoice != null)
            {
                SetSalesOrderDetails(salesOrderId, invoice);
            }
            return invoice;
        }

        public Company GetCompanyInfo(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrder = salesOrderBO.GetById(salesOrderId);

            var userBO = BOFactory.GetBO<UserBO>();
            var user = userBO.GetById(salesOrder.PersonInchargeId);
            var company = new Company() { CompanyCode = user.CompanyCode };
            return company;
        }

        public Company GetCompanyInfoByQuoteId(long quoteId)
        {
            var quotationBO = BOFactory.GetBO<QuotationBO>();
            var quotation = quotationBO.GetById(quoteId);

            var userBO = BOFactory.GetBO<UserBO>();
            var user = userBO.GetById(quotation.PersonInchargeId);
            var company = new Company() { CompanyCode = user.CompanyCode };
            return company;
        }

        private Invoice GetInvoice(SalesOrder so)
        {
            var invoice = new Invoice();
            invoice.QuoteId = so.QuoteId;
            invoice.QuoteIdentifier = so.QuoteIdentifier;            
            invoice.TotalDiscount = so.TotalDiscount;
            invoice.IncludeGST = so.IncludeGST;
            invoice.GST = so.GST;
            invoice.GSTZeroRated = so.GSTZeroRated;
            return invoice;
        }


        private InvoiceLine GetInvoiceLine(SalesOrderLine soline)
        {
            var invoiceLine = new InvoiceLine();
            invoiceLine.Description = soline.Description;
            invoiceLine.SNo = soline.SNo;
            invoiceLine.ItemId = soline.ItemId;
            invoiceLine.Quantity = soline.Quantity;
            invoiceLine.Unit = soline.Unit;

            invoiceLine.Quantity = soline.Quantity;

            invoiceLine.Quantity = soline.Quantity;
            if (soline.Preferred1)
            {
                invoiceLine.BuyingPrice = soline.BuyingPrice1;
                invoiceLine.Remarks = soline.Remarks1;
            }
            else if (soline.Preferred2)
            {
                invoiceLine.BuyingPrice = soline.BuyingPrice2;
                invoiceLine.Remarks = soline.Remarks2;
            }
            else if (soline.Preferred3)
            {
                invoiceLine.BuyingPrice = soline.BuyingPrice3;
                invoiceLine.Remarks = soline.Remarks3;
            }
            invoiceLine.UnitSellingPrice = soline.SellingPrice;
            invoiceLine.SellingPrice = soline.SellingPrice * soline.Quantity;
            invoiceLine.SalesOrderLineId = soline.Id;
            return invoiceLine;
        }

        private void SetSalesOrderDetails(long soId, Invoice invoice)
        {
            var so = BOFactory.GetBO<SalesOrderBO>().Items.Where(s => s.Id == soId).FirstOrDefault();
            if (so != null)
            {
                if (so.ClientId.HasValue)
                {
                    invoice.ClientName = BOHelper.GetClientNameById(so.ClientId.Value);
                    invoice.ClientBillingAddress = BOHelper.GetClientAddress(so.ClientId);
                }
                if (so.VesselId.HasValue)
                {
                    invoice.VesselName = BOHelper.GetVesselNameById(so.VesselId.Value);
                    invoice.VesselCode = BOHelper.GetVesselCodeById(so.VesselId.Value);
                }
                invoice.EstimatedDeliveryDate = so.EstimatedDeliveryDate;
                invoice.SalesOrderIdentifier = so.SalesOrderIdentifier;
                invoice.SalesOrderDate = so.SalesOrderDate;
                invoice.PurcharseOrderIdentifier = so.PurcharseOrderIdentifier;
                invoice.PurcharseOrderDate = so.PurcharseOrderDate;
                invoice.InvoiceIdentifier = so.InvoiceIdentifier;
                invoice.InvoiceDate = so.InvoiceDate;
                invoice.DeliveryIdentifier = so.DeliveryIdentifier;
                invoice.DeliveryDate = so.DeliveryDate;
                invoice.BillToAddress = so.BillToAddress;
                
                SupplierType type = SupplierType.Unknown;
                if (Enum.TryParse<SupplierType>(so.ProvisionType.ToString(), out type))
                {
                    invoice.ProvisionType = type.ToString();
                }

                var payType = PayType.Cash;
                if (!Enum.TryParse<PayType>(so.PayType.ToString(), out payType))
                {
                    payType = PayType.Cash;
                }
                invoice.PayType = payType.ToString();
            }
        }

        #endregion

        #region Operation Sheet

        public OperationSheet GetOperationSheet(long salesOrderId)
        {
            SalesOrder salesOrder = BOFactory.GetBO<SalesOrderBO>().GetById(salesOrderId);
            long quoteId = salesOrder.QuoteId;           
            
            OperationSheet operationSheet = new OperationSheet();
            SetSalesOrderDetails(salesOrderId, operationSheet);
            var pos = BOFactory.GetBO<PurchaseOrderBO>().GetPurchaseOrders(salesOrderId);
            if(pos != null)
            {
                operationSheet.Lines = new List<OperationSheetLine>();
                foreach (var po in pos) 
                {
                    var line = new OperationSheetLine();
                    line.SNo = po.SNo;
                    line.SupplierId = po.SupplierId;
                    line.SupplierName = po.SupplierName;
                    line.LineCount = po.LineCount;
                    line.DeliveryInstruction = po.DeliveryInstruction;
                    operationSheet.Lines.Add(line);
                }
            }
            return operationSheet;
        }

        private void SetSalesOrderDetails(long soId, OperationSheet opVO)
        {
            var so = BOFactory.GetBO<SalesOrderBO>().Items.Where(s => s.Id == soId).FirstOrDefault();
            if (so != null)
            {
                if (so.ClientId.HasValue)
                {
                    opVO.ClientName = BOHelper.GetClientNameById(so.ClientId.Value);
                }
                if (so.VesselId.HasValue)
                {
                    opVO.VesselName = BOHelper.GetVesselNameById(so.VesselId.Value);
                    var vesselCode = BOHelper.GetVesselCodeById(so.VesselId.Value);
                    opVO.VesselCode = string.IsNullOrEmpty(vesselCode) ? string.Empty : vesselCode.ToUpper();
                }
                
                opVO.EstimatedDeliveryDate = so.EstimatedDeliveryDate;
                opVO.SalesOrderIdentifier = so.SalesOrderIdentifier;
                opVO.PurcharseOrderIdentifier = so.PurcharseOrderIdentifier;
                
                SupplierType type = SupplierType.Unknown;
                if (Enum.TryParse<SupplierType>(so.ProvisionType.ToString(), out type))
                {
                    opVO.ProvisionType = type.ToString();
                }
                    
            }
        }

        #endregion

        #region Checklist

        public Checklist GetChecklist(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            SalesOrder salesOrder = salesOrderBO.GetById(salesOrderId);
            var salesOrderLines = salesOrderBO.GetSalesOrderLines(salesOrderId);
            long quoteId = salesOrder.QuoteId;                

            Checklist checklist = new Checklist();
            SetSalesOrderDetails(salesOrderId, checklist);

            if (salesOrderLines != null)
            {
                checklist.Lines = new List<ChecklistLine>();
                foreach (var soline in salesOrderLines)
                {
                    var line = GetChecklistLine(soline);
                    checklist.Lines.Add(line);
                }
            }
            return checklist;
        }

        private ChecklistLine GetChecklistLine(SalesOrderLine soline)
        {
            var line = new ChecklistLine();
            line.SNo = soline.SNo;
            line.SalesOrderId = soline.SalesOrderId;
            line.LineIdentifier = soline.SNo;
            line.SalesOrderLineId = soline.Id;            
            line.QuoteId = soline.QuoteId;
            line.QuoteLineId = soline.QuoteLineId;
            line.ItemId = soline.ItemId;
            line.Description =soline.Description;            
            line.Unit = soline.Unit;
            line.Quantity = soline.Quantity;
            line.SellingPrice = soline.SellingPrice;      
            
            if(soline.Preferred1)
            {
                line.SupplierId = soline.SupplierId1.Value;
                line.Remarks = soline.Remarks1;
            }
            else if (soline.Preferred2)
            {
                line.SupplierId = soline.SupplierId2.Value;
                line.Remarks = soline.Remarks2;
            }
            else if (soline.Preferred3)
            {
                line.SupplierId = soline.SupplierId3.Value;
                line.Remarks = soline.Remarks3;
            }
            line.SupplierName = BOHelper.GetSupplierNameById(line.SupplierId);
            return line;
        }

        private void SetSalesOrderDetails(long soId, Checklist vo)
        {
            var so = BOFactory.GetBO<SalesOrderBO>().Items.Where(s => s.Id == soId).FirstOrDefault();
            if (so != null)
            {
                if (so.ClientId.HasValue)
                {
                    vo.ClientName = BOHelper.GetClientNameById(so.ClientId.Value);
                }
                if (so.VesselId.HasValue)
                {
                    vo.VesselName = BOHelper.GetVesselNameById(so.VesselId.Value);
                }
                vo.SalesOrderIdentifier = so.SalesOrderIdentifier;
                vo.SalesOrderDate = so.SalesOrderDate;
                vo.PurcharseOrderIdentifier = so.PurcharseOrderIdentifier;
                vo.PurcharseOrderDate = so.PurcharseOrderDate;
                vo.InvoiceIdentifier = so.InvoiceIdentifier;
                vo.InvoiceDate = so.InvoiceDate;
                vo.DeliveryIdentifier = so.DeliveryIdentifier;
                vo.DeliveryDate = so.DeliveryDate;
                vo.BillToAddress = so.BillToAddress;
                
                SupplierType type = SupplierType.Unknown;
                if (Enum.TryParse<SupplierType>(so.ProvisionType.ToString(), out type))
                {
                    vo.ProvisionType = type.ToString();
                }
            }
        }

        #endregion

        #region DeliveryOrder

        public DeliveryOrder GetDeliveryOrder(long salesOrderId)
        {
            var salesOrderBO = BOFactory.GetBO<SalesOrderBO>();
            var salesOrder = salesOrderBO.GetById(salesOrderId);
            var salesOrderLines = salesOrderBO.GetSalesOrderLines(salesOrderId);

            long quoteId = salesOrder.QuoteId;      

            DeliveryOrder deliveryOrder = new DeliveryOrder();
            SetSalesOrderDetails(salesOrderId, deliveryOrder);

            if (salesOrderLines != null)
            {
                deliveryOrder.Lines = new List<DeliveryOrderLine>();
                foreach (var soline in salesOrderLines)
                {
                    var line = GetDeliveryOrderLine(soline);
                    deliveryOrder.Lines.Add(line);
                }
            }
            return deliveryOrder;
        }

        private DeliveryOrderLine GetDeliveryOrderLine(SalesOrderLine soline)
        {
            var line = new DeliveryOrderLine();
            line.SNo = soline.SNo;
            line.SalesOrderId = soline.SalesOrderId;
            line.LineIdentifier = soline.SNo;
            line.SalesOrderLineId = soline.Id;
            line.QuoteId = soline.QuoteId;
            line.QuoteLineId = soline.QuoteLineId;
            line.ItemId = soline.ItemId;
            line.Description = soline.Description;
            line.Unit = soline.Unit;
            line.Quantity = soline.Quantity;
            line.SellingPrice = soline.SellingPrice;  

            line.SalesOrderLineId = soline.Id;
            if (soline.Preferred1)
            {
                line.SupplierId = soline.SupplierId1.Value;
                line.Remarks = soline.Remarks1;
            }
            else if (soline.Preferred2)
            {
                line.SupplierId = soline.SupplierId2.Value;
                line.Remarks = soline.Remarks2;
            }
            else if (soline.Preferred3)
            {
                line.SupplierId = soline.SupplierId3.Value;
                line.Remarks = soline.Remarks3;
            }
            line.SupplierName = BOHelper.GetSupplierNameById(line.SupplierId);
            return line;
        }

        private void SetSalesOrderDetails(long soId, DeliveryOrder vo)
        {
            var so = BOFactory.GetBO<SalesOrderBO>().Items.Where(s => s.Id == soId).FirstOrDefault();
            if (so != null)
            {
                if (so.ClientId.HasValue)
                {
                    vo.ClientName = BOHelper.GetClientNameById(so.ClientId.Value);
                }
                if (so.VesselId.HasValue)
                {
                    vo.VesselName = BOHelper.GetVesselNameById(so.VesselId.Value);
                    vo.VesselCode = BOHelper.GetVesselCodeById(so.VesselId.Value);
                }                                
                
                vo.SalesOrderIdentifier = so.SalesOrderIdentifier;
                vo.SalesOrderDate = so.SalesOrderDate;
                vo.PurcharseOrderIdentifier = so.PurcharseOrderIdentifier;
                vo.PurcharseOrderDate = so.PurcharseOrderDate;
                vo.InvoiceIdentifier = so.InvoiceIdentifier;
                vo.InvoiceDate = so.InvoiceDate;
                vo.DeliveryIdentifier = so.DeliveryIdentifier;
                vo.DeliveryDate = so.DeliveryDate;
                vo.BillToAddress = so.BillToAddress;
                
                SupplierType type = SupplierType.Unknown;
                if (Enum.TryParse<SupplierType>(so.ProvisionType.ToString(), out type))
                {
                    vo.ProvisionType = type.ToString();
                }
            }
        }

    #endregion
    }
}