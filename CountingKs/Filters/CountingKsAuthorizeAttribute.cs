using CountingKs.Data;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebMatrix.WebData;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly bool _isPerUser;
        private readonly ICountingKsRepository _countingKsRepository;

        public CountingKsAuthorizeAttribute(bool isPerUser = true)
        {
            _isPerUser = isPerUser;
            _countingKsRepository = (ICountingKsRepository)
                GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICountingKsRepository));
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
#if !NoAuth
            if (!TokenAuthorization(actionContext))
            {
                HandleUnauthorized(actionContext);
                return;
            }

            if (_isPerUser && !PerUserAuthorization(actionContext))
            {
                HandleUnauthorized(actionContext);
            }
#endif
        }

        private bool TokenAuthorization(HttpActionContext actionContext)
        {
            const string ApiKeyName = "apiKey";
            const string TokenName = "token";

            var queryParts = actionContext.Request.RequestUri.Query.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            var apiKey = GetQueryParameter(queryParts, ApiKeyName);
            var token = GetQueryParameter(queryParts, TokenName);
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(token))
                return false;

            var authToken = _countingKsRepository.GetAuthToken(token);
            return authToken?.ApiUser.AppId == apiKey && authToken.Expiration > DateTime.UtcNow;
        }

        private static string GetQueryParameter(string[] queryParts, string theName)
        {
            var thePart = queryParts.FirstOrDefault(qp => Regex.IsMatch(qp, $@"{theName}", RegexOptions.IgnoreCase));
            var theMatch = Regex.Match(thePart ?? "", $@"{theName}\s*=\s*(?'{theName}'.*)", RegexOptions.IgnoreCase);

            return theMatch.Groups[theName].Value;
        }

        private bool PerUserAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                return true;

            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null) return false;

            if (!authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(authHeader.Parameter))
                return false;

            var rawCredentials = authHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
            var credentialParts = credentials.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            var userName = credentialParts[0];
            var password = credentialParts[1];

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName",
                    autoCreateTables: true);
            }

            if (WebSecurity.Login(userName, password))
            {
                var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                Thread.CurrentPrincipal = principal;
                return true;
            }

            return false;
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            if (_isPerUser)
            {
                actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='CountingKs' location='localhost:8901/account/login'");
            }
        }
    }
}