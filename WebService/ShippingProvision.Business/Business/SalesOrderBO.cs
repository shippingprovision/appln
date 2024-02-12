using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using System.Data;
using ShippingProvision.Services.Helpers;
using ShippingProvision.Business.Helpers;

namespace ShippingProvision.Business
{
    public class SalesOrderBO : BaseBO<SalesOrder>
    {
        ItemMasterBO ItemMasterBO = BOFactory.GetBO<ItemMasterBO>();
        public BaseBO<SalesOrderLine> LineBO = BOFactory.GetBO<BaseBO<SalesOrderLine>>();

        public long GetSalesOrder(long quotationId)
        {
            var soId = this.Items
                        .Where(so => so.QuoteId == quotationId)
                        .Where(so => so.Status == Constants.STATUS_LIVE)
                        .Select(so => so.Id)
                        .FirstOrDefault();
            if (soId == 0)
            {
               long userId = 0;
               var result = this.ExecuteStoredProcedure("POS_CREATE_SALES_ORDER", new Dictionary<string, object>() { { "@QUOTE_ID", quotationId }, { "@USER_ID", userId } });
               var row = result.FirstOrDefault();
               if (row != null)
               {
                   soId = Convert.ToInt64(row["ID"]);
               }
            }
            return soId;
        }

        public long AddSalesOrder(SalesOrder salesOrder)
        {
            salesOrder.ClientName = BOHelper.GetClientNameById(salesOrder.ClientId.GetValueOrDefault());
            salesOrder.VesselName = BOHelper.GetVesselNameById(salesOrder.VesselId.GetValueOrDefault());
            salesOrder.PersonIncharge = BOHelper.GetUserById(salesOrder.PersonInchargeId);

            //CheckInvoiceIdentifierExists(salesOrder.Id, salesOrder.InvoiceIdentifier);            
            
            this.SaveOrUpdate(salesOrder);
            return salesOrder.Id;
        }

        public long UpdateSalesOrder(SalesOrder salesOrder)
        {
            var updated = this.GetObjectForUpdate(salesOrder.Id, salesOrder.Rev);

            //CheckInvoiceIdentifierExists(salesOrder.Id, salesOrder.InvoiceIdentifier);

            //updated.QuoteIdentifier = salesOrder.QuoteIdentifier;
            updated.ClientId = salesOrder.ClientId;
            updated.ClientName = BOHelper.GetClientNameById(salesOrder.ClientId.GetValueOrDefault());
            updated.VesselId = salesOrder.VesselId;
            updated.VesselName = BOHelper.GetVesselNameById(salesOrder.VesselId.GetValueOrDefault());
            updated.PersonInchargeId = salesOrder.PersonInchargeId;
            updated.PersonIncharge = BOHelper.GetUserById(salesOrder.PersonInchargeId);
            updated.ProvisionType = salesOrder.ProvisionType;
            updated.PayType = salesOrder.PayType;
            updated.ProvisionStatus = salesOrder.ProvisionStatus;
            updated.TotalDiscount = salesOrder.TotalDiscount;
            updated.IncludeGST = salesOrder.IncludeGST;
            updated.GSTZeroRated = salesOrder.GSTZeroRated;

            updated.InvoiceIdentifier = salesOrder.InvoiceIdentifier;
            updated.InvoiceDate = salesOrder.InvoiceDate;
            updated.DeliveryIdentifier = salesOrder.DeliveryIdentifier;
            updated.DeliveryDate = salesOrder.DeliveryDate;
            updated.PurcharseOrderIdentifier = salesOrder.PurcharseOrderIdentifier;
            updated.EstimatedDeliveryDate = salesOrder.EstimatedDeliveryDate;
            updated.GST = salesOrder.GST;

            this.SaveOrUpdate(updated);
            return salesOrder.Id;
        }

        public string CheckInvoiceIdentifierExists(long soId, string invoiceNo)
        {
            int count = 0;
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(invoiceNo))
            {
                count = this.Items
                            .Where(so => so.Status == Constants.STATUS_LIVE)
                            .Where(so => so.Id != soId)
                            .Where(so => so.InvoiceIdentifier == invoiceNo)
                            .Count();
            }
            bool exists = count > 0;
            if (exists)
            {
                result = string.Format("Invoice number-{0} already exists.", invoiceNo);
            }
            return result;
        }

        public long DeleteSalesOrder(long id)
        {
            var salesOrder = this.GetById(id);
            if (salesOrder == null)
            {
                throw new Exception("Non-existing SalesOrder.");
            }
            this.MarkAsDelete(salesOrder);
            return id;
        }

        public override SalesOrder GetById(long id)
        {
            SalesOrder salesOrder = base.GetById(id);
            return salesOrder;
        }

        public IList<SalesOrder> GetSalesOrders()
        {
            var lsResult = this.Items
                               .Where(item => item.Status == Constants.STATUS_LIVE)                
                               .Where(item => item.ProvisionStatus != (int)SalesOrderStatus.Completed)                                
                               .ToList();
            return lsResult;
        }

        public ListResponseVM<SalesOrder> GetSalesOrdersByFilter(DateTime? fromDate, DateTime? toDate, long typeId, long clientId, long vesselId, int startIndex, int pageSize)
        {
            var queryable = this.Items
                            .Where(item => item.Status == Constants.STATUS_LIVE)
                            .Where(item => item.ProvisionStatus != (int)SalesOrderStatus.Completed);
            if (fromDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date <= toDate.Value.Date);
            }
            if (typeId > 0)
            {
                queryable = queryable.Where(item => item.ProvisionType == typeId);
            }
            if (clientId > 0)
            {
                queryable = queryable.Where(item => item.ClientId == clientId);
            }
            if (vesselId > 0)
            {
                queryable = queryable.Where(item => item.VesselId == vesselId);
            }

            queryable = queryable.OrderByDescending(item => item.EstimatedDeliveryDate);

            var response = queryable.GetPagedResponse(startIndex, pageSize);
            return response;    
            
        }

        public IList<SalesOrderLine> GetSalesOrderLines(long salesOrderId)
        {
            var lsResult = this.LineBO.Items
                                .Where(item => item.Status == Constants.STATUS_LIVE)
                                .Where(item => item.SalesOrderId == salesOrderId)
                                .ToList();
            lsResult = new List<SalesOrderLine>(lsResult.OrderBy(i => i.LineId));
            return lsResult;
        }

        public void SaveSalesOrderLines(ListRequestVM<SalesOrderLine> request)
        {
            if (request != null)
            {
                long soId = 0;
                if (request.AddedList != null)
                {
                    request.AddedList.ForEach(line => { soId = (long)line.SalesOrderId; AddSalesOrderLine(line); });
                }
                if (request.ModifiedList != null)
                {
                    request.ModifiedList.ForEach(line => { soId = (long)line.SalesOrderId; UpdateSalesOrderLine(line); });
                }
                if (request.DeletedList != null)
                {
                    request.DeletedList.ForEach(id =>
                    {
                        var line = this.LineBO.GetById(id);
                        soId = (long)line.SalesOrderId;
                        this.LineBO.MarkAsDelete(line);
                    });
                }
                if(soId != 0)
                {
                    long userId = 0;
                    this.Session.Flush();
                    RecomputePurchaseOrders(soId, userId);
                }
            }
        }

        private void RecomputePurchaseOrders(long soId, long userId)
        {
            var company = this.GetCompanyInfo(soId);
            var exists = BOFactory.GetBO<PurchaseOrderBO>()
                                 .Items
                                 .Where(i => i.SalesOrderId == soId)
                                 .Where(i => i.Status == Constants.STATUS_LIVE)
                                 .Count();
            if (exists > 0)
            {
                this.ExecuteStoredProcedure("POS_CREATE_PURCHASE_ORDERS", 
                    new Dictionary<String, object>() {
                        { "@SALES_ORDER_ID", soId },
                        { "@USER_ID", userId },
                        { "@COMPANY_CODE", company.CompanyCode }
                    });
            }
        }


        public long AddSalesOrderLine(SalesOrderLine line)
        {
            if (line.ItemId.GetValueOrDefault() == 0)
            {
                var itemMaster = ItemMasterBO.GetOrCreateItem(line.Description, line.Unit);
                line.ItemId = itemMaster.Id;
            }
            else
            {
                var itemMaster = BOHelper.GetItemDetailsById(line.ItemId.GetValueOrDefault());
                line.Description = itemMaster.Description;
                line.Unit = itemMaster.Unit;
                if (itemMaster.IsStockItem)
                {
                    line.StockItemId = itemMaster.Id;
                }
            }
            this.LineBO.SaveOrUpdate(line);
            return line.Id;
        }

        private long UpdateSalesOrderLine(SalesOrderLine line)
        {
            var updated = this.LineBO.GetById(line.Id);

            updated.SNo = line.SNo;
            updated.Description = line.Description;
            updated.Unit = line.Unit;
            updated.Quantity = line.Quantity;
            updated.SellingPrice = line.SellingPrice;
            
            updated.SupplierId1 = line.SupplierId1;
            updated.BuyingPrice1 = line.BuyingPrice1;
            updated.Preferred1 = line.Preferred1;
            updated.Remarks1 = line.Remarks1;

            updated.SupplierId2 = line.SupplierId2;
            updated.BuyingPrice2 = line.BuyingPrice2;
            updated.Preferred2 = line.Preferred2;
            updated.Remarks2 = line.Remarks2;

            updated.StockItemId = line.StockItemId;

            //updated.SupplierId3 = line.SupplierId3;
            //updated.BuyingPrice3 = line.BuyingPrice3;
            //updated.Preferred3 = line.Preferred3;
            //updated.Remarks3 = line.Remarks3;

            this.LineBO.SaveOrUpdate(updated);
            return updated.Id;
        }

        public void CompleteOrder(long salesOrderId)
        {
            long userId = 0;
            this.ExecuteStoredProcedure("POS_COMPLETE_SALES_ORDER", new Dictionary<string, object>() { { "@SO_ID", salesOrderId }, { "@USER_ID", userId } });
        }

        public Company GetCompanyInfo(long salesOrderId)
        {
            var salesOrder = this.GetById(salesOrderId);
            return GetCompanyInfo(salesOrder);
        }

        public Company GetCompanyInfo(SalesOrder salesOrder)
        {
            var userBO = BOFactory.GetBO<UserBO>();
            var user = userBO.GetById(salesOrder.PersonInchargeId);
            var company = new Company() { CompanyCode = user.CompanyCode };
            return company;
        }

        public string GenerateInvoiceNumber(long salesOrderId)
        {
            string result = "";
            var salesOrder = this.GetById(salesOrderId);
            var company = GetCompanyInfo(salesOrder);
            List<Dictionary<string, object>> lsResult =
                                    this.ExecuteStoredProcedure("POS_GENERATE_SEQUENCE_NUMBER", 
                                    new Dictionary<String, object>() { { "@COMPANY_CODE", company.CompanyCode } });

            if (lsResult != null && lsResult.Count > 0)
            {
                var seqObj = lsResult[0].FirstOrDefault();
                result = Convert.ToString(seqObj.Value);
            }

            //Update invoice identifier in sales order
            salesOrder.InvoiceIdentifier = result;
            this.SaveOrUpdate(salesOrder);

            return result;
        }

        public string GeneratePONumber(long salesOrderId)
        {
            string result = "";
            var company = GetCompanyInfo(salesOrderId);
            List<Dictionary<string, object>> lsResult =
                                    this.ExecuteStoredProcedure("POS_GENERATE_PO_NUMBER",
                                    new Dictionary<String, object>() { { "@COMPANY_CODE", company.CompanyCode } });

            if (lsResult != null && lsResult.Count > 0)
            {
                var seqObj = lsResult[0].FirstOrDefault();
                result = Convert.ToString(seqObj.Value);
            }
            return result;
        }

        public string ValidateInvoice(long salesOrderId)
        {
            string result = "valid";

            var lsResult = this.LineBO.Items
                     .Where(item => item.Status == Constants.STATUS_LIVE)
                     .Where(item => item.SalesOrderId == salesOrderId)
                     .Where(item => item.SellingPrice == 0)
                     .ToList();
            if (lsResult != null && lsResult.Count > 0)
            {
                result = "invalid";
            }
            return result;
        }
        
        public SalesOrderBO() { }

        
    }

}
