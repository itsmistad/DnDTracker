using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Models
{
    public class SaveCharacterModel
    {
        public string Name, Proficiencies, Equipment, CharacterGuid;

        public int 
            Level,
            Strength,
            Dexterity,
            Constitution,
            Intelligence,
            Wisdom,
            Charisma,
            Health;
    }
}
