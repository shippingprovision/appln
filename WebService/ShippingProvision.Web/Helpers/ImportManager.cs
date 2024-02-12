using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ShippingProvision.Business;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Data;
using ShippingProvision.Business.ViewModels;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace ShippingProvision.Web.Helpers
{
    public class ImportManager 
    {
        public Int64 ImportQuotationFromCsv(Quotation quotation, FileInfo fileInfo)
        {
            QuotationBO quotationBO = BOFactory.GetBO<QuotationBO>();
            return quotationBO.ImportQuotation(quotation, GetQuotationLines(fileInfo));
        }

        public Int64 ImportSalesOrderFromCsv(Quotation quotation, FileInfo fileInfo)
        {
            QuotationBO quotationBO = BOFactory.GetBO<QuotationBO>();
            return quotationBO.ImportQuotation(quotation, GetSalesOrderLines(fileInfo));
        }

        public Int64 ImportStoreOrderFromCsv(Quotation quotation, FileInfo fileInfo)
        {
            QuotationBO quotationBO = BOFactory.GetBO<QuotationBO>();
            return quotationBO.ImportQuotation(quotation, GetStoreOrderLines(fileInfo));
        }

        public IEnumerable<QuotationLine> GetQuotationLines(FileInfo fileInfo)
        {
            if(fileInfo == null)
            {
                return Enumerable.Empty<QuotationLine>();
            }
            using (var readableStream = fileInfo.OpenRead())
            {
                var lines = ImportCsvService.ReadRows<QuotationLine>(readableStream);
                return lines;
            }
        }
        
        public IEnumerable<QuotationLine> GetSalesOrderLines(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return Enumerable.Empty<QuotationLine>();
            }
            using (var readableStream = fileInfo.OpenRead())
            {
                var lines = ImportCsvService.ReadRows<QuotationLine>(readableStream);
                return lines;
            }
        }

        public IEnumerable<QuotationLine> GetStoreOrderLines(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return Enumerable.Empty<QuotationLine>();
            }
            using (var readableStream = fileInfo.OpenRead())
            {
                var lines = ImportCsvService.ReadRows<QuotationLine>(readableStream);
                return lines;
            }
        }


        public Int32 ImportStocksFromCsv(FileInfo fileInfo)
        {
            StockMasterBO stockMasterBO = BOFactory.GetBO<StockMasterBO>();
            stockMasterBO.ImportStockItems(GetStockItems(fileInfo));
            return 0;
        }

        private IEnumerable<StockMasterRow> GetStockItems(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return Enumerable.Empty<StockMasterRow>();
            }
            using (var readableStream = fileInfo.OpenRead())
            {
                var lines = ImportCsvService.ReadRows<StockMasterRow>(readableStream);
                return lines;
            }
        }
        
    }

    public class QuotationLineCsvMap : CsvClassMap<QuotationLine>
    {
        //1 SNO
        //2 DESC
        //3 Unit
        //4 Quantity
        //5 SP
        //6 Remarks
        public QuotationLineCsvMap()
        {
            this.Map(q => q.SNo).Index(1);
            this.Map(q => q.Description).Index(2);
            this.Map(q => q.Unit).Index(3);
            this.Map(q => q.Quantity).Index(4);
            this.Map(q => q.SellingPrice).Index(5);
            this.Map(q => q.Remarks1).Index(6);
        }
    }

    public class StockMasterRowCsvMap : CsvClassMap<StockMasterRow>
    {
        //SNo 1
        //Code 2
        //Description 3
        //Unit 4
        //Quantity 5
        //SupplierCode 6
        //BP 7        
        //Category 8
        public StockMasterRowCsvMap()
        {
            this.Map(s => s.SNo).Index(1);
            this.Map(s => s.ItemCode).Index(2);
            this.Map(s => s.Description).Index(3);
            this.Map(s => s.Unit).Index(4);
            this.Map(s => s.Quantity).Index(5);
            this.Map(s => s.SupplierCode).Index(6);
            this.Map(s => s.BuyingPrice).Index(7);
            this.Map(s => s.CategoryName).Index(8);
        }
    }

    public static class ImportCsvService
    {
        static ImportCsvService()
        {
            RegisterMaps();
        }
        private static CsvConfiguration _csvConfiguration;
        private static void RegisterMaps()
        {
            _csvConfiguration = new CsvConfiguration();
            _csvConfiguration.RegisterClassMap<QuotationLineCsvMap>();
            _csvConfiguration.RegisterClassMap<StockMasterRowCsvMap>();
        }

        public static IEnumerable<TEntity> ReadRows<TEntity>(Stream stream)
            where TEntity : class, new()
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, _csvConfiguration))
            {
                var records = csv.GetRecords<TEntity>();
                return records.ToList();
            }
        }
    }
}
