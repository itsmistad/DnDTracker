using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class BardClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }

        public BardClass()
        {
            Type = CharacterClassType.Bard;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.CollegeOfLore,
                CharacterSubClassType.CollegeOfValor
            };
        }
    }
}
