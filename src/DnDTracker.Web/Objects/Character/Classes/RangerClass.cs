using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class RangerClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public RangerClass()
        {
            Type = CharacterClassType.Ranger;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.Hunter,
                CharacterSubClassType.BeastMaster
            };
        }
    }
}
