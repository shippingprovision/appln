using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;

namespace ShippingProvision.Business
{
    public class SupplierMaster : HistoryEntity<long>
    {
        public virtual string SupplierCode { get; set; }
        public virtual string SupplierName { get; set; }
        public virtual int SupplierType { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual string Address { get; set; }
        public virtual string Telephone1 { get; set; }
        public virtual string Telephone2 { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Email { get; set; }
        public virtual System.Nullable<int> Ranking { get; set; }
        public virtual string RankingRemarks { get; set; }
        public virtual System.Nullable<int> PaymentType { get; set; }
        public virtual string CreditTerms { get; set; }
        public virtual string CreditLimit { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string Impano { get; set; }
        
        public virtual List<ItemSupplierMap> MappedItems { get; set; } 
    }
}
