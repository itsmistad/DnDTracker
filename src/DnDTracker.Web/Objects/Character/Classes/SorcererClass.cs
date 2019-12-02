using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class SorcererClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public SorcererClass()
        {
            Type = CharacterClassType.Sorcerer;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.DraconicBloodline,
                CharacterSubClassType.WildMagic
            };
        }
    }
}
