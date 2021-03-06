﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public class WarlockClass : ICharacterClass
    {
        public CharacterClassType Type { get; set; }
        public List<CharacterSubClassType> SubTypes { get; set; }
        public WarlockClass()
        {
            Type = CharacterClassType.Warlock;
            SubTypes = new List<CharacterSubClassType>
            {
                CharacterSubClassType.TheArchfey,
                CharacterSubClassType.TheFiend,
                CharacterSubClassType.TheGreatOldOne
            };
        }
    }
}
