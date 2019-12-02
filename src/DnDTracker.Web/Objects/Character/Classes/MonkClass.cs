using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class MonkClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public MonkClass()
        {
            Type = CharacterClassType.Monk;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.WayOfTheOpenHand,
                CharacterSubClassType.WayOfTheFourElements
            };
        }
    }
}
