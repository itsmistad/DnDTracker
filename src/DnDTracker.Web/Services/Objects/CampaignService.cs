using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;

namespace DnDTracker.Web.Services.Objects
{
    public class CampaignService
    {
        private DynamoDbPersister Persister => Singleton.Get<DynamoDbPersister>();

        public bool IsMemberInCampaign(Guid memberGuid, Guid campaignGuid)
        {
            var campaign = Persister.Get<CampaignObject>(campaignGuid);
            return campaign != null && (campaign.DungeonMasterGuid == memberGuid || campaign.UserCharacterPairs.FirstOrDefault(_ => _.UserGuid == memberGuid) != null);
        }

        public List<CampaignObject> GetByDungeonMasterGuid(Guid dungeonMasterGuid)
        {
            var results = Persister.Scan<CampaignObject>(new Expression
            {
                ExpressionStatement = "DungeonMasterGuid = :guid",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                {
                    { ":guid", dungeonMasterGuid }
                }
            });

            return results ?? new List<CampaignObject>();
        }

        public List<CampaignObject> GetByMemberGuid(Guid partyMemberGuid)
        {
            return Persister.Scan<CampaignObject>().FindAll(_ => _.UserCharacterPairs.FirstOrDefault(u => u.UserGuid == partyMemberGuid) != null);
        }

        public CampaignObject GetByJoinCode(string joinCode)
        {
            return Persister.Scan<CampaignObject>().First(_ => _.JoinCode == joinCode);
        }
    }
}
