using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web.Logging;
using DnDTracker.Web.Models;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Objects.Character;
using DnDTracker.Web.Persisters;
using DnDTracker.Web.Services.Session;
using Microsoft.AspNetCore.Mvc;

namespace DnDTracker.Web.Services.Character
{
    public class CharacterService
    {
        public bool Create(Controller controller, CreateCharacterModel model)
        {
            var userGuid = Singleton.Get<SessionService>().Get("UserGuid", "", controller: controller);
            try
            {
                if (string.IsNullOrEmpty(model.Name) || model.Name == "..."
                                                     || model.Level == 0
                                                     || model.Strength == 0
                                                     || model.Dexterity == 0
                                                     || model.Constitution == 0
                                                     || model.Intelligence == 0
                                                     || model.Wisdom == 0
                                                     || model.Charisma == 0
                                                     || model.Health == 0
                                                     || string.IsNullOrEmpty(model.Proficiencies) 
                                                     || model.Proficiencies == "..."
                                                     || string.IsNullOrEmpty(model.Equipment)
                                                     || model.Equipment == "..."
                                                     || model.Class == 0
                                                     || model.SubClass == 0
                                                     || model.Race == 0
                                                     || model.Background == 0
                                                     || model.Gender < 0 || model.Gender > 1)
                {
                    Log.Error($"User \"{userGuid}\" attempted to force creation with invalid values.");
                    return false;
                }

                var persister = Singleton.Get<DynamoDbPersister>();
                var characterObj = new CharacterObject(
                    model.Name,
                    model.Level,
                    model.Strength,
                    model.Dexterity,
                    model.Constitution,
                    model.Intelligence,
                    model.Wisdom,
                    model.Charisma,
                    model.Health,
                    model.Proficiencies,
                    model.Equipment,
                    (CharacterClassType) model.Class,
                    (CharacterSubClassType) model.SubClass,
                    (CharacterRaceType) model.Race,
                    (CharacterBackgroundType) model.Background,
                    (CharacterGenderType) model.Gender);

                var userObj = persister.Get<UserObject>(Guid.Parse(userGuid));
                if (userObj.CharacterGuids == null) userObj.CharacterGuids = new List<Guid>();
                userObj.CharacterGuids.Add(characterObj.Guid);

                persister.Save(userObj);
                persister.Save(characterObj);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save new character for user \"{userGuid}\". Exception: {ex.Message}", ex);
            }

            return false;
        }

        public List<CharacterObject> GetByUserGuid(Guid userGuid)
        {
            var persister = Singleton.Get<DynamoDbPersister>();
            var userObj = persister.Get<UserObject>(userGuid);
            var stringGuids = userObj.CharacterGuids.Select(_ => _.ToString()).ToList();
            var expression = new Expression
            {
                ExpressionStatement = "Guid IN :guid",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    { ":guid", stringGuids }
                }
            };
            var results = persister.Scan<CharacterObject>(expression);

            return results;
        }
    }
}
