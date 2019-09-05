using DnDTracker.Web;
using DnDTracker.Web.Configuration;
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
        }

        [OneTimeTearDown]
        public void AfterLast()
        {

        }
    }
}
