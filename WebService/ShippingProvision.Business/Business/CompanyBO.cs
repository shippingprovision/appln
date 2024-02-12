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
    public class CompanyBO : BaseBO<Company>
    {
        public long AddCompany(Company company)
        {
            company.Status = Constants.STATUS_LIVE;

            if (IsCompanyExists(company.CompanyCode))
            {
                throw new Exception("Company already exists.");
            }
            this.SaveOrUpdate(company);
            return company.Id;
        }

        public long UpdateCompany(Company company)
        {
            var updated = this.GetById(company.Id);
            updated.CompanyName = company.CompanyName;

            this.SaveOrUpdate(updated);
            return company.Id;
        }

        public bool IsCompanyExists(string companyCode)
        {
            var itemId = this.Items
                           .Where(i => i.CompanyCode == companyCode)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteCompany(long id)
        {
            var company = this.GetById(id);
            if (company == null)
            {
                throw new Exception("Non-existing Vessel Master.");
            }

            this.MarkAsDelete(company);

            return id;
        }

        public override Company GetById(long id)
        {
            Company company = base.GetById(id);
            return company;
        } 

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                                  .Where(i => i.Status == Constants.STATUS_LIVE)
                                  .Select(i => new OptionItem() { Id = i.Id, Text = i.CompanyName, Code = i.CompanyCode })                                  
                                  .ToList();
            return options;
        }

        public CompanyBO() { }
        
    }
}
