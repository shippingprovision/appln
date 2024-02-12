using System.Collections.Generic; 
using System.Text; 
using System;
using ShippingProvision.Services; 


namespace ShippingProvision.Business {
    
    public class QuotationLine : HistoryEntity<long> {
        public QuotationLine() { }
        public virtual  string SNo { get; set; }
        public virtual  string LineIdentifier { get; set; }
        public virtual  long QuoteId { get; set; }
        public virtual  System.Nullable<long> ItemId { get; set; }
        public virtual string ItemCode { get; set; }
        public virtual  string Description { get; set; }
        public virtual  string Unit { get; set; }
        public virtual  decimal Quantity { get; set; }
        public virtual  decimal SellingPrice { get; set; }
        public virtual  long? SupplierId1 { get; set; }
        public virtual  string Supplier1 { get; set; }
        public virtual  decimal BuyingPrice1 { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual long LineId
        {
            get
            {
                long lineId = 0;
                if (!Int64.TryParse(this.SNo, out lineId))
                {
                    lineId = this.Id;
                }
                return lineId;
            }
        }    
        
        private string _remarks1 = string.Empty;
        public virtual string Remarks1
        {
            get { return _remarks1; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks1 = value;
            }
        }

        public virtual  bool Preferred1 { get; set; }
        public virtual  long? SupplierId2 { get; set; }
        public virtual  string Supplier2 { get; set; }
        public virtual  decimal BuyingPrice2 { get; set; }
        
        private string _remarks2 = string.Empty;
        public virtual string Remarks2
        {
            get { return _remarks2; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks2 = value;
            }
        }

        public virtual  bool Preferred2 { get; set; }
        public virtual  long? SupplierId3 { get; set; }
        public virtual  string Supplier3 { get; set; }
        public virtual  decimal BuyingPrice3 { get; set; }

        private string _remarks3 = string.Empty;
        public virtual string Remarks3 
        {
            get { return _remarks3; }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _remarks3 = value;
            }
        }
        public virtual  bool Preferred3 { get; set; }
        public virtual  decimal AvailableQuantity { get; set; }
        public virtual System.Nullable<long> StockItemId { get; set; }
    }
}
