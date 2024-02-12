using System.Collections.Generic; 
using System.Text; 
using System;
using ShippingProvision.Services;
using System.Linq;

namespace ShippingProvision.Business{

    public class PurchaseOrder : HistoryEntity<long>
    {
        public PurchaseOrder() { }
        public virtual  string SNo { get; set; }
        public virtual  long SupplierId { get; set; }
        public virtual  String SupplierName { get; set; } //TODO:Compute this on save/update purchase order
        public virtual  long SalesOrderId { get; set; }
        public virtual  string SalesOrderIdentifier { get; set; }
        public virtual  string PurchaseOrderIdentifier { get; set; }
        public virtual  string PurchaseOrderMarking { get; set; }
        public virtual int POStatus { get; set; }

        //To display in stock purchase orders screen
        public virtual long ClientId { get; set; }
        public virtual string ClientName { get; set; }
        public virtual long VesselId { get; set; }
        public virtual string VesselName { get; set; }

        private string _deliveryInstruction = string.Empty;
        public virtual string DeliveryInstruction
        {
            get { return _deliveryInstruction; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _deliveryInstruction = value;
            }
        }

        private string _agent = string.Empty;
        public virtual string Agent
        {
            get { return _agent; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _agent = value;
            }
        }

        private string _place = string.Empty;
        public virtual string Place
        {
            get { return _place; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _place = value;
            }
        }
        public virtual  System.Nullable<int> OrderStatus { get; set; }
        public virtual  System.Nullable<System.DateTime> OrderSentDate { get; set; }
        
        private string _remarks = string.Empty;
        public virtual string Remarks
        {
            get { return _remarks; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks = value;
            }
        }

        public virtual long LineCount
        {
            get;
            set;
        }

        public virtual  decimal? TotalBuying
        {
            get
            {
                return (this.PmPurchaseOrderLines != null) ? this.PmPurchaseOrderLines.Where(p => p.IsIncluded).Sum(p => p.BuyingPrice) : default(decimal);
            }
        }
        public virtual  decimal? TotalPrice
        {
            get
            {
                return (this.PmPurchaseOrderLines != null) ? this.PmPurchaseOrderLines.Where(p => p.IsIncluded).Sum(p => p.TotalPrice) : default(decimal);
            }
        }
        public virtual bool Issued { get; set; }

        public virtual  string PersonIncharge { get; set; }
        public virtual  string Role { get; set; }
        public virtual  string Email { get; set; }
        public virtual  string Phone { get; set; }
        public virtual  string Mobile { get; set; }
        public virtual  string Fax { get; set; }
        public virtual  string Address { get; set; }

        public virtual  string VesselCode { get; set; }

        //public virtual  SupplierMaster PmSupplierMaster { get; set; }
        //public virtual  SalesOrder PmSalesOrder { get; set; }
        public virtual  IList<PurchaseOrderLine> PmPurchaseOrderLines { get; set; }
    }
}
