using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Models
{
    public class SaveNoteModel
    {
        public string Contents, CampaignGuid, NoteGuid;
        public List<string> RecipientGuids;
    }
}
