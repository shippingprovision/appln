using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace ShippingProvision.TemplateEngine.Utilities
{
    /// <summary>
    /// Common Utilities
    /// </summary>
    public static class CommonUtils
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Safely execute the delegate which returns the value;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns>any exception returns default value</returns>
        public static T SafeGetValue<T>(Func<T> func)
        {
            T value = default(T);
            try
            {
                value = func();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace, e);
            }
            return value;
        }

        /// <summary>
        /// Safely try to get expected result or default value if failed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <returns>true; returns false if failed; result will be filled or default value if fails</returns>
        public static bool TrySafeGetValue<T>(Func<T> func, out T result)
        {
            try
            {
                result = func();
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
                Log.Warn(e.StackTrace, e);

                result = default(T);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Safely maps the path to physical path irrespective of execution domain
        /// i.e., web, console or unit testing framework
        /// - if rooted path or existing physical path is given; it retains the same path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>physical path</returns>
        public static String SafeGetMapPath(String path)
        {
            string defaultPath = path;

            if (File.Exists(path))
            {
                return path;
            }

            string resultPath;
            if (!TrySafeGetValue(() => HttpContext.Current.Request.MapPath(path), out resultPath))
            {
                if (!TrySafeGetValue(() => Path.GetFullPath(path), out resultPath))
                {
                    if (!TrySafeGetValue(() => Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path), out resultPath))
                    {
                        return defaultPath;
                    }
                }
            }

            return resultPath;
        }
    }
}
