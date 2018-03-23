using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CountingKs.Services
{
    public class CountingKsControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public CountingKsControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();   //omits controllers with the same name

            var routData = request.GetRouteData();
            // must coincide with WebApiConfig http rout property in anonymous object
            var controllerName = (string)routData.Values["controller"];

            HttpControllerDescriptor descriptor;
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return base.SelectController(request);
            }

            if (controllers.TryGetValue(controllerName, out descriptor))
            {
                //var version = GetVersionFromQueryString(request);
                //var version = GetVersionFromHeader(request);
                //var version = GetVersionFromAcceptHeader(request);
                var version = GetVersionFromMediaType(request);

                var newControllerName = $"{controllerName}V{version}";

                HttpControllerDescriptor versionedDescriptor;
                if (controllers.TryGetValue(newControllerName, out versionedDescriptor))
                {
                    return versionedDescriptor;
                }

                return descriptor;
            }

            return null;    //so asp.net pipe can decide what to do; basically return a 404
        }

        private string GetVersionFromMediaType(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;
            var expression = new Regex(@"application/vnd\.countingks\.\w+\.v(?'v'\d+)\+json", RegexOptions.IgnoreCase);
            return accept
                .Select(mime => expression.Match(mime.MediaType))
                .FirstOrDefault(match => match.Groups["v"].Success)
                ?.Groups["v"].Value;
        }

        private string GetVersionFromAcceptHeader(HttpRequestMessage request)
        {
            return request.Headers.Accept
                .Select(mime => mime.Parameters
                                    .FirstOrDefault(param => param.Name.Equals("version", StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault(param => param != null)
                ?.Value;
        }

        private string GetVersionFromHeader(HttpRequestMessage request)
        {
            IEnumerable<string> version;
            if (request.Headers.TryGetValues("version", out version))
            {
                return version.FirstOrDefault();
            }

            return null;
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = request.RequestUri.Query;
            var versionPart = query
                .Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => Regex.Match(part, @"v\s*=\s*(?'v'\d+)", RegexOptions.IgnoreCase))
                .FirstOrDefault(match => match.Groups["v"].Success);

            return versionPart?.Groups["v"].Value;
        }
    }
}