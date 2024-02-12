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
    public class QuotationBO : BaseBO<Quotation>
    {
        ItemMasterBO ItemMasterBO = BOFactory.GetBO<ItemMasterBO>();
        public BaseBO<QuotationLine> LineBO = BOFactory.GetBO<BaseBO<QuotationLine>>();


        public long AddQuotation(Quotation quotation)
        {
            quotation.ClientName = BOHelper.GetClientNameById(quotation.ClientId.GetValueOrDefault());
            quotation.VesselName = BOHelper.GetVesselNameById(quotation.VesselId.GetValueOrDefault());
            quotation.PersonIncharge = BOHelper.GetUserById(quotation.PersonInchargeId);
            this.SaveOrUpdate(quotation);
            return quotation.Id;
        }

        public long UpdateQuotation(Quotation quotation)
        {
            var updated = this.GetObjectForUpdate(quotation.Id, quotation.Rev);

            updated.QuoteIdentifier = quotation.QuoteIdentifier;
            updated.ClientId = quotation.ClientId;
            updated.ClientName = BOHelper.GetClientNameById(quotation.ClientId.GetValueOrDefault());
            updated.VesselId = quotation.VesselId;
            updated.VesselName = BOHelper.GetVesselNameById(quotation.VesselId.GetValueOrDefault());
            updated.PersonInchargeId = quotation.PersonInchargeId;
            updated.PersonIncharge = BOHelper.GetUserById(quotation.PersonInchargeId);
            updated.ProvisionType = quotation.ProvisionType;
            updated.PayType = quotation.PayType;
            updated.ProvisionStatus = quotation.ProvisionStatus;
            updated.EstimatedDeliveryDate = quotation.EstimatedDeliveryDate;

            this.SaveOrUpdate(updated);
            return quotation.Id;
        }

        public long DeleteQuotation(long id)
        {
            var quotation = this.GetById(id);
            if (quotation == null)
            {
                throw new Exception("Non-existing Quotation.");
            }
            this.MarkAsDelete(quotation);
            return id;
        }

        public override Quotation GetById(long id)
        {
            Quotation quotation = base.GetById(id);
            return quotation;
        }

        public IList<Quotation> GetQuotations()
        {
            var lsResult = this.Items
                                .Where(item => item.Status == Constants.STATUS_LIVE)
                                .ToList();
            return lsResult;
        }

        public ListResponseVM<Quotation> GetQuotationsByFilter(DateTime? fromDate, DateTime? toDate, long typeId, long clientId, long vesselId, int startIndex, int pageSize)
        {
            var queryable = this.Items
                            .Where(item => item.Status == Constants.STATUS_LIVE)
                            .Where(item => item.ProvisionStatus != (int)QuotationStatus.Completed);
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

        public long ImportQuotation(Quotation quotation, IEnumerable<QuotationLine> lines)
        {
            long quotationId = default(long);
            int lineno = 0;
            try
            {
                quotationId = AddQuotation(quotation);
                foreach (var line in lines)
                {
                    lineno++;
                    line.QuoteId = quotationId;                    
                    AddQuotationLine(line);
                }

                this.ExecuteStoredProcedure("POS_UPDATE_QUOTATION_SUPPLIERS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId } });
                this.ExecuteStoredProcedure("POS_UPDATE_BOOKINGS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId } });
            }
            catch (Exception ex)
            {
                ex.Data["AddlnInfo"] = string.Format("Last Executed Line No: {0}", lineno);
                throw;
            }
            return quotationId;
        }

        public IList<QuotationLine> GetQuotationLines(long quotationId)
        {
            var lsResult = this.LineBO.Items
                                .Where(item => item.Status == Constants.STATUS_LIVE)
                                .Where(item => item.QuoteId == quotationId)
                                .ToList();
            lsResult = new List<QuotationLine>(lsResult.OrderBy(q => q.LineId));
            return lsResult;
        }

        public void SaveQuotationLines(ListRequestVM<QuotationLine> request)
        {
            if (request != null)
            {
                long quotationId = 0;
                if (request.AddedList != null)
                {
                    request.AddedList.ForEach(line => { quotationId = (long)line.QuoteId; AddQuotationLine(line); });
                }
                if (request.ModifiedList != null)
                {
                    request.ModifiedList.ForEach(line => { quotationId = (long)line.QuoteId; var a = (line.Id == 0) ? AddQuotationLine(line) : UpdateQuotationLine(line); });
                }
                if (request.DeletedList != null)
                {
                    request.DeletedList.ForEach(id =>
                    {
                        var line = this.LineBO.GetById(id);                        
                        quotationId = (long)line.QuoteId; 
                        this.LineBO.MarkAsDelete(line);
                    });
                }
                if (quotationId != 0)
                {
                    long userId = 0;
                    this.Session.Flush();
                    this.ExecuteStoredProcedure("POS_UPDATE_QUOTATION_SUPPLIERS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId }, { "@COPY_DEFAULTS", 1 } });
                    this.ExecuteStoredProcedure("POS_UPDATE_BOOKINGS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId } });
                    //recompute RFQ
                    RecomputeSupplierRfqs(quotationId, userId);
                    RecomputeClientQuotation(quotationId, userId);
                }
            }
        }

        private void RecomputeSupplierRfqs(long quotationId, long userId)
        {
            var exists = BOFactory.GetBO<SupplierRfqBO>()
                                 .Items
                                 .Where(i => i.QuoteId == quotationId)
                                 .Where(i => i.Status == Constants.STATUS_LIVE)
                                 .Count();
            if (exists > 0)
            {
                this.ExecuteStoredProcedure("POS_CREATE_SUPPLIER_RFQS", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId }, { "@USER_ID", userId } });
            }
        }

        private void RecomputeClientQuotation(long quotationId, long userId)
        {
            var exists = BOFactory.GetBO<ClientRfqBO>()
                              .Items
                              .Where(i => i.QuoteId == quotationId)
                              .Where(i => i.Status == Constants.STATUS_LIVE)
                              .Count();
            if (exists > 0)
            {
                this.ExecuteStoredProcedure("POS_CREATE_CLIENT_RFQ", new Dictionary<String, object>() { { "@QUOTE_ID", quotationId }, { "@USER_ID", userId } });
            }
        }

        public long AddQuotationLine(QuotationLine line)
        {
            if (!string.IsNullOrEmpty(line.ItemCode))
            {
                var itemMaster = ItemMasterBO.GetOrCreateItem(line.ItemCode, line.Description, line.Unit);
                line.ItemId = itemMaster.Id;
                line.Description = string.Format("{0}-{1}", itemMaster.ItemCode, itemMaster.ItemDescription);
                line.Unit = itemMaster.Unit;
            }
            else if (line.ItemId.GetValueOrDefault() == 0)
            {
                var itemMaster = ItemMasterBO.GetOrCreateItem(line.Description, line.Unit);
                line.ItemId = itemMaster.Id;
            }
            else
            {
                var itemMaster = BOHelper.GetItemDetailsById(line.ItemId.GetValueOrDefault());
                line.Description = itemMaster.Description;
                line.Unit = itemMaster.Unit;
                if(itemMaster.IsStockItem)
                {
                    line.StockItemId = itemMaster.Id;
                    ItemSupplierMap itemSupplierMap = BOFactory.GetBO<ItemSupplierMapBO>().Items.Where(map => map.ItemId == line.StockItemId)
                                                                    .OrderBy(item=> item.Preference)
                                                                    .FirstOrDefault();
                    if (itemSupplierMap != null)
                    {
                        line.SupplierId1 = itemSupplierMap.SupplierId;
                        line.BuyingPrice1 = itemSupplierMap.BuyingPrice;
                        line.Preferred1 = true;
                    }
                }
            }
            this.LineBO.SaveOrUpdate(line);
            return line.Id;
        }

        private long UpdateQuotationLine(QuotationLine line)
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

            //updated.SupplierId3 = line.SupplierId3;
            //updated.BuyingPrice3 = line.BuyingPrice3;
            //updated.Preferred3 = line.Preferred3;
            //updated.Remarks3 = line.Remarks3;

            updated.StockItemId = line.StockItemId; 

            this.LineBO.SaveOrUpdate(updated);
            return updated.Id;
        }

        public QuotationBO() { }
    }

}
