using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    [DynamoDBTable("ConfigKeys")]
    public class ConfigKeyObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Key { get; set; }
        [DynamoDBProperty]
        public string Value { get; set; }

        public ConfigKeyObject() : base() { }

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
