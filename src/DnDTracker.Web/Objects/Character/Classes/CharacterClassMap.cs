using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Objects.Character.Classes
{
    public class CharacterClassMap
    {
        private readonly Dictionary<CharacterClassType, ICharacterClass> _characterClasses;

        public CharacterClassMap()
        {
            _characterClasses = new Dictionary<CharacterClassType, ICharacterClass>
            {
                { CharacterClassType.Barbarian, new BarbarianClass() },
                { CharacterClassType.Bard, new BardClass()},
                { CharacterClassType.Cleric, new ClericClass()},
                { CharacterClassType.Druid, new DruidClass()},
                { CharacterClassType.Fighter, new FighterClass()},
                { CharacterClassType.Monk, new MonkClass()},
                { CharacterClassType.Paladin, new PaladinClass()},
                { CharacterClassType.Ranger, new RangerClass()},
                { CharacterClassType.Rogue, new RogueClass()},
                { CharacterClassType.Sorcerer, new SorcererClass()},
                { CharacterClassType.Warlock, new WarlockClass()},
                { CharacterClassType.Wizard, new WizardClass()}
            };
        }

        public ICharacterClass this[CharacterClassType classType] => _characterClasses.TryGetValue(classType, out var val) ? val : null;
    }
}
