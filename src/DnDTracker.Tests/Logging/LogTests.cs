using NUnit.Framework;
using DnDTracker.Web.Logging;
using Moq;
using DnDTracker.Web.Persisters;
using System;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Configuration;

namespace DnDTracker.Tests
{
    /// <summary>
    /// This is an example of unit testing with NUnit against a type's non-static methods.
    /// Static methods cannot be mocked using Moq directly.
    /// Instead, you need to create an instantiable wrapper that calls those methods.
    /// Methods that function as "passthroughs" (methods that do not alter behavior and do not compare conditions) do not need tests.
    /// </summary>
    [TestFixture]
    public class LogTests : UnitTestFixture
    {
        private Mock<DynamoDbPersister> _persister;
        private Mock<AppConfig> _appConfig;

        [SetUp]
        public void Setup()
        {
            _persister = new Mock<DynamoDbPersister>();
            _appConfig = new Mock<AppConfig>();

            _appConfig.SetupGet(_ => _[It.IsAny<ConfigKey>()])
                .Returns("true");

            MockSingleton
                .Override<DynamoDbPersister>(_persister.Object)
                .Override<AppConfig>(_appConfig.Object);
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldLogDebugWhenDebugFlagIsTrue()
        {
            // Execute
            Log.Debug("Test message");

            // Assert
            _persister.Verify(_ => _.Save(
                It.Is<IObject>(x => x is LogObject && ((LogObject)x).Message == "Test message")), 
                Times.Once);
        }
    }
}