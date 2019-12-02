using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Models
{
    public class CreateCharacterModel
    {
        public string Name, Proficiencies, Equipment;

        public int Gender,
            Level,
            Strength,
            Dexterity,
            Constitution,
            Intelligence,
            Wisdom,
            Charisma,
            Class,
            SubClass,
            Race,
            Background,
            Health;
    }
}
