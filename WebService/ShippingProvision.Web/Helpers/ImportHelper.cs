using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

namespace ShippingProvision.Web.Helpers
{
    public class ImportHelper
    {
        public static DataTable ExcelToTable(string pth, string sheetName = "Sheet1")
        {
            string strcon = string.Empty;
            if (System.IO.Path.GetExtension(pth).ToLower().Equals(".xls"))
            {
                strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                + pth +
                                ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            }
            else
            {
                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                              + pth +
                              ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
            }

            string strselect = String.Format("Select * from [{0}$]", sheetName.Replace("'", ""));

            DataTable exDT = new DataTable();
            using (OleDbConnection excelCon = new OleDbConnection(strcon))
            {
                try
                {
                    excelCon.Open();
                    using (OleDbDataAdapter exDA = new OleDbDataAdapter(strselect, excelCon))
                    {
                        exDA.Fill(exDT);
                    }
                }
                catch (OleDbException oledb)
                {
                    throw new Exception(oledb.Message.ToString());
                }
                finally
                {
                    excelCon.Close();
                }
                exDT.AcceptChanges();  // refresh rows changes

                return exDT;
            }
        }

        public static IList<String> GetExcelSheetNames(string fileName)
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            try
            {
                String connString = string.Empty;

                if (System.IO.Path.GetExtension(fileName).ToLower().Equals(".xls"))
                {
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                    + fileName +
                                    ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                }
                else
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                  + fileName +
                                  ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
                }
                objConn = new OleDbConnection(connString);
                objConn.Open();
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                var excelSheets = new List<String>();
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets.Add(row["TABLE_NAME"].ToString().Replace("$", ""));
                }
                return excelSheets;
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }
    }
}