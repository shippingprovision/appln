using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Runtime.Caching;
using System.Configuration;

namespace ShippingProvision.Services.Caching
{
    public class ObjectCacheProvider
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string ExpirationPolicy = "ExpirationPolicy";
        public const string DefaultPolicy = "DefaultPolicy";

        private static int sessionTimeoutInMins = 20; //default
        private static int SessionTimeoutInMins { 
            get{
                var minString = ConfigurationManager.AppSettings["SessionTimeoutInMins"];
                if (!string.IsNullOrEmpty(minString))
                {
                    int mins;
                    if (!Int32.TryParse(minString, out mins))
                    {
                        mins = 20; //default
                    }
                    sessionTimeoutInMins = mins;
                }
                return sessionTimeoutInMins;
            }
        }

        private static readonly ObjectCache ObjectCache = MemoryCache.Default;

        private static readonly Dictionary<string, CacheItemPolicy> policy = new Dictionary<string, CacheItemPolicy>(){
            { ExpirationPolicy, new CacheItemPolicy() {
                SlidingExpiration = new TimeSpan(0, SessionTimeoutInMins, 0),
                RemovedCallback = CacheRemovedCallback
            }},
            { DefaultPolicy, new CacheItemPolicy() {
                Priority = CacheItemPriority.Default
            }}
        };

        public static bool AddItem(string key, object value, string policyKey = DefaultPolicy)
        {
            var addedCacheEntry = ObjectCache.AddOrGetExisting(key, value, policy[policyKey]);
            Log.Debug(string.Format("Request Add Key:{0} Status:{1}", key, addedCacheEntry == null));
            return addedCacheEntry == null;
        }

        public static TValue GetItem<TValue>(string key) where TValue : class
        {
            var value = ObjectCache.Get(key) as TValue;
            Log.Debug(string.Format("Request Get Key:{0} Status:{1}", key, value != default(TValue)));
            return value;
        }

        public static bool ContainsKey(string key)
        {
            var exists = ObjectCache.Contains(key);
            Log.Debug(string.Format("Request Contains Key:{0} Status:{1}", key, exists));
            return exists;
        }

        public static TValue RemoveKey<TValue>(string key) where TValue : class
        {
            var removed = ObjectCache.Remove(key);
            Log.Debug(string.Format("Request Remove Key:{0} Status:{1}", key, removed != null));
            return removed != null ? (removed as TValue) : default(TValue);
        }

        public static IEnumerable<TValue> GetValues<TValue>(Func<KeyValuePair<string, object>, bool> predicate)
        {
            var values = ObjectCache.Where(predicate)
                                  .Select(kv => kv.Value)
                                  .Cast<TValue>()
                                  .ToList();
            return values;
        }

        public static void ClearAll()
        {
            var keys = ObjectCache.Select(kv => kv.Key).ToList();
            keys.ForEach(key => ObjectCache.Remove(key));
            Log.Debug(string.Format("Request Clear All Keys Count:{0}", keys.Count));
        }

        private static void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            var cacheItem = arguments.CacheItem;
            Log.Debug(string.Format("Callback Removed Cache Key:{0} Reason:{1}", cacheItem.Key, arguments.RemovedReason));

            //TODO: handle removed event            
        }
    }
}
