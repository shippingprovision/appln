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
    public class ItemSupplierMapBO : BaseBO<ItemSupplierMap>
    {        
        public string UpdateItemSupplierMaps(List<ItemSupplierMap> lsItemSupplierMaps)
        {
            foreach (var itemSupplierMap in lsItemSupplierMaps)
            {
                if (itemSupplierMap.IsNew == 0)
                {
                    var updated = this.Items.FirstOrDefault(item => item.SupplierId == itemSupplierMap.SupplierId &&
                                                    item.ItemId == itemSupplierMap.ItemId);
                    
                    updated.BuyingPrice= itemSupplierMap.BuyingPrice;
                    updated.ItemRemarks = itemSupplierMap.ItemRemarks;
                    updated.Preference = itemSupplierMap.Preference;
                    this.SaveOrUpdate(updated);
                }
                else if(itemSupplierMap.IsNew == 1)
                {
                    this.SaveOrUpdate(itemSupplierMap);
                }
            }
            return "";
        }

        public string SaveItemSupplierMaps(List<ItemSupplierMap> lsItemSupplierMaps)
        {
            var lsresut = new List<long>();
            try
            {
                if (lsItemSupplierMaps != null && lsItemSupplierMaps.Count > 0)
                {
                    //Remove all maps associated with the current item
                    var itemId = lsItemSupplierMaps.FirstOrDefault().ItemId;
                    var list = this.Items.Where(item => item.ItemId == itemId).ToList();
                    foreach (var itemMap in list)
                    {
                        this.Delete(itemMap);
                    }

                    //Save the input map
                    foreach (var itemSupplierMap in lsItemSupplierMaps)
                    {
                        this.Save(itemSupplierMap);
                        lsresut.Add(itemSupplierMap.SupplierId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public string DeleteItemSupplierMap(long itemId, long supplierId)
        {
            var itemSupplierMap = this.Items.FirstOrDefault(item => item.SupplierId == supplierId &&
                                                    item.ItemId == itemId);                    
            if (itemSupplierMap == null)
            {
                throw new Exception("Non-existing Item supplier map.");
            }

            this.Delete(itemSupplierMap);

            return "";
        }

        public List<ItemSupplierMap> GetItemSupplierMapsBySupplierId(long supplierId, int start = 0, int count = 500)
        {
            var items = BOFactory.GetBO<ItemMasterBO>().Items;
            var itemsuppliermap = BOFactory.GetBO<ItemSupplierMapBO>().Items;

            var itemSuppliers = (from map in itemsuppliermap
                                 from item in items
                                 where map.ItemId == item.Id
                                 where map.SupplierId == supplierId
                                 select new ItemSupplierMap()
                                 {
                                     BuyingPrice = map.BuyingPrice,
                                     ItemDescription = item.ItemDescription,
                                     ItemId = map.ItemId,
                                     ItemRemarks = map.ItemRemarks,
                                     ItemUnit = item.Unit,
                                     SupplierId = map.SupplierId,
                                     Preference = map.Preference,
                                     Category = item.CategoryName
                                 })
                                 .Skip(start)
                                 .Take(count)
                                 .ToList();
            return itemSuppliers;
        }

        public List<ItemSupplierMap> GetItemSupplierMapsByItemId(long itemId)
        {
            SupplierMasterBO supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            List<ItemSupplierMap> lsResult = this.Items.Where(item => item.ItemId == itemId).ToList();

            //Not needed as of now
            //lsResult.ForEach(itemSupplierMap =>
            //{
            //    var supplierMaster = supplierMasterBO.GetById(itemSupplierMap.SupplierId);
            //    itemSupplierMap.SupplierName = supplierMaster.SupplierName;                
            //});
            return lsResult;
        }

        public ItemSupplierMapBO() { }

    }
}
