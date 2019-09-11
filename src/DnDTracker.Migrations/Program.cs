using DnDTracker.Web;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Persisters;
using System;

namespace DnDTracker.Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting migrator...");

            Console.WriteLine("Registering Singleton instances...");
            Singleton.Initialize()
                .Add<EnvironmentConfig>(new EnvironmentConfig())
                .Add<TableMap>(new TableMap())
                .Add<DynamoDbPersister>(new DynamoDbPersister());

            var timeStampArg = args != null && args.Length > 0 ? args[0] : "";
            Console.WriteLine($"Target timestamp: {timeStampArg}");
            MigrationRunner.Start(long.TryParse(timeStampArg, out var timeStamp) ? (long?)timeStamp : null);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ResetColor();
        }
    }
}
