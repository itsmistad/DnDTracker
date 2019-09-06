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
        private int _cacheExpirationSeconds;
        private Dictionary<string, DateTime> _timeOfRetrieveal;
        private Dictionary<string, object> _configCache;

        public AppConfig()
        {
            _cacheExpirationSeconds = 30;
            _timeOfRetrieveal = new Dictionary<string, DateTime>();

            Load();
        }

        private void Load()
        {
            if (_configCache != null) return;

            _configCache = new Dictionary<string, object>();
            var persister = Singleton.Get<DynamoDbPersister>();
            var results = persister.Scan<ConfigKeyObject>();
            foreach (var configKey in results)
            {
                var key = configKey.Key;
                var value = configKey.Value;
                if (!_configCache.ContainsKey(key))
                    _configCache.Add(key, value);
                if (!_timeOfRetrieveal.ContainsKey(key))
                    _timeOfRetrieveal.Add(key, DateTime.Now);
            }
        }

        public object this[string key]
        {
            get
            {
                // Has this been retrieved before?
                if (_timeOfRetrieveal.ContainsKey(key) && _configCache.ContainsKey(key))
                {
                    var seconds = (DateTime.Now - _timeOfRetrieveal[key]).TotalSeconds;
                    // Has our cached value NOT expired?
                    if (seconds < _cacheExpirationSeconds)
                        return _configCache[key];
                    else
                        _timeOfRetrieveal[key] = DateTime.Now;
                }
                else
                    _timeOfRetrieveal.Add(key, DateTime.Now);

                var persister = Singleton.Get<DynamoDbPersister>();
                var scanFilter = new ScanFilter();
                scanFilter.AddCondition("Key", ScanOperator.Equal, key);
                var result = persister.Scan<ConfigKeyObject>().First();

                if (_configCache.ContainsKey(key))
                    _configCache[key] = result;
                else
                    _configCache.Add(key, result);
                return result;
            }
        }
    }
}
