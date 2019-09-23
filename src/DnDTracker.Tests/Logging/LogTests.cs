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

        [SetUp]
        public void Setup()
        {
            _persister = new Mock<DynamoDbPersister>();

            MockSingleton
                .Override<DynamoDbPersister>(_persister.Object);

            Log.ForcePersist = true;
        }

        [TearDown]
        public void Teardown()
        {
            Log.ForcePersist = false;
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