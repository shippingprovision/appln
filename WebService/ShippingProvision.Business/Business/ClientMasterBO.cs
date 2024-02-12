using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;
using NHibernate;
using NHibernate.Criterion;
using ShippingProvision.Business.Helpers;

namespace ShippingProvision.Business
{
    public class ClientMasterBO : BaseBO<ClientMaster>
    {
        public long AddClientMaster(ClientMaster clientMaster)
        {
            clientMaster.Status = Constants.STATUS_LIVE;

            if (IsClientMasterExists(clientMaster.ClientName))
            {
                throw new Exception("Client already exists.");
            }
            this.SaveOrUpdate(clientMaster);
            return clientMaster.Id;
        }

        public long UpdateClientMaster(ClientMaster clientMaster)
        {
            var existingClient = this.GetById(clientMaster.Id);
            existingClient.ClientName = clientMaster.ClientName;
            existingClient.ClientCode = clientMaster.ClientCode;
            existingClient.ContactPerson = clientMaster.ContactPerson;
            existingClient.Address = clientMaster.Address;
            existingClient.Marking = clientMaster.Marking == null ? string.Empty : clientMaster.Marking; 
            existingClient.Telephone = clientMaster.Telephone;
            existingClient.Fax = clientMaster.Fax;
            existingClient.Email = clientMaster.Email;
            existingClient.Remarks = clientMaster.Remarks;
            existingClient.BillingAddress = clientMaster.BillingAddress;
            
            existingClient.Status = Constants.STATUS_LIVE;

            this.SaveOrUpdate(existingClient);
            return clientMaster.Id;
        }

        public bool IsClientMasterExists(string name)
        {
            var itemId = this.Items
                           .Where(i => i.ClientName== name)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteClientMaster(long id)
        {
            var clientMaster = this.GetById(id);
            if (clientMaster == null)
            {
                throw new Exception("Non-existing Client.");
            }

            this.MarkAsDelete(clientMaster);

            return id;
        }

        public IList<ClientMaster> GetClientMasters()
        {
            return this.Items.Where(item => item.Status == Constants.STATUS_LIVE).ToList();
        }

        public ListResponseVM<ClientMaster> GetClientMastersByFilter(string name, int startIndex, int pageSize)
        {
            var queryable = this.Items.Where(c => c.Status == Constants.STATUS_LIVE);

            if (!string.IsNullOrWhiteSpace(name))
            {
                queryable = queryable.Where(c => c.ClientName.StartsWith(name));
            }
            queryable = queryable.OrderBy(item => item.ClientName);

            var response = queryable.GetPagedResponse(startIndex, pageSize);
            return response;            
        }

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE)
                              .OrderBy(i => i.ClientName)
                              .Select(i => new OptionItem() { Id = i.Id, Text = i.ClientName })                              
                              .ToList();
            return options;
        }

        public ClientMasterBO() { }

    }
}
