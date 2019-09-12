using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Logging;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Configuration
{
    public class AppConfig
    {
        public const int CacheExpirationSeconds = 30;
        public Dictionary<string, DateTime> TimeOfRetrieval;
        public Dictionary<string, string> ConfigCache;

        public AppConfig()
        {
            TimeOfRetrieval = new Dictionary<string, DateTime>();
            ConfigCache = new Dictionary<string, string>();

            Load();
        }

        private void Load()
        {
            var persister = Singleton.Get<DynamoDbPersister>();
            var results = persister.Scan<ConfigKeyObject>();
            if (results != null)
                foreach (var configKey in results)
                {
                    var key = configKey.Key;
                    var value = configKey.Value;
                    if (!ConfigCache.ContainsKey(key))
                        ConfigCache.Add(key, value);
                    if (!TimeOfRetrieval.ContainsKey(key))
                        TimeOfRetrieval.Add(key, DateTime.Now);
                }
        }

        public virtual string this[ConfigKey configKey]
        {
            get
            {
                var key = configKey.Name;
                // Has this been retrieved before?
                if (TimeOfRetrieval.ContainsKey(key) && ConfigCache.ContainsKey(key))
                {
                    var seconds = (DateTime.Now - TimeOfRetrieval[key]).TotalSeconds;
                    // Has our cached value NOT expired?
                    if (seconds < CacheExpirationSeconds)
                        return ConfigCache[key];
                    else
                        TimeOfRetrieval[key] = DateTime.Now;
                }
                else
                    TimeOfRetrieval.Add(key, DateTime.Now);

                var persister = Singleton.Get<DynamoDbPersister>();
                var result = persister.Get<ConfigKeyObject>(configKey.Guid);

                if (result != null)
                    if (ConfigCache.ContainsKey(key))
                        ConfigCache[key] = result.Value;
                    else
                        ConfigCache.Add(key, result.Value);
                return result?.Value;
            }
        }
    }
}
