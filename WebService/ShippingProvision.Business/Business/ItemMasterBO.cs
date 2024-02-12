using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using NHibernate;
using NHibernate.Criterion;

namespace ShippingProvision.Business
{
    public class ItemMasterBO : BaseBO<ItemMaster>
    {
        private CategoryBO CategoryBO = BOFactory.GetBO<CategoryBO>();

        public ItemMaster GetOrCreateItem(string description, string unit)
        {
            var itemMaster = this.Items
                               .Where(i => i.ItemDescription == description)
                               .Where(i => i.Unit == unit)
                               .Where(i => i.IsStockItem == false)
                               .Where(i => i.ItemType != (int)SupplierType.TechnicalStores)
                               .Where(i => i.Status == (int)Constants.STATUS_LIVE)
                               .FirstOrDefault();
            if (itemMaster != null)
            {
                return itemMaster;
            }

            itemMaster = new ItemMaster()
            {
                ItemDescription = description,
                Unit = unit,
                ItemType = (int)SupplierType.Provision,
                IsStockItem = false
            };
            itemMaster.Status = Constants.STATUS_LIVE;
            this.SaveOrUpdate(itemMaster);
            return itemMaster;
        }

        public ItemMaster GetOrCreateItem(string itemCode, string description, string unit)
        {
            var itemMaster = this.Items
                               .Where(i => i.ItemCode == itemCode)
                               .Where(i => i.IsStockItem == false)
                               .Where(i => i.ItemType == (int)SupplierType.TechnicalStores)
                               .Where(i => i.Status == (int)Constants.STATUS_LIVE)
                               .FirstOrDefault();
            if (itemMaster != null)
            {
                return itemMaster;
            }
            var categoryCode = itemCode.Substring(0, 2);
            var category = CategoryBO
                                    .Items
                                    .Where(c => c.Code == categoryCode)
                                    .Where(c => c.Type == (int)SupplierType.TechnicalStores) //Technical Stores
                                    .FirstOrDefault();
            itemMaster = new ItemMaster()
            {
                ItemCode = itemCode,
                ItemDescription = description,
                Unit = unit,
                CategoryId = category != null ? category.Id : 0,
                CategoryName = category != null ? category.Name : "",
                ItemType = (int)SupplierType.TechnicalStores,
                IsStockItem = false
            };
            this.SaveOrUpdate(itemMaster);
            return itemMaster;
        }

        public long AddItemMaster(ItemMaster itemMaster)
        {
            itemMaster.Status = Constants.STATUS_LIVE;

            if (IsItemMasterExists(itemMaster.ItemDescription))
            {
                throw new Exception("ItemMaster already exists.");
            }
            this.SaveOrUpdate(itemMaster);
            return itemMaster.Id;
        }

        public long UpdateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.Status = Constants.STATUS_LIVE;

            var existingItem = this.GetById(itemMaster.Id);
            existingItem.ItemDescription = itemMaster.ItemDescription;
            existingItem.ItemCode = itemMaster.ItemCode;
            existingItem.Unit = itemMaster.Unit;
            existingItem.ItemType = itemMaster.ItemType;
            existingItem.CategoryId = itemMaster.CategoryId;
            existingItem.CategoryName = itemMaster.CategoryName;
            existingItem.Remarks = itemMaster.Remarks;

            this.SaveOrUpdate(existingItem);

            return itemMaster.Id;
        }

        public bool IsItemMasterExists(string name)
        {
            var itemId = this.Items
                           .Where(i => i.ItemDescription == name)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteItemMaster(long id)
        {
            var itemMaster = this.GetById(id);
            if (itemMaster == null)
            {
                throw new Exception("Non-existing ItemMaster.");
            }

            this.MarkAsDelete(itemMaster);

            return id;
        }

        public IList<ItemMaster> GetItemMasters()
        {
            return this.Items.Where(item => item.Status == Constants.STATUS_LIVE).ToList();
        }

        public IList<ItemMaster> GetItemMastersByCategoryId(long categoryId)
        {
            return this.Items.Where(item => item.CategoryId == categoryId && item.Status == Constants.STATUS_LIVE).ToList();
        }

        public IList<ItemMaster> GetItemMastersBySearchText(string searchText)
        {
            if (!String.IsNullOrWhiteSpace(searchText))
            {
                return this.Items.Where(item => item.Status == Constants.STATUS_LIVE && item.ItemDescription.StartsWith(searchText)).ToList();
            }
            return null;
        }

        public IList<ItemMaster> GetItemMastersByFilter(int itemType, long categoryId, string itemDesc, string itemCode, int start = 0, int count = 100)
        {
            var query = this.Items.Where(i => i.Status == Constants.STATUS_LIVE);

            if (!string.IsNullOrWhiteSpace(itemDesc))
            {
                query = query.Where(i => i.ItemDescription.StartsWith(itemDesc));
            }
            if (!string.IsNullOrWhiteSpace(itemCode))
            {
                query = query.Where(i => i.ItemCode.StartsWith(itemCode));
            }
            if (itemType > 0)
            {
                query = query.Where(i => i.ItemType == itemType);
            }
            if (categoryId > 0)
            {
                query = query.Where(i => i.CategoryId == categoryId);
            }

            var result = query
                            .Skip(start)
                            .Take(count)
                            .ToList();
            return result;
        }

        public IList<ItemMaster> GetItemMastersForMap(long categoryId, long supplierId)
        {
            IList<ItemMaster> result = null;
            ItemSupplierMapBO itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();
            if (categoryId > 0 && supplierId > 0)
            {
                var tempItemIds = itemSupplierMapBO.Items.Where(map => map.SupplierId == supplierId).Select(item => item.ItemId).ToList<long>();
                result = this.Items.Where(item => tempItemIds.Contains(item.Id) && item.CategoryId == categoryId).ToList();
            }
            else if (categoryId > 0)
            {
                result = this.Items.Where(item => item.CategoryId == categoryId).ToList();
            }
            else if (supplierId > 0)
            {
                var tempItemIds = itemSupplierMapBO.Items.Where(map => map.SupplierId == supplierId).Select(item => item.ItemId).ToList<long>();
                result = this.Items.Where(item => tempItemIds.Contains(item.Id)).ToList();
            }
            return result;
        }

        public IList<BondedItem> GetBondedItems(long categoryId, long supplierId)
        {
            if (categoryId > 0 || supplierId > 0)
            {

                var itemCategories = BOFactory.GetBO<ItemMasterBO>().Items;
                var itemSuppliers = BOFactory.GetBO<ItemSupplierMapBO>().Items;

                var itemDescs = BOFactory.GetBO<ItemMasterBO>().Items;
                var itemSpecificSuppliers = BOFactory.GetBO<ItemSupplierMapBO>().Items;

                var items = (from ic in itemCategories
                             from itemsupplier in itemSuppliers
                             from itemdesc in itemDescs
                             from itemspec in itemSpecificSuppliers
                             where categoryId == 0 || ic.CategoryId == categoryId
                             where supplierId == 0 || itemsupplier.SupplierId == supplierId
                             where itemdesc.CategoryId == ic.CategoryId || itemdesc.Id == itemsupplier.ItemId
                             where itemspec.ItemId == itemdesc.Id
                             select new BondedItem
                                     {
                                         Id = itemdesc.Id,
                                         ItemDescription = itemdesc.ItemDescription,
                                         Unit = itemdesc.Unit,
                                         CategoryId = itemdesc.CategoryId,
                                         CategoryName = itemdesc.CategoryName,

                                         BuyingPrice = itemspec.BuyingPrice,
                                         Preference = itemspec.Preference,
                                         SupplierId = itemspec.SupplierId,
                                         ItemRemarks = itemspec.ItemRemarks
                                     }
                            )
                            .Skip(0)
                            .Take(1000)
                            .ToList();

                var groups = items.GroupBy(im => im.Id);
                var result = new List<BondedItem>();
                foreach (var group in groups)
                {
                    var im = new BondedItem() { Id = group.Key };
                    foreach (var line in group)
                    {
                        im.ItemDescription = line.ItemDescription;
                        im.Unit = line.Unit;
                        im.CategoryId = line.CategoryId;
                        im.CategoryName = line.CategoryName;
                        var sm = new ItemSupplierMap()
                        {
                            SupplierId = line.SupplierId,
                            Preference = line.Preference,
                            BuyingPrice = line.BuyingPrice,
                            ItemRemarks = line.ItemRemarks
                        };
                        im.Suppliers = im.Suppliers ?? new List<ItemSupplierMap>();
                        im.Suppliers.Add(sm);
                    }
                    result.Add(im);
                }
                return result;

            }
            return Enumerable.Empty<BondedItem>().ToList();
        }

        public dynamic GetOptions(string searchText, int start, int count)
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE && i.ItemType != (int)SupplierType.TechnicalStores)
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

        public dynamic GetTechOptions(string searchText, int start, int count)
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE && i.ItemType == (int)SupplierType.TechnicalStores)
                              .Where(i => i.ItemDescription.StartsWith(searchText))
                              .OrderBy(i => i.ItemDescription)
                              .Select(i => new
                              {
                                  Id = i.Id,
                                  Text = string.Format("{0} {1} {2}", i.ItemDescription, i.Unit, i.Remarks),
                                  Description = i.ItemDescription,
                                  Code = i.ItemCode,
                                  Unit = i.Unit,
                                  Remarks = i.Remarks
                              })
                              .Skip(start)
                              .Take(count)
                              .ToList();
            return options;
        }

        public dynamic GetTechOptionsByCode(string searchText, int start, int count)
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE && i.ItemCode.StartsWith(searchText) && i.ItemType == (int)SupplierType.TechnicalStores)
                              .OrderBy(i => i.ItemDescription)
                              .Select(i => new
                              {
                                  Id = i.Id,
                                  Text = string.Format("{0} {1} {2}", i.ItemDescription, i.Unit, i.Remarks),
                                  Description = i.ItemDescription,
                                  Code = i.ItemCode,
                                  Unit = i.Unit,
                                  Remarks = i.Remarks
                              })
                              .Skip(start)
                              .Take(count)
                              .ToList();
            return options;
        }

        public ItemMasterBO() { }

    }
}
