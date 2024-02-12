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
    public class PurchaseOrderBO : BaseBO<PurchaseOrder>
    {
        ItemMasterBO ItemMasterBO = BOFactory.GetBO<ItemMasterBO>();
        public BaseBO<PurchaseOrderLine> LineBO = BOFactory.GetBO<BaseBO<PurchaseOrderLine>>();

        public ListResponseVM<PurchaseOrder> GetPurchaseOrdersByFilter(long supplierId, string poIdentifier, DateTime? fromDate, DateTime? toDate, int startIndex, int pageSize)
        {
            var queryable = this.Items.Where(item => item.Status == Constants.STATUS_LIVE);
            if (supplierId > 0)
            {
                queryable = queryable.Where(item => item.SupplierId == supplierId);
            }
            if (!String.IsNullOrWhiteSpace(poIdentifier))
            {
                queryable = queryable.Where(item => item.PurchaseOrderIdentifier == poIdentifier);
            }
            if (fromDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date <= toDate.Value.Date);
            }
            var soItems = BOFactory.GetBO<SalesOrderBO>().Items;
            var lsResult = (from po in queryable
                            from so in soItems                             
                             where po.SalesOrderId == so.Id
                             where so.Status == (int)Constants.STATUS_LIVE
                             select new PurchaseOrder() {
                                            Id = po.Id,
                                            SalesOrderId = so.Id,
                                            DeliveryInstruction = po.DeliveryInstruction,
                                            SupplierName = po.SupplierName,
                                            Agent = po.Agent,
                                            Place = po.Place,
                                            Remarks = po.Remarks,
                                            ClientId = so.ClientId.GetValueOrDefault(),
                                            ClientName = so.ClientName,
                                            VesselId = so.VesselId.GetValueOrDefault(),
                                            VesselName = so.VesselName,
                                            POStatus = po.POStatus,
                                            LineCount = po.LineCount,
                                            CreatedDate = po.CreatedDate
                             });

            lsResult = lsResult.OrderByDescending(item => item.CreatedDate);

            var response = lsResult.GetPagedResponse(startIndex, pageSize);
            return response;    
        }

        public ListResponseVM<PurchaseOrder> GetPurchaseOrdersByClient(long clientId, string poIdentifier, DateTime? fromDate, DateTime? toDate, int startIndex, int pageSize)
        {
            var queryable = this.Items.Where(item => item.Status == Constants.STATUS_LIVE);
            if (!String.IsNullOrWhiteSpace(poIdentifier))
            {
                queryable = queryable.Where(item => item.PurchaseOrderIdentifier == poIdentifier);
            }
            if (fromDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                queryable = queryable.Where(item => item.CreatedDate.Value.Date <= toDate.Value.Date);
            }
            var soItems = BOFactory.GetBO<SalesOrderBO>().Items;
            var lsResult = (from po in queryable
                            from so in soItems
                            where po.SalesOrderId == so.Id
                            where so.ClientId == clientId
                            where so.Status == (int)Constants.STATUS_LIVE
                            select new PurchaseOrder()
                            {
                                Id = po.Id,
                                SalesOrderId = so.Id,
                                DeliveryInstruction = po.DeliveryInstruction,
                                SupplierName = po.SupplierName,
                                Agent = po.Agent,
                                Place = po.Place,
                                Remarks = po.Remarks,
                                ClientId = so.ClientId.GetValueOrDefault(),
                                ClientName = so.ClientName,
                                VesselId = so.VesselId.GetValueOrDefault(),
                                VesselName = so.VesselName,
                                POStatus = po.POStatus,
                                LineCount = po.LineCount,
                                CreatedDate = po.CreatedDate
                            });
            
            lsResult = lsResult.OrderByDescending(item => item.CreatedDate);

            var response = lsResult.GetPagedResponse(startIndex, pageSize);
            return response;    

        }

        public IList<PurchaseOrder> GetPurchaseOrders(long salesOrderId)
        {
            var pos = GetOrCreatePurchaseOrders(salesOrderId);
            return pos;
        }

        private IList<PurchaseOrder> GetOrCreatePurchaseOrders(long salesOrderId)
        {
            var company = this.GetCompanyInfo(salesOrderId);
            var lsResult = this.Items
                               .Where(item => item.SalesOrderId == salesOrderId)
                               .Where(item => item.Status == Constants.STATUS_LIVE)
                               .OrderBy(item=> item.SNo)
                               .ToList();
            if (lsResult.Count == 0)
            {
                long userId = 0;
                var result = this.ExecuteStoredProcedure("POS_CREATE_PURCHASE_ORDERS", 
                    new Dictionary<string, object>() {
                        { "@SALES_ORDER_ID", salesOrderId },
                        { "@USER_ID", userId },
                        { "@COMPANY_CODE", company.CompanyCode }
                    });
                lsResult = this.Items
                        .Where(po => po.SalesOrderId == salesOrderId)
                        .Where(po => po.Status == Constants.STATUS_LIVE)
                        .OrderBy(item => item.SNo)
                        .ToList();
                this.SessionEvict(lsResult);
            }
            return lsResult;
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

        public void UpdatePurchaseOrders(ListRequestVM<PurchaseOrder> request)
        {
            if (request != null && request.ModifiedList != null)
            {
                request.ModifiedList.ForEach(item =>
                {
                    var updated = this.GetObjectForUpdate(item.Id, item.Rev);
                    updated.DeliveryInstruction = item.DeliveryInstruction;
                    updated.Agent = item.Agent;
                    updated.Place = item.Place;
                    updated.Remarks = item.Remarks;
                    this.SaveOrUpdate(updated);
                });
            }
        }

        public long DeletePurchaseOrder(long id)
        {
            var purchaseOrder = this.GetById(id);
            if (purchaseOrder == null)
            {
                throw new Exception("Non-existing PurchaseOrder.");
            }
            this.MarkAsDelete(purchaseOrder);
            return id;
        }

        public override PurchaseOrder GetById(long id)
        {
            PurchaseOrder purchaseOrder = base.GetById(id);
            return purchaseOrder;
        }        

        public IList<PurchaseOrderLine> GetPurchaseOrderLines(long purchaseOrderId)
        {
            var lsResult = this.LineBO.Items
                                .Where(item => item.Status == Constants.STATUS_LIVE)
                                .Where(item => item.PurchaseOrderId == purchaseOrderId)
                                .ToList();
            this.SessionEvict(lsResult);
            lsResult = new List<PurchaseOrderLine>(lsResult.OrderBy(i => i.LineId));
            return lsResult;
        }

        public void SavePurchaseOrderLines(ListRequestVM<PurchaseOrderLine> request)
        {
            if (request != null)
            {
                if (request.AddedList != null)
                {
                    request.AddedList.ForEach(line => AddPurchaseOrderLine(line));
                }
                if (request.ModifiedList != null)
                {
                    request.ModifiedList.ForEach(line => UpdatePurchaseOrderLine(line));
                }
                if (request.DeletedList != null)
                {
                    request.DeletedList.ForEach(id =>
                    {
                        var line = this.LineBO.GetById(id);
                        this.LineBO.MarkAsDelete(line);
                    });
                }
            }
        }

        public long AddPurchaseOrderLine(PurchaseOrderLine line)
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
            }
            this.LineBO.SaveOrUpdate(line);
            return line.Id;
        }

        private long UpdatePurchaseOrderLine(PurchaseOrderLine line)
        {
            var updated = this.LineBO.GetObjectForUpdate(line.Id, line.Rev);

            updated.Unit = line.Unit;
            updated.IsIncluded = line.IsIncluded;
            updated.Remarks = line.Remarks;

            this.LineBO.SaveOrUpdate(updated);
            return updated.Id;
        }

        public void CancelPurchaseOrder(long purchaseOrderId)
        {
            long userId = SessionContext.UserId;
            this.ExecuteStoredProcedure("POS_CANCEL_PURCHASE_ORDER", new Dictionary<string, object>() { { "@PO_ID", purchaseOrderId }, { "@USER_ID", userId } });
        }

        public void ReceivePurchaseOrder(long purchaseOrderId)
        {
            long userId = SessionContext.UserId;
            this.ExecuteStoredProcedure("POS_RECEIVE_PURCHASE_ORDER", new Dictionary<string, object>() { { "@PO_ID", purchaseOrderId }, { "@USER_ID", userId } });
        }

        public void ReceivePurchaseOrderLine(long lineItemId)
        {
            long userId = SessionContext.UserId;
            this.ExecuteStoredProcedure("POS_RECEIVE_PURCHASE_ORDER_LINE", new Dictionary<string, object>() { { "@PO_LINE_ID", lineItemId }, { "@USER_ID", userId } });
        }

        public void IssuePurchaseOrder(long purchaseOrderId)
        {
            long userId = SessionContext.UserId;
            this.ExecuteStoredProcedure("POS_ISSUE_PURCHASE_ORDER", new Dictionary<string, object>(){ {"@PO_ID", purchaseOrderId } , { "@USER_ID", userId } });
        }

        public void IssuePurchaseOrderLine(long lineItemId)
        {
            long userId = SessionContext.UserId;
            this.ExecuteStoredProcedure("POS_ISSUE_PURCHASE_ORDER_LINE", new Dictionary<string, object>() { { "@PO_LINE_ID", lineItemId }, { "@USER_ID", userId } });
        }

        public PurchaseOrderBO() { }

    }

}
