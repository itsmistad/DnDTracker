using DnDTracker.Web;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Tests
{
    [SetUpFixture]
    public abstract class UnitTestFixture 
    {
        protected static Singleton MockSingleton;

        [OneTimeSetUp]
        public void BeforeFirst()
        {
            MockSingleton = Singleton.Initialize()
                .Add<EnvironmentConfig>(new EnvironmentConfig());

            Log.IgnoreLogs = true;
        }

        [OneTimeTearDown]
        public void AfterLast()
        {

        }
    }
}
