using DnDTracker.Web.Models;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using DnDTracker.Web.Services.Objects;
using DnDTracker.Web.Services.Redirect;
using DnDTracker.Web.Services.Session;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Controllers
{
    public class CampaignController : Controller
    {
        public IActionResult Index()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult Create()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult Join()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }
        public IActionResult Dash()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult Note(Guid campaignGuid)
        {
            var persister = Singleton.Get<DynamoDbPersister>();
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            var campaignService = Singleton.Get<CampaignService>();

            if (Guid.TryParse(Singleton.Get<SessionService>().Get("UserGuid", "", controller: this), out var userGuid))
            {
                if (campaignObj != null && campaignService.IsMemberInCampaign(userGuid, campaignObj.Guid))
                {
                    return View();
                }

                return RedirectToAction("Dash", "Campaign");
            }

            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult DungeonMaster(Guid campaignGuid)
        {
            if (Guid.TryParse(Singleton.Get<SessionService>().Get("UserGuid", "", controller: this), out var userGuid))
            {
                var campaignObj = Singleton.Get<DynamoDbPersister>().Get<CampaignObject>(campaignGuid);
                if (campaignObj.DungeonMasterGuid == userGuid)
                {
                    return View();
                }

                return RedirectToAction("Dash", "Campaign");
            }

            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult ViewDetails(Guid campaignGuid)
        {
            if (Guid.TryParse(Singleton.Get<SessionService>().Get("UserGuid", "", controller: this), out var userGuid))
            {
                var campaignService = Singleton.Get<CampaignService>();
                if (campaignService.IsMemberInCampaign(userGuid, campaignGuid))
                {
                    return View();
                }

                return RedirectToAction("Dash", "Campaign");
            }

            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        [HttpPost]
        public IActionResult SaveNote([FromBody] SaveNoteModel saveNote)
        {
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            var campaignService = Singleton.Get<CampaignService>();
            var recipientGuids = new List<Guid>();
            var includeAll = true;
            Guid.TryParse(saveNote.CampaignGuid, out var campaignGuid);
            Guid.TryParse(saveNote.NoteGuid, out var noteGuid);
            foreach (var r in saveNote.RecipientGuids)
            {
                if (Guid.TryParse(r, out var guid))
                {
                    recipientGuids.Add(guid);
                    includeAll = false;
                }
            }
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignService.IsMemberInCampaign(userGuid, campaignGuid))
                    {
                        NoteObject noteObj = null;

                        if (string.IsNullOrEmpty(saveNote.NoteGuid))
                        {
                            if (includeAll)
                                noteObj = new NoteObject(userGuid, campaignGuid, saveNote.Contents, includeAll);
                            else
                                noteObj = new NoteObject(userGuid, campaignGuid, saveNote.Contents, recipientGuids);
                            if (campaignObj.NoteGuids == null) campaignObj.NoteGuids = new List<Guid>();
                            if (!campaignObj.NoteGuids.Contains(noteObj.Guid)) campaignObj.NoteGuids.Add(noteObj.Guid);
                        }
                        else if (noteGuid != Guid.Empty)
                        {
                            noteObj = persister.Get<NoteObject>(noteGuid);
                            noteObj.Contents = saveNote.Contents;
                            noteObj.SharedWithGuids = recipientGuids;
                            noteObj.SharedWithAll = includeAll;
                        }

                        if (noteObj != null)
                        {
                            persister.Save(noteObj);
                            persister.Save(campaignObj);
                        }
                        return Json(new
                        {
                            response = "ok",
                            message = "Saved successfully."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "Sender is not in the campaign."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "CampaignGuid is invalid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult AcceptRequest([FromBody] HandleRequestModel handleRequest)
        {
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            Guid.TryParse(handleRequest.CampaignGuid, out var campaignGuid);
            Guid.TryParse(handleRequest.UserGuid, out var memberGuid);
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignObj.DungeonMasterGuid == userGuid)
                    {
                        campaignObj.PendingGuids.Remove(memberGuid);
                        if (!campaignObj.PendingGuids.Any()) campaignObj.PendingGuids = null;

                        Task.Run(() => persister.Save(campaignObj)).Wait();

                        campaignObj.UserCharacterPairs.Add(new UserCharacterPairModel
                        {
                            UserGuid = memberGuid,
                            CharacterGuid = Guid.Empty
                        });

                        Task.Run(() => persister.Update(campaignObj)).Wait();
                        return Json(new
                        {
                            response = "ok",
                            message = "Saved successfully."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "Sender is not the Dungeon Master."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "CampaignGuid is invalid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult RemoveUser([FromBody] HandleRequestModel handleRequest)
        {
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            Guid.TryParse(handleRequest.CampaignGuid, out var campaignGuid);
            Guid.TryParse(handleRequest.UserGuid, out var memberGuid);
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignObj.DungeonMasterGuid == userGuid)
                    {
                        campaignObj.UserCharacterPairs.RemoveAll(_ => _.UserGuid == memberGuid);
                        if (!campaignObj.UserCharacterPairs.Any()) campaignObj.UserCharacterPairs = null;
                        persister.Save(campaignObj);
                        return Json(new
                        {
                            response = "ok",
                            message = "Saved successfully."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "Sender is not the Dungeon Master."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "CampaignGuid is invalid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult DeclineRequest([FromBody] HandleRequestModel handleRequest)
        {
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            Guid.TryParse(handleRequest.CampaignGuid, out var campaignGuid);
            Guid.TryParse(handleRequest.UserGuid, out var memberGuid);
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignObj.DungeonMasterGuid == userGuid)
                    {
                        campaignObj.PendingGuids.Remove(memberGuid);
                        if (!campaignObj.PendingGuids.Any()) campaignObj.PendingGuids = null;

                        persister.Save(campaignObj);
                        return Json(new
                        {
                            response = "ok",
                            message = "Saved successfully."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "Sender is not the Dungeon Master."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "CampaignGuid is invalid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult Update([FromBody] UpdateCampaignModel updateCampaign)
        {
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            Guid.TryParse(updateCampaign.CampaignGuid, out var campaignGuid);
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignObj.DungeonMasterGuid == userGuid)
                    {
                        if (!string.IsNullOrEmpty(updateCampaign.Description))
                            campaignObj.Description = updateCampaign.Description;
                        if (updateCampaign.Information != null)
                            campaignObj.Information = updateCampaign.Information;

                        persister.Save(campaignObj);
                        return Json(new
                        {
                            response = "ok",
                            message = "Saved successfully."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "Sender is not the Dungeon Master."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "CampaignGuid is invalid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult RequestJoin([FromBody] JoinRequestModel joinRequest)
        {
            var campaignService = Singleton.Get<CampaignService>();
            var campaignObj = campaignService.GetByJoinCode(joinRequest.JoinCode);
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            var persister = Singleton.Get<DynamoDbPersister>();
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignObj.DungeonMasterGuid == userGuid)
                    {
                        return Json(new
                        {
                            response = "err",
                            message = "The specified join code is for your own campaign!<br/>Unfortunately, you can't be your own friend (or party member) :("
                        });
                    }
                    if (campaignService.IsMemberInCampaign(userGuid, campaignObj.Guid))
                    {
                        return Json(new
                        {
                            response = "err",
                            message = "The specified join code is for a campaign you're already in."
                        });
                    }

                    if (!campaignObj.PendingGuids.Contains(userGuid))
                    {
                        campaignObj.PendingGuids.Add(userGuid);
                        persister.Save(campaignObj);
                        return Json(new
                        {
                            response = "ok",
                            message = "Join request is now pending.<br/>Please wait for the Dungeon Master to accept your request."
                        });
                    }

                    return Json(new
                    {
                        response = "ok",
                        message = "Join request already pending.<br/>Please wait for the Dungeon Master to accept your request."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "The specified join code is invalid.<br/>Please double-check for any errors before trying again."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult AddCharacter([FromBody] HandleRequestModel handleRequest)
        {
            var persister = Singleton.Get<DynamoDbPersister>();
            Guid.TryParse(handleRequest.CampaignGuid, out var campaignGuid);
            var campaignObj = persister.Get<CampaignObject>(campaignGuid);
            var campaignService = Singleton.Get<CampaignService>();
            var userGuidStr = Singleton.Get<SessionService>().Get("UserGuid", "", controller: this);
            if (Guid.TryParse(userGuidStr, out var userGuid))
            {
                if (campaignObj != null)
                {
                    if (campaignService.IsMemberInCampaign(userGuid, campaignObj.Guid))
                    {
                        campaignObj.UserCharacterPairs.Find(_ => _.UserGuid == userGuid).CharacterGuid =
                            Guid.Parse(handleRequest.CharacterGuid);

                        persister.Update(campaignObj);
                        return Json(new
                        {
                            response = "ok",
                            message = "Your character in this campaign has been set."
                        });
                    }

                    return Json(new
                    {
                        response = "err",
                        message = "You are not in this campaign."
                    });
                }

                return Json(new
                {
                    response = "err",
                    message = "Invalid CampaignGuid."
                });
            }

            return Json(new
            {
                response = "err",
                message = "UserGuid is missing from session."
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] CampaignModel createCampaign)
        {
            try
            {
                var persister = Singleton.Get<DynamoDbPersister>();
                persister.Save(new CampaignObject(createCampaign.Name, createCampaign.Description,
                    createCampaign.Information, Guid.Parse(createCampaign.DungeonMasterGuid)));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    response = "err",
                    message = $"Campaign creation for \"{createCampaign.Name}\" failed. Exception: " + ex.Message
                });
            }

            return Json(new
            {
                response = "ok",
                message = $"Campaign \"{createCampaign.Name}\" created successfully."
            });
        }
    }
}