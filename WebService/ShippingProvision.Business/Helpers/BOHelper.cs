using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingProvision.Business
{
    public class BOHelper
    {
        public static string GetCompanyCodeById(long id)
        {
            return BOFactory.GetBO<CompanyBO>().Items.Where(item => item.Id == id).Select(item => item.CompanyCode).FirstOrDefault();
        }

        public static string GetClientNameById(long id)
        {
            return BOFactory.GetBO<ClientMasterBO>().Items.Where(item => item.Id == id).Select(item => item.ClientName).FirstOrDefault();
        }

        public static string GetSupplierNameById(long id)
        {
            return BOFactory.GetBO<SupplierMasterBO>().Items.Where(item => item.Id == id).Select(item => item.SupplierName).FirstOrDefault();
        }

        public static string GetVesselNameById(long id)
        {
            return BOFactory.GetBO<VesselMasterBO>().Items.Where(item => item.Id == id).Select(item => item.VesselName).FirstOrDefault();
        }

        public static string GetVesselCodeById(long id)
        {
            return BOFactory.GetBO<VesselMasterBO>().Items.Where(item => item.Id == id).Select(item => item.VesselCode).FirstOrDefault();
        }

        public static string GetUserById(long id)
        {
            return BOFactory.GetBO<UserBO>().Items.Where(item => item.Id == id).Select(item => item.LoginName).FirstOrDefault();
        }

        public static dynamic GetItemDetailsById(long id)
        {
            var itemMaster = BOFactory.GetBO<ItemMasterBO>().Items.Where(item => item.Id == id).FirstOrDefault();
            dynamic result = new
                                     {
                                         Id = itemMaster.Id,
                                         Description = itemMaster.ItemDescription,
                                         Unit = itemMaster.Unit,
                                         IsStockItem = itemMaster.IsStockItem
                                     };
            return result;

        }

        public static string GetClientAddress(long? clientId)
        {
            String billingAddress = String.Empty;
            if (clientId.HasValue && clientId.Value > 0)
            {
                billingAddress = BOFactory.GetBO<ClientMasterBO>().Items
                                    .Where(c => c.Id == clientId)
                                    .Select(c => c.Address)
                                    .FirstOrDefault();
            }
            return billingAddress;
        }

        public static string CurrencyInWords(decimal currency, string bigUnitPrefix = "Rs ", string smallUnitSuffix = "Paisa ", string suffix = " Only")
        {
            var IntegerPart = (decimal)Math.Floor((double)currency);
            var DecimalPart = currency - IntegerPart;
            //int Precision = 2;
            DecimalPart = DecimalPart * 100;
            var InWords = bigUnitPrefix + NumberToWords((int)IntegerPart);
            if (DecimalPart > 0)
            {
                InWords += " and " + NumberToWords((int)DecimalPart) + smallUnitSuffix;
            }
            InWords += suffix;
            return InWords;
        }

        public static string CurrencyInWords(string currencyValue, string bigUnitPrefix = "Rs ", string smallUnitSuffix = "Paisa ", string suffix = " Only")
        {
            var parts = currencyValue.Split(new char[] {'.'});
            int integ = 0;
            int decim = 0;
            if (parts.Length == 2)
            {
                integ = Convert.ToInt32(parts[0]);
                decim = Convert.ToInt32(parts[1]);
            }
            if (parts.Length == 1)
            {
                integ = Convert.ToInt32(parts[0]);
                decim = 0;
            }
            var InWords = bigUnitPrefix + NumberToWords(integ);
            if (decim > 0)
            {
                InWords += " and " + NumberToWords(decim) + smallUnitSuffix;
            }
            InWords += suffix;
            return InWords;
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " lac ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

#if !SILVERLIGHT
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words);
#else
                        return words;
#endif
        }
    }
}
