using DnDTracker.Web;
using DnDTracker.Web.Services.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DnDTracker.Tests.Services.Session
{
    [TestFixture]
    public class SessionServiceTests : UnitTestFixture
    {
        private DefaultHttpContext _httpContext;
        private Mock<ISession> _session;

        [SetUp]
        public void Setup()
        {
            _httpContext = new DefaultHttpContext
            {
                Session = (_session = new Mock<ISession>()).Object
            };

            MockSingleton
                .Override<SessionService>(new SessionService());
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldReturnDefaultValueWhenKeyNotFound()
        {
            // Setup
            var bytes = Encoding.UTF8.GetBytes("{}");
            _session.Setup(_ => _.TryGetValue("AppState", out bytes)).Returns(true);
            var sessionService = Singleton.Get<SessionService>();

            // Execute
            var val = sessionService.Get("key", "default", new MockHttpContextAccessor(_httpContext));

            // Assert
            Assert.AreEqual(val, "default");
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldReturnValueWhenKeyFound()
        {
            // Setup
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                key = "value"
            }));
            _session.Setup(_ => _.TryGetValue("AppState", out bytes)).Returns(true);
            var sessionService = Singleton.Get<SessionService>();

            // Execute
            var val = sessionService.Get("key", "default", new MockHttpContextAccessor(_httpContext));

            // Assert
            Assert.AreEqual(val, "value");
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldSetValueWithOldKey()
        {
            // Setup
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                key = "value"
            }));
            _session.Setup(_ => _.TryGetValue("AppState", out bytes)).Returns(true);
            var sessionService = Singleton.Get<SessionService>();

            // Execute
            sessionService.Set("key", "newvalue", new MockHttpContextAccessor(_httpContext));

            // Assert
            _session.Setup(_ => _.Set("AppState", It.Is<byte[]>(b => (JObject.Parse(Encoding.UTF8.GetString(b))["key"].ToString() ?? "") == "newvalue")));
        }

        [Test]
        [Author("Derek Williamson")]
        public void ShouldSetValueWithNewKey()
        {
            // Setup
            var bytes = Encoding.UTF8.GetBytes("{}");
            _session.Setup(_ => _.TryGetValue("AppState", out bytes)).Returns(true);
            var sessionService = Singleton.Get<SessionService>();

            // Execute
            sessionService.Set("key", "value", new MockHttpContextAccessor(_httpContext));

            // Assert
            _session.Setup(_ => _.Set("AppState", It.Is<byte[]>(b => (JObject.Parse(Encoding.UTF8.GetString(b))["key"].ToString() ?? "") == "value")));
        }

        public class MockHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext { get; set;  }

            public MockHttpContextAccessor(HttpContext httpContext)
            {
                HttpContext = httpContext;
            }
        }
    }
}
