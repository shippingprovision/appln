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
    public class CategoryBO : BaseBO<PMCategory>
    {
        public long AddCategory(PMCategory category)
        {
            category.Status = Constants.STATUS_LIVE;

            if (IsCategoryExists(category.Name))
            {
                throw new Exception("Category already exists.");
            }
            this.SaveOrUpdate(category);
            return category.Id;
        }

        public long UpdateCategory(PMCategory category)
        {
            category.Status = Constants.STATUS_LIVE;
            this.SaveOrUpdate(category);
            return category.Id;
        }

        public IList<long> UpdateCategorys(IList<PMCategory> categorys)
        {
            List<long> lsResult = new List<long>();
            foreach (var newCat in categorys)
            {
                if (newCat.Id != 0)
                {
                    var existingCat = this.GetById(newCat.Id);
                    existingCat.Code = newCat.Code;
                    existingCat.Name = newCat.Name;
                    existingCat.Description = newCat.Description;
                    existingCat.Type = newCat.Type;
                    existingCat.Rev = newCat.Rev;
                    existingCat.Status = Constants.STATUS_LIVE;
                    this.Update(existingCat);
                }
                else
                {
                    this.SaveOrUpdate(newCat);
                }

                lsResult.Add(newCat.Id);
            }

            return lsResult;
        }

        public bool IsCategoryExists(string name)
        {
            var itemId = this.Items
                           .Where(i => i.Name== name)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteCategory(long id)
        {
            var category = this.GetById(id);
            if (category == null)
            {
                throw new Exception("Non-existing Category.");
            }

            this.MarkAsDelete(category);           

            return id;
        }

        public IList<PMCategory> GetCategorysByFilter(string name, int type)
        {
            ICriteria criteria = Session.CreateCriteria<PMCategory>();
            criteria.Add(Restrictions.Eq("Status", Constants.STATUS_LIVE));

            if (!string.IsNullOrWhiteSpace(name))
            {
                criteria.Add(Restrictions.Like("Name", name, MatchMode.Start));
            }
            if (type > 0)
            {
                criteria.Add(Restrictions.Eq("Type", type));
            }

            var result = criteria.List<PMCategory>();
            return result;
        }


        public IList<PMCategory> GetCategorys()
        {
            return this.Items.Where(item => item.Status == Constants.STATUS_LIVE).ToList();
        }

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                                .Where(i => i.Status == Constants.STATUS_LIVE && i.Type != (int)SupplierType.TechnicalStores)
                                .OrderBy(i => i.Name)
                                .Select(i => new OptionItem() { Id = i.Id, Text = i.Name })
                                .ToList();
            return options;
        }

        public List<OptionItem> GetTechOptions()
        {
            var options = this.Items
                                .Where(i => i.Status == Constants.STATUS_LIVE && i.Type == (int)SupplierType.TechnicalStores)
                                .OrderBy(i => i.Name).ToList();
           
            var result = new List<OptionItem>();
            foreach(var item in options) 
            {
                var optionItem = new OptionItem();
                optionItem.Id = item.Id;

                string text = item.Name;
                if (!String.IsNullOrEmpty(item.Code))
                {
                    text = item.Code + " - " + text;
                }
                
                optionItem.Text = text;
                result.Add(optionItem);
            }
            return result;
        }

        public CategoryBO() { }

    }
}
