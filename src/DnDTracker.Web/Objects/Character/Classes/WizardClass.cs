using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class WizardClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public WizardClass()
        {
            Type = CharacterClassType.Wizard;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.SchoolOfAbjuration,
                CharacterSubClassType.SchoolOfConjuration,
                CharacterSubClassType.SchoolOfDivination,
                CharacterSubClassType.SchoolOfEnchantment,
                CharacterSubClassType.SchoolOfEvocation,
                CharacterSubClassType.SchoolOfIllusion,
                CharacterSubClassType.SchoolOfNecromancy,
                CharacterSubClassType.SchoolOfTransmutation
            };
        }
    }
}
