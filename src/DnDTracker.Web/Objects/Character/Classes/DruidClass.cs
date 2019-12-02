using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class DruidClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public DruidClass()
        {
            Type = CharacterClassType.Druid;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.CircleOfTheLand,
                CharacterSubClassType.CircleOfTheMoon
            };
        }
    }
}
