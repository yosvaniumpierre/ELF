namespace Avanade.AppKernel.Controllers
{
    using System;
    using System.Web.Mvc;

    using BootStrapper.Web.AddIn;
    using BootStrapper.Web.Attributes;
    using BootStrapper.Web.Framework;

    using Models;

    using NLog;

    /// <summary>
    /// 
    /// </summary>
    public class AuthenticateController : BaseController
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAuthenticatePlugin authenticatePlugin;

        #endregion Fields

        #region Constructors

        public AuthenticateController(IAuthenticatePlugin authenticate)
        {
            authenticatePlugin = authenticate;
        }

        #endregion Constructors

        #region Methods

        [JsonPox]
        [Authenticate]
        [RequireHttps]
        public ViewResult Index(string name, string password)
        {
            //FIXME Put in the logic for processing the hash signature.

            // decode the hashed signature
            string authorizationHeader = ControllerContext.HttpContext.Request.Headers["authorization"];
            bool result = authenticatePlugin.Verify(authorizationHeader);

            //if hash signature is ok then generate a token
            ViewData.Model = new AuthToken
            {
                AccessId = "Hello " + name + ": " + password,
                ExpiryTime = DateTime.UtcNow.AddMinutes(30),
                Identifier = authorizationHeader,
                IsAuthenticated = result
            };

            //else return a failed token.
            return View();
        }

        public ViewResult OnError(string message)
        {
            ViewData["message"] = message;

            return View();
        }

        #endregion Methods
    }
}