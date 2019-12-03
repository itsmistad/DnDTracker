using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Models;

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
        [DynamoDBProperty]
        public Guid DungeonMasterGuid { get; set; }
        [DynamoDBProperty]
        public List<UserCharacterPairModel> UserCharacterPairs { get; set; }
        [DynamoDBProperty]
        public List<Guid> NoteGuids { get; set; }
        [DynamoDBProperty]
        public string JoinCode { get; set; }
        [DynamoDBProperty]
        public List<Guid> PendingGuids { get; set; }

        public CampaignObject() : base()  // Required
        {
            UserCharacterPairs = new List<UserCharacterPairModel>();
            NoteGuids = new List<Guid>();
            PendingGuids = new List<Guid>();
        }

        public CampaignObject(string name, string description, string information, Guid dungeonMasterGuid)
        {
            Name = name;
            Description = description;
            Information = information;
            DungeonMasterGuid = dungeonMasterGuid;
            UserCharacterPairs = new List<UserCharacterPairModel>();
            NoteGuids = new List<Guid>();
            PendingGuids = new List<Guid>();

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();
            JoinCode = new string(Enumerable.Repeat(chars, 7)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public override void FromDocument(Document document)
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
            DungeonMasterGuid = document.TryGetValue("DungeonMasterGuid", out entry) ?
                entry.AsGuid() :
                Guid.Empty;
            JoinCode = document.TryGetValue("JoinCode", out entry) ?
                entry.AsString() :
                "";

            UserCharacterPairs = new List<UserCharacterPairModel>();
            NoteGuids = new List<Guid>();
            PendingGuids = new List<Guid>();

            if (document.TryGetValue("UserCharacterPairs", out entry))
            {
                foreach (var v in entry.AsListOfDocument())
                {
                    UserCharacterPairs.Add(new UserCharacterPairModel
                    {
                        UserGuid = v["UserGuid"].AsGuid(),
                        CharacterGuid = v["CharacterGuid"].AsGuid()
                    });
                }
            }
            if (document.TryGetValue("NoteGuids", out entry))
            {
                foreach (var v in entry.AsListOfPrimitive())
                    NoteGuids.Add(v.AsGuid());
            }
            if (document.TryGetValue("PendingGuids", out entry))
            {
                foreach (var v in entry.AsListOfPrimitive())
                    PendingGuids.Add(v.AsGuid());
            }
        }
    }
}
