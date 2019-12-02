using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects.Character
{
    public enum CharacterSubClassType
    {
        None = 0,
        // Barbarian
        PathOfTheBerserker,
        PathOfTheTotemWarrior,
        // Bard
        CollegeOfLore,
        CollegeOfValor,
        // Cleric
        KnowledgeDomain,
        LifeDomain,
        LightDomain,
        NatureDomain,
        TempestDomain,
        TrickeryDomain,
        WarDomain,
        // Druid
        CircleOfTheLand,
        CircleOfTheMoon,
        // Fighter
        Champion,
        BattleMaster,
        EldritchKnight,
        // Monk
        WayOfTheOpenHand,
        WayOfTheFourElements,
        // Paladin
        OathOfDevotion,
        OathOfTheAncients,
        OathOfVengeance,
        // Ranger
        Hunter,
        BeastMaster,
        // Rogue
        Thief,
        Assassin,
        ArcaneTrickster,
        // Sorcerer
        DraconicBloodline,
        WildMagic,
        // Warlock
        TheArchfey,
        TheFiend,
        TheGreatOldOne,
        // Wizard
        SchoolOfAbjuration,
        SchoolOfConjuration,
        SchoolOfDivination,
        SchoolOfEnchantment,
        SchoolOfEvocation,
        SchoolOfIllusion,
        SchoolOfNecromancy,
        SchoolOfTransmutation
    }
}
