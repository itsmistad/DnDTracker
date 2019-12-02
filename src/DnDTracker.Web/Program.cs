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
using DnDTracker.Web.Objects.Character.Classes;
using Microsoft.AspNetCore;
using DnDTracker.Web.Services.Auth;
using DnDTracker.Web.Services.Character;
using DnDTracker.Web.Services.Session;

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
                .Add<AuthService>(new AuthService())
                .Add<SessionService>(new SessionService())
                .Add<CharacterClassMap>(new CharacterClassMap())
                .Add<CharacterService>(new CharacterService())
                // Add more global instances here
                ;

            if (!Singleton.Get<AppConfig>().CanContinue) return;

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
