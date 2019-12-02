using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Models;
using DnDTracker.Web.Services.Character;
using Microsoft.AspNetCore.Mvc;


namespace DnDTracker.Web.Controllers
{
    public class CharacterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save([FromBody] CreateCharacterModel model)
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
