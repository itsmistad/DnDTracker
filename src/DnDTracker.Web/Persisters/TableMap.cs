using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Persisters
{
    public class TableMap
    {
        private Dictionary<Type, string> _tableNames = new Dictionary<Type, string>();
        private string TableNamePrefix => "dndtracker-" + Singleton.Get<EnvironmentConfig>().Current + "-";

        public TableMap()
        {
            Load();
        }

        private void Load()
        {
            Add<ConfigKeyObject>("configkeys");
            Add<LogObject>("logs");
        }
        
        private void Add<T>(string tableName) where T : IObject
        {
            if (!_tableNames.ContainsKey(typeof(T)))
                _tableNames.Add(typeof(T), TableNamePrefix + tableName);
        }

        public string this[Type t]
        {
            get
            {
                if (_tableNames.ContainsKey(t))
                    return _tableNames[t];
                else
                    return "";
            }
        }
    }
}
