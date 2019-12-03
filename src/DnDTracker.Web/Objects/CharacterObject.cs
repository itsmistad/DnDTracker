using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Objects.Character;
using DnDTracker.Web.Objects.Character.Classes;

namespace DnDTracker.Web.Objects
{
    public class CharacterObject : AbstractObject
    {
        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public int Level { get; set; }
        [DynamoDBProperty]
        public int Strength { get; set; }
        [DynamoDBProperty]
        public int Dexterity { get; set; }
        [DynamoDBProperty]
        public int Constitution { get; set; }
        [DynamoDBProperty]
        public int Intelligence { get; set; }
        [DynamoDBProperty]
        public int Wisdom { get; set; }
        [DynamoDBProperty]
        public int Charisma { get; set; }
        [DynamoDBProperty]
        public int Health { get; set; }
        [DynamoDBProperty]
        public string Proficiencies { get; set; }
        [DynamoDBProperty]
        public string Equipment { get; set; }
        [DynamoDBProperty]
        public CharacterClassType ClassType { get; set; }
        [DynamoDBProperty]
        public CharacterSubClassType SubClassType { get; set; }
        [DynamoDBProperty]
        public CharacterRaceType RaceType { get; set; }
        [DynamoDBProperty]
        public CharacterBackgroundType BackgroundType { get; set; }
        [DynamoDBProperty]
        public CharacterGenderType GenderType { get; set; }
        [DynamoDBProperty]
        public Guid UserGuid { get; set; }

        public ICharacterClass Class
        {
            get
            {
                var classMap = Singleton.Get<CharacterClassMap>();
                return classMap[ClassType];
            }
        }

        public CharacterObject() : base() { }

        public CharacterObject(string name, int level, int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, int health, string proficiencies, string equipment,
            CharacterClassType classType, CharacterSubClassType subClassType, CharacterRaceType raceType, CharacterBackgroundType backgroundType, CharacterGenderType genderType, Guid userGuid)
        {
            Name = name;
            Level = level;
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constitution;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Charisma = charisma;
            Health = health;
            Proficiencies = proficiencies;
            Equipment = equipment;
            ClassType = classType;
            SubClassType = subClassType;
            RaceType = raceType;
            BackgroundType = backgroundType;
            GenderType = genderType;
            UserGuid = userGuid;
        }

        public override void FromDocument(Document document)
        {
            base.FromDocument(document);

            Name = document.TryGetValue("Name", out var entry) ? entry.AsString() : "";
            Level = document.TryGetValue("Level", out entry) ? entry.AsInt() : 0;
            Strength = document.TryGetValue("Strength", out entry) ? entry.AsInt() : 0;
            Dexterity = document.TryGetValue("Dexterity", out entry) ? entry.AsInt() : 0;
            Constitution = document.TryGetValue("Constitution", out entry) ? entry.AsInt() : 0;
            Intelligence = document.TryGetValue("Intelligence", out entry) ? entry.AsInt() : 0;
            Wisdom = document.TryGetValue("Wisdom", out entry) ? entry.AsInt() : 0;
            Charisma = document.TryGetValue("Charisma", out entry) ? entry.AsInt() : 0;
            Health = document.TryGetValue("Health", out entry) ? entry.AsInt() : 0;
            Proficiencies = document.TryGetValue("Proficiencies", out entry) ? entry.AsString() : "";
            Equipment = document.TryGetValue("Equipment", out entry) ? entry.AsString() : "";
            ClassType = document.TryGetValue("ClassType", out entry) ? (CharacterClassType)entry.AsInt() : 0;
            SubClassType = document.TryGetValue("SubClassType", out entry) ? (CharacterSubClassType)entry.AsInt() : 0;
            RaceType = document.TryGetValue("RaceType", out entry) ? (CharacterRaceType)entry.AsInt() : 0;
            BackgroundType = document.TryGetValue("BackgroundType", out entry) ? (CharacterBackgroundType)entry.AsInt() : 0;
            GenderType = document.TryGetValue("GenderType", out entry) ? (CharacterGenderType)entry.AsInt() : 0;
            UserGuid = document.TryGetValue("UserGuid", out entry) ? entry.AsGuid() : Guid.Empty;
        }
    }
}
