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
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public string Information { get; set; }

        public CampaignObject() : base() { } // Required

        public CampaignObject(string name, string description, string information)
        {
            Name = name;
            Description = description;
            Information = information;
        }

        public new void FromDocument(Document document)
        {
            base.FromDocument(document); // Required

            Name = document.TryGetValue("Name", out var entry) ?
                entry.AsString() :
                "";

            Description = document.TryGetValue("Description", out entry) ?
                entry.AsString() :
                "";

            Information = document.TryGetValue("Information", out entry) ?
                entry.AsString() :
                "";
        }
    }
}
