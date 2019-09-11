using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations
{
    /*
     * Migration naming convention:
     * The first word of your class name should be the action of your migration:
     * - Create
     * - Delete
     * - Modify
     * 
     * The end of the class name should be descriptive of the type of item that is being migrated.
     * Examples:
     * - Database
     * - Table
     * - ConfigKey(s)
     * 
     * Between these two elements, the naming is up to you. Be descriptive in what is being migrated.
     */

    public abstract class AbstractMigration : IMigration
    {
        public void Up()
        {
        }

        public void Down()
        {
        }
    }
}
