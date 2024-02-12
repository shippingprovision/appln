using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using System.Data;
using ShippingProvision.Services.Helpers;
using System.Collections;
using System.IO;
using System.Configuration;

namespace ShippingProvision.Business
{
    public class SupplierCategoryMapBO : BaseBO<SupplierCategoryMap>
    {        
        public string UpdateSupplierCategoryMaps(List<SupplierCategoryMap> lsSupplierCategoryMaps)
        {
            foreach (var supplierCategoryMap in lsSupplierCategoryMaps)
            {
                if (supplierCategoryMap.IsNew == 0)
                {
                    var updated = this.Items.FirstOrDefault(item => item.SupplierId == supplierCategoryMap.SupplierId &&
                                                    item.CategoryId == supplierCategoryMap.CategoryId);
                    
                    updated.SupplierId= supplierCategoryMap.SupplierId;
                    updated.CategoryId = supplierCategoryMap.CategoryId;
                    this.SaveOrUpdate(updated);
                }
                else if(supplierCategoryMap.IsNew == 1)
                {
                    this.SaveOrUpdate(supplierCategoryMap);
                }
            }
            return "";
        }

        public string SaveSupplierCategoryMaps(List<SupplierCategoryMap> lsSupplierCategoryMaps)
        {
            var lsresut = new List<long>();
            try
            {
                if (lsSupplierCategoryMaps != null && lsSupplierCategoryMaps.Count > 0)
                {
                    //Remove all maps associated with the current item
                    var supplierId = lsSupplierCategoryMaps.FirstOrDefault().SupplierId;
                    var list = this.Items.Where(item => item.SupplierId == supplierId).ToList();
                    foreach (var supplierMap in list)
                    {
                        this.Delete(supplierMap);
                    }

                    //Save the input map
                    foreach (var supplierCategoryMap in lsSupplierCategoryMaps)
                    {
                        this.Save(supplierCategoryMap);
                        lsresut.Add(supplierCategoryMap.SupplierId);
                    }
                }

                CreateJSFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public string DeleteSupplierCategoryMap(long supplierId, long categoryId)
        {
            var supplierCategoryMap = this.Items.FirstOrDefault(item => item.SupplierId == supplierId &&
                                                    item.CategoryId== categoryId);                    
            if (supplierCategoryMap == null)
            {
                throw new Exception("Non-existing Supplier category map.");
            }

            this.Delete(supplierCategoryMap);

            CreateJSFile();

            return "";
        }


        public List<SupplierCategoryMap> GetSupplierCategoryMapsBySupplierId(long supplierId)
        {
            SupplierMasterBO supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            List<SupplierCategoryMap> lsResult = this.Items.Where(item => item.SupplierId == supplierId).ToList();

            return lsResult;
        }

        private void CreateJSFile()
        {
            SupplierMasterBO supplierMasterBO = BOFactory.GetBO<SupplierMasterBO>();
            CategoryBO categoryBO = BOFactory.GetBO<CategoryBO>();

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("var techSupplierCategoryMap = {" + Environment.NewLine);

            //Grouping the suppliers under category
            Dictionary<long, ArrayList> lsMap = new Dictionary<long, ArrayList>();
            foreach (var item in this.Items)
            {
                long categoryId = item.CategoryId;
                ArrayList arrayList = null;
                if (lsMap.ContainsKey(categoryId))
                {
                    arrayList = lsMap[categoryId];
                }
                else
                {
                    arrayList = new ArrayList();
                }
                arrayList.Add(item.SupplierId);
                lsMap[categoryId] = arrayList;
            }

            //Generate JSon string for each category
            foreach (var categoryId in lsMap.Keys)
            {
                strBuilder.Append("\t");
                string catStr = categoryId + ": [";
                ArrayList list = lsMap[categoryId];
                foreach (long supplierId in list)
                {
                    var supplier = supplierMasterBO.Items.First(item => item.Id == supplierId);
                    catStr += String.Format("{{Id : {0}, Text : \"{1}\"}},", supplier.Id, supplier.SupplierName);
                }
                strBuilder.Append(catStr + "],");
                strBuilder.Append(Environment.NewLine);
            }

            strBuilder.Append("};");

            // Create a new file 
            string filePath = ConfigurationManager.AppSettings["RootFolder"];
            string fileName = String.Format("{0}//js//techsuppliercategorymap.js", filePath);
            using (StreamWriter sw = File.CreateText(fileName))
            {
                var result = strBuilder.ToString();
                sw.WriteLine(result);                
            }
        }        

        public SupplierCategoryMapBO() { }

    }
}
