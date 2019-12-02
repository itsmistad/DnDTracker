using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class FighterClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public FighterClass()
        {
            Type = CharacterClassType.Fighter;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.Champion,
                CharacterSubClassType.BattleMaster,
                CharacterSubClassType.EldritchKnight
            };
        }
    }
}
