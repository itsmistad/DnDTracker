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
using Microsoft.AspNetCore;

namespace DnDTracker.Web
{
    public class Program
    {
        public static void Main()
        {
            Singleton.Initialize()
                .Add<EnvironmentConfig>(new EnvironmentConfig())
                .Add<TableMap>(new TableMap())
                .Add<DynamoDbPersister>(new DynamoDbPersister())
                .Add<AppConfig>(new AppConfig())
                // Add more global instances here
                ;

            Log.Debug("Global instances registered.");

            Log.Debug("Setting up WebHost...");
            CreateWebHostBuilder().Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
