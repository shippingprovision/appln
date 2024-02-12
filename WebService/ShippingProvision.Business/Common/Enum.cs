using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business
{
    public enum FilterType
    {
        StartsWith = 1,
        Contains = 2
    }

    public enum QuotationStatus
    {
        Unknown = 1,
        Pending = 1,
        Inprogress = 2,
        Completed = 3,
    }

    public enum SalesOrderStatus
    {
        Unknown = 1,
        Pending = 1,
        Inprogress = 2,
        Completed = 3,
    }

    public enum UserGroup
    {
        User = 1,
        Admin = 2
    }  
    public enum SupplierType
    {
        Unknown = 1,
        Provision = 1,     
        Bonded = 2,     
        Private = 3,
        Stores = 4,
        TechnicalStores = 5,
        Crewing = 6,
        Welfare = 7
    }    
    public enum PayType
    {        
        Cash = 1,     
        Credit = 2
    }

    public enum StockEntryType
    {
        OpenStock = 1,
        PurchaseStock = 2,
        IssueStock = 3,
        AdjustStock = 4,
        ReceiveStock = 5,
        CancelStock = 6,
        ReturnStock = 7
    }

    public enum POStatus
    {
        Inprogress = 1,
        Issued = 2,
        Received = 3,
        Cancelled = 4
    }
}
