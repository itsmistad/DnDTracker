using Amazon.DynamoDBv2.Model;
using DnDTracker.Migrations.Helpers;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations.Dynamo
{
    [Migration(MigrationType.DynamoDb, 1568152737, "CreateBaseDatabase")]
    public class CreateBaseDatabase : AbstractMigration
    {
        public new void Up()
        {
            DynamoDbHelper.CreateTable<ConfigKeyObject>();
            DynamoDbHelper.CreateTable<LogObject>();

            DynamoDbHelper.Save(new ConfigKeyObject(ConfigKeys.System.PersistLogs, "true"));
            DynamoDbHelper.Save(new ConfigKeyObject(ConfigKeys.System.WelcomeMessage, "Nice."));
        }

        public new void Down()
        {
            DynamoDbHelper.DeleteTable<ConfigKeyObject>();
            DynamoDbHelper.DeleteTable<LogObject>();
        }
    }
}
