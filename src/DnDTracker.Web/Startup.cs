using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDTracker.Web.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnDTracker.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                return context.Response.WriteAsync("Nice.");
            });

            Log.Info("Nice.");
        }
    }
}
