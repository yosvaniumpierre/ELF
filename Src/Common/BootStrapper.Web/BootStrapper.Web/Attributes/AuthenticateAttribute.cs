namespace Avanade.BootStrapper.Web.Attributes
{
    using System;
    using System.Web.Mvc;

    using Avanade.BootStrapper.Web.AddIn;

    using NLog;

    public class AuthenticateAttribute : AuthorizeAttribute
    {
        #region Fields

        private const string RestQuery = "/OnError/?message=";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        public IAuthenticatePlugin AuthenticatePlugin
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string authorizationHeader = filterContext.HttpContext.Request.Headers["authorization"];
            if(Logger.IsDebugEnabled)
            {
                Logger.Debug("Authorization header: {0}", authorizationHeader);
            }

            if (filterContext.HttpContext.Request.Url == null)
            {
                Logger.Fatal("Why is the URL null?");
            }
            else
            {
                string url = filterContext.HttpContext.Request.Url.AbsolutePath;

                if (String.IsNullOrEmpty(authorizationHeader))
                {
                    filterContext.HttpContext.Response.Headers["message"] = "Unable to perform authentication because information was unavailable.";
                    filterContext.Result = new RedirectResult(url + RestQuery + "Unable to perform authentication.");
                }
                else
                {
                    if (AuthenticatePlugin == null)
                    {
                        Logger.Error("Why is the AuthenticatePlugin null???");
                    }
                    else if (!AuthenticatePlugin.Verify(authorizationHeader))
                    {
                        filterContext.HttpContext.Response.Headers["message"] = "Authentication was unsuccessful. Do check the credentials.";
                        filterContext.Result = new RedirectResult(url + RestQuery +
                            "Authentication was not successful. Check the credentials used.");
                    }
                }
            }
        }

        #endregion Methods
    }
}