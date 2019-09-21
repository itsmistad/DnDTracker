using Amazon.DynamoDBv2.Model;
using DnDTracker.Migrations.Helpers;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations.Dynamo
{
    [Migration(MigrationType.DynamoDb, 1569101756, "CreateGoogleSignInIdConfigKey")]
    public class CreateGoogleSignInIdConfigKey : AbstractMigration
    {
        public new void Up()
        {
            DynamoDbHelper.Save(new ConfigKeyObject(
                ConfigKeys.System.Google.SignInId, 
                "803910589055-mo49e7cm87l9glprn38aqrf211sog67u.apps.googleusercontent.com"));
        }

        public new void Down()
        {
            DynamoDbHelper.Delete<ConfigKeyObject>(ConfigKeys.System.Google.SignInId.Guid);
        }
    }
}
