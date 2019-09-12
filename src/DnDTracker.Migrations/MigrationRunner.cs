using DnDTracker.Migrations.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DnDTracker.Migrations
{
    /*
     * No, I didn't add FluentMigrator.
     * Yes, you can add it.
     * Yes, I'm a scrub ._.
     */

    public class MigrationRunner
    {
        /// <summary>
        /// Runs Up() migrations up to and including the timestamp and Down() migrations after and excluding the timestamp.
        /// </summary>
        /// <param name="targetTimeStamp"></param>
        public static void Start(long? targetTimeStamp = null)
        {
            var migrationTypes = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(_ => _.GetCustomAttribute(typeof(MigrationAttribute)) != null)
                .GroupBy(_ => ((MigrationAttribute)_.GetCustomAttribute(typeof(MigrationAttribute))).Type)
                .ToDictionary(
                    _ => _.Key,
                    _ => _.ToList().OrderBy(x => ((MigrationAttribute)x.GetCustomAttribute(typeof(MigrationAttribute))).EpochTimeStamp));

            Console.WriteLine($"Found {migrationTypes.Count} migration types.");
            foreach(var migrationType in migrationTypes)
            {
                Console.WriteLine($"Running {migrationType.Value.Count()} {migrationType.Key.ToString()} migrations:");
                foreach (var migration in migrationType.Value)
                {
                    var attribute = (MigrationAttribute)migration.GetCustomAttribute(typeof(MigrationAttribute));

                    Console.WriteLine($" - [{attribute.EpochTimeStamp}] {attribute.Name}");
                    var migrationInstance = Activator.CreateInstance(migration);
                    var UpMethod = migration.GetMethod("Up");
                    var DownMethod = migration.GetMethod("Down");
                    // If there is a target timestamp and this migration is beyond that timestamp, migrate down. Otherwise, migrate up.
                    if (targetTimeStamp != null && attribute.EpochTimeStamp > targetTimeStamp)
                        DownMethod?.Invoke(migrationInstance, new object[] { });
                    else
                        UpMethod?.Invoke(migrationInstance, new object[] { });
                }
            }
        }
    }
}
