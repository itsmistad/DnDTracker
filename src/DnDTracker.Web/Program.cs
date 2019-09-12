using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using DnDTracker.Web.Persisters;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Logging;

namespace DnDTracker.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var singleton = Singleton.Initialize()
                .Add<EnvironmentConfig>(new EnvironmentConfig())
                .Add<TableMap>(new TableMap())
                .Add<DynamoDbPersister>(new DynamoDbPersister())
                .Add<AppConfig>(new AppConfig())
                // Add more global instances here
                ;

            Log.Debug("Global instances registered.");

            Log.Debug("Setting up WebHostBuilder...");
            var host = new WebHostBuilder()
              .UseKestrel()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseIISIntegration()
              .UseStartup<Startup>()
              .Build();

            Log.Debug("Starting WebHost...");
            host.Run();
        }
    }
}
