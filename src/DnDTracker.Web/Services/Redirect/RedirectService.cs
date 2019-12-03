using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Services.Session;
using Microsoft.AspNetCore.Mvc;

namespace DnDTracker.Web.Services.Redirect
{
    public class RedirectService
    {
        public IActionResult HandleMissingUser(Controller controller, IActionResult view)
        {
            var userGuid = Singleton.Get<SessionService>().Get("UserGuid", "", controller: controller);
            if (string.IsNullOrEmpty(userGuid))
                return controller.RedirectToAction("Index", "Login");
            return view;
        }
    }
}
