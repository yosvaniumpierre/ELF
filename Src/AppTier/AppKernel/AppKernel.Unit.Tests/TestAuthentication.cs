namespace Avanade.AppKernel.Unit.Tests
{
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Avanade.BootStrapper.Web.AddIn;

    using Controllers;

    using Models;

    using Moq;

    using MvcContrib.TestHelper;

    using NUnit.Framework;

    [TestFixture]
    public class TestAuthentication
    {
        #region Methods

        [Test]
        public void TestAuthorizationControllerFailure()
        {
            Assert.Fail("Need to implement the test");
        }

        [Test]
        public void TestAuthorizationControllerSuccess()
        {
            const string headerValue = "AuthUnitTest";

            // Let's mock up the authentication plugin
            var mockAuthenticatePlugin = new Mock<IAuthenticatePlugin>();
            mockAuthenticatePlugin.Setup(x => x.Verify(headerValue)).Returns(true);

            // Create the mock HttpRequest object and the required header
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(new WebHeaderCollection { { "authorization", headerValue } });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            // Now create the controller with the mock authentication plugin
            var controller = new AuthenticateController(mockAuthenticatePlugin.Object);

            // Now we inject the mocked up controller context in order to ensure the test to function
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var viewResult = controller.Index("Michael", "Mark");
            var model = (AuthToken) viewResult.Model;

            Assert.NotNull(model);
            Assert.IsInstanceOf<AuthToken>(model);
            Assert.IsTrue(model.IsAuthenticated);
            Assert.AreEqual("Hello Michael: Mark", model.AccessId);
            Assert.AreEqual(headerValue, model.Identifier);
        }

        [Test]
        public void TestControllerAction()
        {
            var mockAuthenticatePlugin = new Mock<IAuthenticatePlugin>();

            var builder = new TestControllerBuilder();
            var controller = new AuthenticateController(mockAuthenticatePlugin.Object);
            builder.InitializeController(controller);

            var viewResult = controller.OnError("ErrorMessage");
            var message = viewResult.ViewData["message"];

            Assert.NotNull(message);
            Assert.AreEqual("ErrorMessage", message);
        }

        #endregion Methods
    }
}