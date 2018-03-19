using System.Net.Http;
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
            if (controllers.TryGetValue(controllerName, out descriptor))
            {
                var version = "2";
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
    }
}