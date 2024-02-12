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
    public class SupplierMasterBO : BaseBO<SupplierMaster>
    {
        public long AddSupplierMaster(SupplierMaster supplierMaster)
        {
            supplierMaster.Status = Constants.STATUS_LIVE;

            if (IsSupplierMasterExists(supplierMaster.SupplierName))
            {
                throw new Exception("Supplier already exists.");
            }
            this.SaveOrUpdate(supplierMaster);
            return supplierMaster.Id;
        }

        public long UpdateSupplierMaster(SupplierMaster supplierMaster)
        {
            var existingSupplier = this.GetById(supplierMaster.Id);
            
            existingSupplier.SupplierCode = supplierMaster.SupplierCode;
            existingSupplier.SupplierName = supplierMaster.SupplierName;
            existingSupplier.SupplierType = supplierMaster.SupplierType;
            existingSupplier.ContactPerson = supplierMaster.ContactPerson;
            existingSupplier.Address = supplierMaster.Address;
            existingSupplier.Telephone1 = supplierMaster.Telephone1;
            existingSupplier.Telephone2 = supplierMaster.Telephone2;
            existingSupplier.Fax = supplierMaster.Fax;
            existingSupplier.Email = supplierMaster.Email;
            existingSupplier.Ranking = supplierMaster.Ranking;
            existingSupplier.RankingRemarks = supplierMaster.RankingRemarks;
            existingSupplier.PaymentType = supplierMaster.PaymentType;
            existingSupplier.Remarks = supplierMaster.Remarks;
            existingSupplier.CreditTerms = supplierMaster.CreditTerms;
            existingSupplier.CreditLimit = supplierMaster.CreditLimit;
            existingSupplier.Impano = supplierMaster.Impano;

            existingSupplier.Status = Constants.STATUS_LIVE;

            this.SaveOrUpdate(existingSupplier);
            return supplierMaster.Id;
        }

        public bool IsSupplierMasterExists(string name)
        {
            var itemId = this.Items
                           .Where(i => i.SupplierName== name)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteSupplierMaster(long id)
        {
            var supplierMaster = this.GetById(id);
            if (supplierMaster == null)
            {
                throw new Exception("Non-existing Supplier.");
            }

            this.MarkAsDelete(supplierMaster);

            return id;
        }

        public override SupplierMaster GetById(long id)
        {
            ItemSupplierMapBO itemSupplierMapBO = BOFactory.GetBO<ItemSupplierMapBO>();           

            SupplierMaster supplierMaster = base.GetById(id);
            
            //Not needed - Remove after received feedback from Aameer
            //supplierMaster.MappedItems = itemSupplierMapBO.GetItemSupplierMapsBySupplierId(id);

            return supplierMaster;
        }

        public IList<SupplierMaster> GetSupplierMastersByFilter(string code, string name, int supplierType)
        {
            ICriteria criteria = Session.CreateCriteria<SupplierMaster>();
            criteria.Add(Restrictions.Eq("Status", Constants.STATUS_LIVE));

            if(!string.IsNullOrWhiteSpace(code))
            {
                criteria.Add(Restrictions.Like("SupplierCode", code, MatchMode.Start));
            }
            if(!string.IsNullOrWhiteSpace(name))
            {
                criteria.Add(Restrictions.Like("SupplierName", name, MatchMode.Start));
            }
            if(supplierType > 0)
            {
                criteria.Add(Restrictions.Eq("SupplierType", supplierType));
            }
            
            var result = criteria.List<SupplierMaster>();
            return result;            
        }

        public IList<SupplierMaster> GetSupplierMasters()
        {   
            return this.Items.Where(item => item.Status == Constants.STATUS_LIVE).ToList();
        }

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                                .Where(i => i.Status == Constants.STATUS_LIVE)
                                .Where(i => i.SupplierType != (int)SupplierType.TechnicalStores)
                                .OrderBy(i => i.SupplierName)
                                .Select(i => new OptionItem() { Id = i.Id, Text = i.SupplierName })
                                .ToList();
            return options;
        }

        public List<OptionItem> GetTechOptions()
        {
            var options = this.Items
                                .Where(i => i.Status == Constants.STATUS_LIVE)
                                .Where(i => i.SupplierType == (int)SupplierType.TechnicalStores)
                                .OrderBy(i => i.SupplierName)
                                .Select(i => new OptionItem() { Id = i.Id, Text = i.SupplierName })
                                .ToList();
            return options;
        }

        public SupplierMasterBO() { }

        
    }
}
