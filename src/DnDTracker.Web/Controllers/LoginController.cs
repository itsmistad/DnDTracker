using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Logging;
using DnDTracker.Web.Models;
using DnDTracker.Web.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using DnDTracker.Web.Services.Session;

namespace DnDTracker.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Auth([FromBody] AuthUserModel authUser)
        {
            try
            {
                if (authUser != null)
                {
                    Log.Debug($"Received login attempt from {authUser.Email}");
                    var authService = Singleton.Get<AuthService>();
                    var result = authService.Authenticate(this, authUser);
                    return Json(result);
                }
                else
                    Log.Error($"Received bad json during login attempt: {JsonConvert.SerializeObject(authUser)}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetType() + " | " + ex.Message, ex);
            }

            return Json(new
            {
                response = "err",
                message = "Failed to authenticate. Please sign-out and try again."
            });
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            var session = Singleton.Get<SessionService>();
            session.Set("UserGuid", "", null, this);
            session.Set("UserEmail", "", null, this);
            session.Set("UserFirstName", "", null, this);
            session.Set("UserImageUrl", "", null, this);

            return Json(new
            {
                response = "ok",
                message = "Successfully signed out."
            });
        }
    }
}
