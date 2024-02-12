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
    public class VesselMasterBO : BaseBO<VesselMaster>
    {
        public long AddVesselMaster(VesselMaster vesselMaster)
        {
            vesselMaster.Status = Constants.STATUS_LIVE;

            if (IsVesselMasterExists(vesselMaster.VesselName))
            {
                throw new Exception("Vessel Name already exists.");
            }
            this.SaveOrUpdate(vesselMaster);
            return vesselMaster.Id;
        }

        public long UpdateVesselMaster(VesselMaster vesselMaster)
        {
            var updated = this.GetById(vesselMaster.Id);
            updated.Status = Constants.STATUS_LIVE;
            updated.Rev = vesselMaster.Rev;

            updated.VesselCode = vesselMaster.VesselCode;
            updated.VesselName = vesselMaster.VesselName;
            updated.ClientId = vesselMaster.ClientId;

            this.SaveOrUpdate(updated);
            return vesselMaster.Id;
        }

        public List<long> UpdateVesselMasters(List<VesselMaster> lsVesselMasters)
        {
            List<long> lsResult = new List<long>();
            foreach (var vesselMaster in lsVesselMasters)
            {
                if (vesselMaster.Id != 0)
                {
                    var updated = this.GetById(vesselMaster.Id);
                    updated.Rev = vesselMaster.Rev;
                    updated.Status = Constants.STATUS_LIVE;

                    updated.VesselCode = vesselMaster.VesselCode;
                    updated.VesselName = vesselMaster.VesselName;
                    updated.ClientId = vesselMaster.ClientId;
                    this.SaveOrUpdate(updated);
                }
                else
                {
                    this.SaveOrUpdate(vesselMaster);
                }
                lsResult.Add(vesselMaster.Id);
            }
            return lsResult;
        }

        public bool IsVesselMasterExists(string vesselName)
        {
            var itemId = this.Items
                           .Where(i => i.VesselName == vesselName)
                           .Select(i => i.Id)
                           .FirstOrDefault();
            return itemId != 0;
        }

        public long DeleteVesselMaster(long id)
        {
            var vesselMaster = this.GetById(id);
            if (vesselMaster == null)
            {
                throw new Exception("Non-existing Vessel Master.");
            }

            this.MarkAsDelete(vesselMaster);

            return id;
        }

        public override VesselMaster GetById(long id)
        {
            VesselMaster vesselMaster = base.GetById(id);
            return vesselMaster;
        }

        public IList<VesselMaster> GetVesselMasters()
        {
            ClientMasterBO clientMasterBO = BOFactory.GetBO<ClientMasterBO>();  
            List<VesselMaster> lsResult =  this.Items.Where(item => item.Status == Constants.STATUS_LIVE).ToList();

            lsResult.ForEach(vesselMaster =>
            {
                var clientMaster = clientMasterBO.GetById(vesselMaster.ClientId);
                vesselMaster.ClientName = clientMaster.ClientName;
            });
            return lsResult;
        }

        public IList<VesselMaster> GetVesselsByClientId(long clientId)
        {
            List<VesselMaster> lsResult = this.Items.Where(item => item.ClientId == clientId && item.Status == Constants.STATUS_LIVE).ToList();
            return lsResult;
        }

        public ListResponseVM<VesselMaster> GetVesselMastersByFilter(string searchText, int clientId,int startIndex, int pageSize)
        {
            ClientMasterBO clientMasterBO = BOFactory.GetBO<ClientMasterBO>();

            var querable = this.Items
                               .Where(item => item.Status == Constants.STATUS_LIVE);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                querable = querable.Where(item => item.VesselName.StartsWith(searchText));
            }
            if (clientId > 0)
            {
                querable = querable.Where(item => item.ClientId == clientId);
            }

            querable = querable.OrderBy(item => item.VesselName);

            var response = querable.GetPagedResponse(startIndex, pageSize);

            response.Items.ForEach(vesselMaster =>
            {
                vesselMaster.ClientName = clientMasterBO.Items
                                               .Where(c => c.Id == vesselMaster.ClientId)
                                               .Select(c => c.ClientName)
                                               .FirstOrDefault();
            });
            
            return response;
        }        

        public List<OptionItem> GetOptions(long clientId)
        {
            var options = this.Items
                                  .Where(i => i.Status == Constants.STATUS_LIVE)
                                  .Where(i => i.ClientId == clientId)
                                  .OrderBy(i => i.VesselName)
                                  .Select(i => new OptionItem() { Id = i.Id, Text = i.VesselName })                                  
                                  .ToList();
            return options;
        }

        public VesselMasterBO() { }
        
    }
}
