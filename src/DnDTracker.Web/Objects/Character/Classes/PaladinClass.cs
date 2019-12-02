using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class PaladinClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public PaladinClass()
        {
            Type = CharacterClassType.Paladin;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.OathOfDevotion,
                CharacterSubClassType.OathOfTheAncients,
                CharacterSubClassType.OathOfVengeance
            };
        }
    }
}
