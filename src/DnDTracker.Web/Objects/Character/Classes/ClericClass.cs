using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class ClericClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public ClericClass()
        {
            Type = CharacterClassType.Cleric;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.KnowledgeDomain,
                CharacterSubClassType.LifeDomain,
                CharacterSubClassType.LightDomain,
                CharacterSubClassType.NatureDomain,
                CharacterSubClassType.TempestDomain,
                CharacterSubClassType.TrickeryDomain,
                CharacterSubClassType.WarDomain
            };
        }
    }
}
