using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class RogueClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public RogueClass()
        {
            Type = CharacterClassType.Rogue;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.Thief,
                CharacterSubClassType.Assassin,
                CharacterSubClassType.ArcaneTrickster
            };
        }
    }
}
