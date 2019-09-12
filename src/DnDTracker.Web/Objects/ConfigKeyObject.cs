using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public class ConfigKeyObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Key { get; set; }
        [DynamoDBProperty]
        public string Value { get; set; }

        public ConfigKeyObject() : base() { }

        public ConfigKeyObject(ConfigKey configKey, string value)
        {
            Key = configKey.Name;
            Guid = configKey.Guid;
            Value = value;
        }

        public new void FromDocument(Document document)
        {
            base.FromDocument(document);

            Key = document.TryGetValue("Key", out var entry) ?
                entry.AsString() :
                "";
            Value = document.TryGetValue("Value", out entry) ?
                entry.AsString() :
                "";
        }
    }
}
