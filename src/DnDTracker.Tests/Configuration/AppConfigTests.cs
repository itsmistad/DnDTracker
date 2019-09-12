using Amazon.DynamoDBv2.DocumentModel;
using DnDTracker.Web;
using DnDTracker.Web.Configuration;
using DnDTracker.Web.Objects;
using DnDTracker.Web.Persisters;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnDTracker.Tests.Configuration
{
    [TestFixture]
    public class AppConfigTests : UnitTestFixture
    {
        private Mock<DynamoDbPersister> _persister;

        [SetUp]
        public void Setup()
        {
            _persister = new Mock<DynamoDbPersister>();
            _persister.Setup(
                _ => _.Scan<ConfigKeyObject>(It.IsAny<ScanFilter>()))
                .Returns(new List<ConfigKeyObject>()
                {
                    new ConfigKeyObject(ConfigKeys.System.PersistLogs, "true")
                });
            _persister.Setup(
                _ => _.Get<ConfigKeyObject>(It.IsAny<Guid>()))
                .Returns(new ConfigKeyObject(ConfigKeys.System.PersistLogs, "false"));

            MockSingleton
                .Override<DynamoDbPersister>(_persister.Object)
                .Override<AppConfig>(new AppConfig());
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldRetrieveFromDynamoOnFirstTime()
        {
            // Setup
            var appConfig = Singleton.Get<AppConfig>();
            appConfig.TimeOfRetrieval.Clear();
            var startTime = DateTime.Now;

            // Execute
            var result = appConfig[ConfigKeys.System.PersistLogs];

            // Assert
            _persister.Verify(
                _ => _.Get<ConfigKeyObject>(
                    It.Is<Guid>(g => g == ConfigKeys.System.PersistLogs.Guid)),
                Times.Once);
            Assert.IsTrue(appConfig.TimeOfRetrieval.ContainsKey(ConfigKeys.System.PersistLogs.Name));
            Assert.IsTrue(appConfig.TimeOfRetrieval[ConfigKeys.System.PersistLogs.Name] > startTime);
            Assert.AreEqual(result, "false"); // Should pull from Get(), which returns false
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldRetrieveFromCacheBeforeExpiration()
        {
            // Setup
            var appConfig = Singleton.Get<AppConfig>();
            var startTime = DateTime.Now;
            appConfig.TimeOfRetrieval = new Dictionary<string, DateTime>()
            {
                {
                    ConfigKeys.System.PersistLogs.Name,
                    startTime
                }
            };

            // Execute
            var result = appConfig[ConfigKeys.System.PersistLogs];

            // Assert
            _persister.Verify(
                _ => _.Get<ConfigKeyObject>(
                    It.IsAny<Guid>()),
                Times.Never);
            Assert.IsTrue(appConfig.TimeOfRetrieval.ContainsKey(ConfigKeys.System.PersistLogs.Name));
            Assert.IsTrue(appConfig.TimeOfRetrieval[ConfigKeys.System.PersistLogs.Name] == startTime);
            Assert.AreEqual(result, "true"); // Should pull from cache (which was populated by Scan()), which returns true
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldRetrieveFromDynamoAfterExpiration()
        {
            // Setup
            var appConfig = Singleton.Get<AppConfig>();
            var startTime = DateTime.Now - new TimeSpan(1, 0, 0);
            appConfig.TimeOfRetrieval = new Dictionary<string, DateTime>()
            {
                {
                    ConfigKeys.System.PersistLogs.Name,
                    startTime
                }
            };

            // Execute
            var result = appConfig[ConfigKeys.System.PersistLogs];

            // Assert
            _persister.Verify(
                _ => _.Get<ConfigKeyObject>(
                    It.Is<Guid>(g => g == ConfigKeys.System.PersistLogs.Guid)),
                Times.Once);
            Assert.IsTrue(appConfig.TimeOfRetrieval.ContainsKey(ConfigKeys.System.PersistLogs.Name));
            Assert.IsTrue(appConfig.TimeOfRetrieval[ConfigKeys.System.PersistLogs.Name] != startTime);
            Assert.AreEqual(result, "false"); // Should pull from Get(), which returns false
        }
    }
}
