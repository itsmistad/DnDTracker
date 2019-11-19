using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects
{
    public class NoteObject : AbstractObject
    {
        [DynamoDBProperty]
        public Guid AuthorGuid { get; set; }
        [DynamoDBProperty]
        public Guid CampaignGuid { get; set; }
        [DynamoDBProperty]
        public string Contents { get; set; }
        [DynamoDBProperty]
        public List<Guid> SharedWithGuids { get; set; }

        public NoteObject() : base() { }

        public NoteObject(Guid authorGuid, Guid campaignGuid, string contents, List<Guid> sharedWithGuids)
        {
            AuthorGuid = authorGuid;
            CampaignGuid = campaignGuid;
            Contents = contents;
            SharedWithGuids = sharedWithGuids;
        }

        public override void FromDocument(Document document)
        {
            base.FromDocument(document);

            AuthorGuid = document.TryGetValue("AuthorGuid", out var entry) ?
                entry.AsGuid() :
                Guid.Empty;
            CampaignGuid = document.TryGetValue("CampaignGuid", out entry) ?
                entry.AsGuid() :
                Guid.Empty;
            Contents = document.TryGetValue("Contents", out entry) ?
                entry.AsString() :
                "";
            if (document.TryGetValue("SharedWithGuids", out entry))
            {
                SharedWithGuids = new List<Guid>();
                foreach (var v in entry.AsListOfPrimitive())
                    SharedWithGuids.Add(v.AsGuid());
            }
        }
    }
}