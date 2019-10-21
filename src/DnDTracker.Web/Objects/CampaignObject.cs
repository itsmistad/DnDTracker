using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public class CampaignObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Name { get; set; }

        public CampaignObject() : base() { } // Required

        public CampaignObject(string name)
        {
            Name = name;
        }

        public new void FromDocument(Document document)
        {
            base.FromDocument(document); // Required

            Name = document.TryGetValue("Name", out var entry) ?
                entry.AsString() :
                "";
        }
    }
}
