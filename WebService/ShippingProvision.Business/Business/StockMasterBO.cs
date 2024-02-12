using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using System.Data;
using ShippingProvision.Services.Helpers;
using ShippingProvision.Business.ViewModels;

namespace ShippingProvision.Business
{
    public class StockMasterBO : BaseBO<StockMaster>
    {
        StockItemHistoryBO StockItemHistoryBO = BOFactory.GetBO<StockItemHistoryBO>();

        public long AddStockMaster(StockMaster stockMaster)
        {
            stockMaster.Status = Constants.STATUS_LIVE;
            stockMaster.IsStockItem = true;

            if (stockMaster.UnitPrice.HasValue)
            {
                stockMaster.PriceUpdatedDate = DateTime.Now;
            }

            this.SaveOrUpdate(stockMaster);

            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stockMaster.Id,
                TransType = (int)StockEntryType.PurchaseStock,
                TransDate = DateTime.Now,
                TransIdentifier = GetCurrentTransIdentifier(),
                Quantity = stockMaster.Quantity
            });

            return stockMaster.Id;
        }

        public long UpdateStockMaster(StockMaster stockMaster)
        {
            var updated = this.GetObjectForUpdate(stockMaster.Id, stockMaster.Rev);
            updated.ItemDescription = stockMaster.ItemDescription;
            updated.Unit = stockMaster.Unit;
            updated.ItemType = stockMaster.ItemType;
            updated.CategoryId = stockMaster.CategoryId;
            updated.CategoryName = stockMaster.CategoryName;
            updated.Remarks = stockMaster.Remarks;
            updated.IsStockItem = true;

            if (updated.UnitPrice != stockMaster.UnitPrice)
            {
                updated.UnitPrice = stockMaster.UnitPrice;
                updated.PriceUpdatedDate = DateTime.Now;
            }

            this.SaveOrUpdate(updated);
            
            return stockMaster.Id;
        }

        public List<long> UpdateStockMasters(List<StockMaster> lsStockMasters)
        {
            List<long> lsResult = new List<long>();
            foreach (var stockMaster in lsStockMasters)
            {
                var updated = this.GetObjectForUpdate(stockMaster.Id, stockMaster.Rev);
                updated.Rev = stockMaster.Rev;
                updated.Status = Constants.STATUS_LIVE;
                updated.IsStockItem = true;
                this.SaveOrUpdate(updated);

                lsResult.Add(updated.Id);
            }
            return lsResult;
        }

        public void ImportStockItems(IEnumerable<StockMasterRow> stockRows)
        {
            int lineno = 0;
            try
            {
                foreach (var stockRow in stockRows)
                {
                    lineno++;
                    var stockItem = GetStockItem(stockRow);
                    var stockMaster = UploadStockItem(stockItem);
                    UpdateStockSupplier(stockMaster, stockRow);
                }
            }
            catch (Exception ex)
            {
                ex.Data["AddlnInfo"] = string.Format("Last Executed Line No: {0}", lineno);
                throw;
            }
        }

        private StockMaster GetStockItem(StockMasterRow stockRow)
        {
            var stock = new StockMaster()
            {
                ItemCode = stockRow.ItemCode,
                ItemDescription = stockRow.Description,
                Unit = stockRow.Unit,
                Quantity = stockRow.Quantity,
                CategoryName = stockRow.CategoryName
            };
            return stock;
        }

        private StockMaster UploadStockItem(StockMaster stockMaster)
        {
            var stock = this.GetOrCreateItem(
                stockMaster.ItemCode,
                stockMaster.ItemDescription,
                stockMaster.Unit,
                stockMaster.CategoryName
            );
            if (stock == null)
                return null;

            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stock.Id,
                TransType = (int)StockEntryType.PurchaseStock,
                TransDate = DateTime.Now,
                TransIdentifier = GetCurrentTransIdentifier(),
                Quantity = stockMaster.Quantity
            });

            return stock;
        }

        string transIdentifier = string.Empty;
        //for upload excel - all stock item history would have same identifier
        private string GetCurrentTransIdentifier()
        {
            return (string.IsNullOrEmpty(transIdentifier)) ? transIdentifier = Guid.NewGuid().ToString() : transIdentifier;
        }

        private StockMaster GetOrCreateItem(string code, string description, string unit, string categoryName)
        {
            var categoryId = GetOrCreateCategory(categoryName);

            var stockMaster = this.Items
                               .Where(i => i.ItemCode == code)
                               .Where(i => i.ItemDescription == description)
                               .Where(i => i.Unit == unit)
                               .Where(i => i.CategoryId == categoryId)
                               .Where(i => i.IsStockItem)
                               .Where(i => i.Status == (int)Constants.STATUS_LIVE)
                               .FirstOrDefault();
            if (stockMaster != null)
            {
                return stockMaster;
            }

            stockMaster = new StockMaster()
            {
                ItemCode = code,
                ItemDescription = description,
                Unit = unit,
                CategoryId = categoryId,
                CategoryName = categoryName,
                IsStockItem = true
            };
            stockMaster.Status = Constants.STATUS_LIVE;
            this.SaveOrUpdate(stockMaster);
            return stockMaster;
        }

        private Dictionary<string, long> categoryMap = new Dictionary<string, long>();
        private CategoryBO CategoryBO = BOFactory.GetBO<CategoryBO>();

        private long GetOrCreateCategory(string categoryName)
        {
            long categoryId;
            if (!categoryMap.TryGetValue(categoryName, out categoryId))
            {
                categoryId = this.CategoryBO.Items
                                  .Where(c => c.Name == categoryName)
                                  .Where(c => c.Status == (int)Constants.STATUS_LIVE)
                                  .Select(c => c.Id)
                                  .FirstOrDefault();
                if (categoryId == 0)
                {
                    var category = new PMCategory()
                    {
                        Name = categoryName,
                        Code = categoryName,
                        Description = categoryName
                    };
                    this.CategoryBO.SaveOrUpdate(category);
                    categoryId = category.Id;
                    categoryMap.Add(categoryName, categoryId);
                }
            }
            return categoryId;
        }

        private ItemSupplierMapBO ItemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();

        private void UpdateStockSupplier(StockMaster stockItem, StockMasterRow stockRow)
        {
            var supplierId = this.GetSupplierId(stockRow.SupplierCode);

            var itemSupplier = this.ItemSupplierMapBO.Items.Where(itsup => itsup.ItemId == stockItem.Id
                                             && itsup.SupplierId == supplierId)
                                             .FirstOrDefault();
            itemSupplier = itemSupplier ?? new ItemSupplierMap()
                {
                    ItemId = stockItem.Id,
                    SupplierId = supplierId
                };
            itemSupplier.BuyingPrice = stockRow.BuyingPrice;
            itemSupplier.Preference = 1;

            this.ItemSupplierMapBO.SaveOrUpdate(itemSupplier);
        }

        private Dictionary<string, long> supplierMap = new Dictionary<string, long>();
        private SupplierMasterBO SupplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();

        private long GetSupplierId(string supplierCode)
        {
            long supplierId;
            if (!supplierMap.TryGetValue(supplierCode, out supplierId))
            {
                supplierId = this.SupplierMasterBO.Items
                                  .Where(s => s.SupplierCode == supplierCode)
                                  .Where(c => c.Status == (int)Constants.STATUS_LIVE)
                                  .Select(c => c.Id)
                                  .FirstOrDefault();
                if (supplierId != 0)
                {
                    supplierMap.Add(supplierCode, supplierId);
                }
            }
            return supplierId;
        }

        public void AddStockQuantity(StockMaster stock)
        {
            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stock.Id,
                TransType = (int)StockEntryType.PurchaseStock,
                TransDate = DateTime.Now,
                TransIdentifier = "Master",
                Quantity = stock.Quantity
            });
        }

        public void AdjustStockQuantity(StockMaster stock)
        {
            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stock.Id,
                TransType = (int)StockEntryType.AdjustStock,
                TransDate = DateTime.Now,
                TransIdentifier = "Master",
                Quantity = stock.Quantity
            });
        }

        public void CancelStockQuantity(StockMaster stock)
        {
            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stock.Id,
                TransType = (int)StockEntryType.CancelStock,
                TransDate = DateTime.Now,
                TransIdentifier = "Cancel",
                Quantity = stock.Quantity,
                Remarks = stock.Remarks
            });
        }

        public void ReturnStockQuantity(StockMaster stock)
        {
            StockItemHistoryBO.AddStockItemHistory(new StockItemHistory()
            {
                ItemId = stock.Id,
                TransType = (int)StockEntryType.ReturnStock,
                TransDate = DateTime.Now,
                TransIdentifier = "Return",
                Quantity = stock.Quantity,
                Remarks = stock.Remarks
            });
        }

        public bool IsStockMasterExists(int itemMasterId)
        {
            var itemId = this.Items
                           .Where(i => i.Id == itemMasterId)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteStockMaster(long id)
        {
            var stockMaster = this.GetById(id);
            if (stockMaster == null)
            {
                throw new Exception("Non-existing Stock Master.");
            }

            this.MarkAsDelete(stockMaster);

            return id;
        }

        public override StockMaster GetById(long id)
        {
            StockMaster stockMaster = base.GetById(id);
            return stockMaster;
        }

        public IList<StockMaster> GetStockMastersByFilter(string searchText, int filterType, long categoryId)
        {
            var querable = this.Items
                               .Where(item => item.Status == Constants.STATUS_LIVE && item.IsStockItem);

            if (!String.IsNullOrEmpty(searchText))
            {
                if (filterType == (int)FilterType.StartsWith)
                {
                    querable = querable.Where(item => item.ItemDescription.StartsWith(searchText));
                }
                else if (filterType == (int)FilterType.Contains)
                {
                    querable = querable.Where(item => item.ItemDescription.Contains(searchText));
                }
            }
            if (categoryId > 0)
            {
                querable = querable.Where(item => item.CategoryId == categoryId);
            }

            var lsResult = querable.ToList();
            return lsResult;
        }

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                            .Where(i => i.Status == Constants.STATUS_LIVE)
                            .Where(i => i.IsStockItem)
                            .OrderBy(i => i.ItemDescription)
                            .Select(i => new OptionItem { Id = i.Id, Text = string.Format("{0} {1} {2}", i.ItemDescription, i.Unit, i.Remarks) })
                            .ToList();
            return options;
        }

        public List<StockItemHistoryLog> GetStockItemHistory(long itemId)
        {
            var results = new List<StockItemHistoryLog>();
            var rows = this.ExecuteStoredProcedure("POS_VIEW_STOCK_ITEM_HISTORY", new Dictionary<String, object>() { { "@ITEM_ID", itemId } });
            foreach (var row in rows)
            {
                results.Add(new StockItemHistoryLog()
                {
                    Sno = Convert.ToInt32(row["Sno"]),
                    Description = Convert.ToString(row["Description"]),
                    Type = Convert.ToString(row["Type"]),
                    QtyIn = Convert.ToDecimal(row["QtyIn"]),
                    QtyOut = Convert.ToDecimal(row["QtyOut"]),
                    TransDate = Convert.ToDateTime(row["TransDate"]),
                    TransUser = Convert.ToString(row["TransUser"])
                });
            }
            return results;
        }

        public dynamic GetOptions(string searchText, int start, int count)
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE)
                              .Where(i => i.IsStockItem == true)
                              .Where(i => i.ItemDescription.StartsWith(searchText))
                              .OrderBy(i => i.ItemDescription)
                              .Select(i => new
                              {
                                  Id = i.Id,
                                  Text = string.Format("{0} {1} {2}", i.ItemDescription, i.Unit, i.Remarks),
                                  Description = i.ItemDescription,
                                  Unit = i.Unit,
                                  Remarks = i.Remarks
                              }) 
                              .Skip(start)
                              .Take(count)
                              .ToList();
            return options;
        }

        public StockMasterBO() { }



    }
}
