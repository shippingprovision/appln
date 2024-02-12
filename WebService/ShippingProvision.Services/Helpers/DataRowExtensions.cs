using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ShippingProvision.Services.Helpers
{
    public static class DataRowExtension
    {
        public static decimal GetValueOrDefault(this DataRow row, string key, decimal defaultValue)
        {
            decimal value = default(decimal);
            try
            {
                value = Convert.ToDecimal(row[key]);
            }
            catch (Exception)
            {
                value = defaultValue;
            }
            return value;
        }

        public static String GetValueOrDefault(this DataRow row, String key, String defaultValue)
        {
            String value = String.Empty;
            try
            {
                value = Convert.ToString(row[key]);
            }
            catch (Exception)
            {
                value = defaultValue;
            }
            return value;
        }

        public static int GetValueOrDefault(this DataRow row, string key, int defaultValue)
        {
            int value = default(int);
            try
            {
                value = Convert.ToInt32(row[key]);
            }
            catch (Exception)
            {
                value = defaultValue;
            }
            return value;
        }

        public static bool GetValueOrDefault(this DataRow row, string key, bool defaultValue)
        {
            bool value = default(bool);
            try
            {
                var boolString = Convert.ToString(row[key]);
                return boolString.Equals("Y", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception)
            {
                value = defaultValue;
            }
            return value;
        }

    }
}
