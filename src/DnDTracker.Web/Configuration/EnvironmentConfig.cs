using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DnDTracker.Web.Configuration
{
    public class EnvironmentConfig
    {
        private readonly Dictionary<string, JToken> _configPairs;
        private readonly string _envFileName;

        public EnvironmentConfig()
        {
            _configPairs = new Dictionary<string, JToken>();
            _envFileName = "env.json";

            Load();
        }

        private void Load()
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), _envFileName);
            if (!File.Exists(envPath))
            {
                SetDefaultValues();
                Log.Error($"Failed to locate {envPath}. Using default settings.");
                return;
            }

            try
            {
                var envObject = JObject.Parse(File.ReadAllText(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    _envFileName)));

                foreach (var pair in envObject)
                {
                    if (_configPairs.ContainsKey(pair.Key))
                        _configPairs[pair.Key] = pair.Value;
                    else
                        _configPairs.Add(pair.Key, pair.Value);
                    Log.Debug($"{pair.Key}={pair.Value}");
                }
                Log.Debug($"Successfully loaded {_envFileName}.");
            }
            catch (Exception ex)
            {
                SetDefaultValues();
                Log.Error($"Failed to parse {_envFileName} into a json object. Using default settings. Exception: {ex.ToString()}");
            }
        }

        private void SetDefaultValues()
        {
            _configPairs.Clear();
            _configPairs.Add("env", Environments.Local);
            _configPairs.Add("aws", JToken.FromObject(new
            {
                access_key = "foo",
                secret_key = "bar",
                endpoint = "http://localhost:8000"
            }));
        }

        /// <summary>
        /// Returns the current environment from <see cref="Environments"/>.
        /// </summary>
        public string Current => _configPairs["env"].ToString();

        public string CurrentHomeUrl
        {
            get
            {
                switch (Current)
                {
                    case Environments.Development:
                        return "//dev.dnd.mistad.net";
                    case Environments.Production:
                        return "//dnd.mistad.net";
                    case Environments.Local:
                    default:
                        return "//localhost:57124";
                }

                return "";
            }
        }

        /// <summary>
        /// Retrieves the value of a specified config key.
        /// </summary>
        /// <param name="key">The environment config key.</param>
        /// <returns>The <see cref="JToken"/> value of the specified config key.</returns>
        public JToken this[string key]
        {
            get
            {
                if (_configPairs.ContainsKey(key))
                    return _configPairs[key];
                
                Log.Error($"Tried retrieving a non-existent config key. Key {key}");
                return null;
            }
        }
    }
    
    public class Environments
    {
        public const string Local = "local";
        public const string Development = "dev";
        public const string Production = "prod";
    }
}
