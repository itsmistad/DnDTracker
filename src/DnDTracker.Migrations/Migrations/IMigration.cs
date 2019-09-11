using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Migrations.Migrations
{
    public interface IMigration
    {
        void Up();
        void Down();
    }
}
