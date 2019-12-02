using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DnDTracker.Web.Objects.Character.Classes
{
    public interface ICharacterClass
    {
        CharacterClassType Type { get; set; }
        List<CharacterSubClassType> SubTypes { get; set; }
    }
}
