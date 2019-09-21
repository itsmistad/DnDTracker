using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Configuration
{
    public class ConfigKeys
    {
        public class System
        {
            public static readonly ConfigKey PersistLogs = new ConfigKey("System.PersistLogs", Guid.Parse("0c0f0c6a-ad8e-4f1e-9ac5-0224826622e0"));
            public static readonly ConfigKey WelcomeMessage = new ConfigKey("System.WelcomeMessage", Guid.Parse("42bd0d3f-9103-450d-885b-c7f25216046a"));
            public class Google
            {
                public static readonly ConfigKey SignInId = new ConfigKey("System.Google.SignInId", Guid.Parse("82bf1200-4e97-4542-8f83-92386eddf020"));
            }
        }
    }

    public class ConfigKey
    {
        public string Name;
        public Guid Guid;

        public ConfigKey(string name, Guid guid)
        {
            Name = name;
            Guid = guid;
        }
    }
}
