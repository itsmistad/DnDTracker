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
            Console.WriteLine("Starting migrator...");

            Console.WriteLine("Registering Singleton instances...");
            Singleton.Initialize()
                .Add<EnvironmentConfig>(new EnvironmentConfig())
                .Add<TableMap>(new TableMap())
                .Add<DynamoDbPersister>(new DynamoDbPersister());

            long? timestamp = null;
            bool forceSave = false;
            foreach (var arg in args)
            {
                var split = arg.Split("=");
                var key = split[0];
                var val = "";
                if (split.Length > 1)
                    val = split[1];
                switch (key)
                {
                    case "-t":
                    case "--timestamp":
                        if (long.TryParse(val, out var converted))
                        {
                            Console.WriteLine($"Target timestamp: {val}");
                            timestamp = converted;
                        }
                        break;
                    case "-f":
                    case "--force":
                        forceSave = true;
                        Console.WriteLine("Forcing saves regardless of pre-existence.");
                        break;
                }
            }
            MigrationRunner.Start(timestamp, forceSave);

            Console.WriteLine("Done!");
        }
    }
}
