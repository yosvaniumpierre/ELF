namespace Avanade.AppKernel.Integration.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    using Avanade.AppKernel.Models;

    using NUnit.Framework;

    using RestSharp;

    [TestFixture]
    public class TestAuthenticationIntegration
    {
        #region Fields

        private const int Port = 443;

        #endregion Fields

        #region Methods

        public bool AcceptAllCertifications(object sender, 
            X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        [SetUp]
        public void Setup()
        {
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
        }

        [Test]
        public void TestHttpRequestJsonRestSharp()
        {
            var client = new RestClient
                {
                    BaseUrl = string.Format("https://localhost:{0}", Port)
                };

            var request = new RestRequest { Resource = "authenticate" };
            request.AddParameter("name", "Mike");
            request.AddParameter("password", "P@ssw0rd");

            request.AddHeader("authorization", "whatever");
            request.AddHeader("Content-Type", "application/json");

            RestResponse<AuthToken> response = client.Execute<AuthToken>(request);
            var token = response.Data;

            Assert.NotNull(token);
            Assert.AreEqual("Hello Mike: P@ssw0rd", token.AccessId);
        }

        [Test]
        public void TestHttpRequestPlain()
        {
            var uri = new Uri(string.Format("https://localhost:{0}/authenticate", Port));
            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                var request = WebRequest.Create(uri);
                request.Method = WebRequestMethods.Http.Get;
                using (var response = request.GetResponse())
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    string tmp = reader.ReadToEnd();

                    Assert.NotNull(tmp);
                    response.Close();
                    Assert.IsTrue(tmp == "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Index</title>\r\n</head>\r\n\r\n<body>\r\n    \r\n<h2>Authentication</h2>\r\n\r\n</body>\r\n</html>\r\n");
                }
            }
        }

        [Test]
        public void TestHttpRequestPoxRestSharp()
        {
            var client = new RestClient
            {
                BaseUrl = string.Format("https://localhost:{0}", Port)
            };

            var request = new RestRequest { Resource = "authenticate" };
            request.AddParameter("name", "Mike");
            request.AddParameter("password", "P@ssw0rd");

            request.AddHeader("authorization", "this is it");
            request.AddHeader("Content-Type", "text/xml");
            RestResponse<AuthToken> response = client.Execute<AuthToken>(request);
            var token = response.Data;

            Assert.NotNull(token);
            Assert.AreEqual("Hello Mike: P@ssw0rd", token.AccessId);
        }

        #endregion Methods
    }
}