using Amazon.DynamoDBv2.Model;
using DnDTracker.Migrations.Helpers;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations.Dynamo
{
    [Migration(MigrationType.DynamoDb, 1574038026, "CreateCharactersTable")]
    public class CreateCharactersTable : AbstractMigration
    {
        public new void Up()
        {
            DynamoDbHelper.CreateTable<CharacterObject>();
        }

        public new void Down()
        {
            DynamoDbHelper.DeleteTable<CharacterObject>();
        }
    }
}
