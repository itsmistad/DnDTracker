using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Models;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using Microsoft.AspNetCore.Mvc;

namespace DnDTracker.Web.Controllers
{
    public class CampaignController : Controller
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
        public IActionResult Create([FromBody] CampaignModel createCampaign)
        {
            try
            {
                var persister = Singleton.Get<DynamoDbPersister>();
                persister.Save(new CampaignObject(createCampaign.Name, createCampaign.Description, createCampaign.Information));
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