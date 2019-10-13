using Amazon.DynamoDBv2.Model;
using DnDTracker.Migrations.Helpers;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations.Dynamo
{
    [Migration(MigrationType.DynamoDb, 1570915232, "CreateNotificationConfigKeys")]
    public class CreateNotificationConfigKeys : AbstractMigration
    {
        public new void Up()
        {
            DynamoDbHelper.Save(new ConfigKeyObject(ConfigKeys.System.Notification.CornerPopupClasses, "notify-popup botleft-corner"));
            DynamoDbHelper.Save(new ConfigKeyObject(ConfigKeys.System.Notification.CenterPopupClasses, "notify-popup"));
            var welcomeMessage = DynamoDbHelper.Get<ConfigKeyObject>(ConfigKeys.System.WelcomeMessage.Guid);
            welcomeMessage.Value = "Please forgive any issues you encounter while the app is still in development.<br/>Be sure to check out our <a href=\"https://github.com/itsmistad/DnDTracker\">GitHub</a> for updates.";
            DynamoDbHelper.Save(welcomeMessage);
        }

        public new void Down()
        {
            DynamoDbHelper.Delete<ConfigKeyObject>(ConfigKeys.System.Notification.CornerPopupClasses.Guid);
            DynamoDbHelper.Delete<ConfigKeyObject>(ConfigKeys.System.Notification.CenterPopupClasses.Guid);
        }
    }
}
