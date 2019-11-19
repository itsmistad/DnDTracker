using Amazon.DynamoDBv2.Model;
using DnDTracker.Migrations.Helpers;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations.Dynamo
{
    [Migration(MigrationType.DynamoDb, 1574186996, "CreateNotesTable")]
    public class CreateNotesTable : AbstractMigration
    {
        public new void Up()
        {
            DynamoDbHelper.CreateTable<NoteObject>();
        }

        public new void Down()
        {
            DynamoDbHelper.DeleteTable<NoteObject>();
        }
    }
}
