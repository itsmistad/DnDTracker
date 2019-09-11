using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations
{
    public class MigrationAttribute : Attribute
    {
        /// <summary>
        /// The type of migration.
        /// </summary>
        public MigrationType Type { get; private set; }
        /// <summary>
        /// The name of the migration script.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The epoch timestamp of creation. Refer to https://www.epochconverter.com/
        /// </summary>
        public long EpochTimeStamp { get; private set; }

        /// <summary>
        /// The attribute the <see cref="MigrationRunner"/> uses to find migrations.
        /// </summary>
        /// <param name="migrationType">The <see cref="MigrationType"/> the migration is grouped in.</param>
        /// <param name="epochTimeStamp">The epoch timestamp of when the migration was created. This is better than a version number because it prevents version number ambiguity.</param>
        /// <param name="name">The name of the migration. Typically, this is the name of the class.</param>
        public MigrationAttribute(MigrationType migrationType, long epochTimeStamp, string name)
        {
            Type = migrationType;
            EpochTimeStamp = epochTimeStamp;
            Name = name;
        }
    }

    public enum MigrationType
    {
        DynamoDb
    }
}
