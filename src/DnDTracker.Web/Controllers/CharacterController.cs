using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Models;
using DnDTracker.Web.Services.Objects;
using DnDTracker.Web.Services.Redirect;
using Microsoft.AspNetCore.Mvc;


namespace DnDTracker.Web.Controllers
{
    public class CharacterController : Controller
    {
        public IActionResult Index()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult Create()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        public IActionResult Add()
        {
            return Singleton.Get<RedirectService>().HandleMissingUser(this, View());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCharacterModel model)
        {
            var characterService = Singleton.Get<CharacterService>();
            if (characterService.Create(this, model))
            {
                return Json(new
                {
                    response = "ok",
                    message = $"Successfully created character \"{model.Name}\""
                });
            }
            
            return Json(new
            {
                response = "err",
                message = "Not yet implemented."
            });
        }

        [HttpPost]
        public IActionResult Update([FromBody] SaveCharacterModel model)
        {
            var characterService = Singleton.Get<CharacterService>();
            if (characterService.Update(this, model))
            {
                return Json(new
                {
                    response = "ok",
                    message = $"Successfully modified character \"{model.Name}\""
                });
            }

            return Json(new
            {
                response = "err",
                message = "Not yet implemented."
            });
        }

        [HttpPost]
        public IActionResult Export()
        {
            return Json(new
            {
                response = "err",
                message = "Not yet implemented."
            });
        }

        public IActionResult ViewDetails()
        {
            return View();
        }

        public IActionResult Dash()
        {
            return View();
        }
    }
}
