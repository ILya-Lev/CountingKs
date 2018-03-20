using CacheCow.Server;
using CacheCow.Server.EntityTagStore.SqlServer;
using CountingKs.Services;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace CountingKs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var jsonMediaTypeFormatter = config.Formatters.JsonFormatter;
            jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            CreateMediaTypes(jsonMediaTypeFormatter);
            //config.Formatters.Insert(0, new JsonpMediaTypeFormatter(jsonMediaTypeFormatter));

            //config.Formatters.Clear();
            //config.Formatters.Add(jsonMediaTypeFormatter);


            config.Routes.MapHttpRoute(
                name: "Food",
                routeTemplate: "api/nutrition/foods/{foodid}",
                defaults: new { controller = "foods", foodid = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Measures",
                routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
                defaults: new { controller = "measures", id = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    name: "MeasuresV2",
            //    routeTemplate: "api/v2/nutrition/foods/{foodid}/measures/{id}",
            //    defaults: new { controller = "measuresv2", id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
                name: "Diary",
                routeTemplate: "api/user/diaries/{diaryid}",
                defaults: new { controller = "diaries", diaryid = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Diary Entry",
                routeTemplate: "api/user/diaries/{diaryid}/entries/{id}",
                defaults: new { controller = "diaryEntries", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Diary Summary",
                routeTemplate: "api/user/diaries/{diaryid}/summary",
                defaults: new { controller = "diarySummary" }
            );
            config.Routes.MapHttpRoute(
                name: "Auth Token",
                routeTemplate: "api/token",
                defaults: new { controller = "token" }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            ////config.Filters.Add(new EnableCorsAttribute);

            config.Services.Replace(typeof(IHttpControllerSelector), new CountingKsControllerSelector(config));

            ConfigureCachingETagSupport(config);

#if !DEBUG
            config.Filters.Add(new RequireHttpsAttribute());
#endif
        }

        private static void ConfigureCachingETagSupport(HttpConfiguration config)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            var sqlTagStore = new SqlServerEntityTagStore(connectionString);
            var cachingHandler = new CachingHandler(config, sqlTagStore);
            config.MessageHandlers.Add(cachingHandler);
        }

        private static void CreateMediaTypes(JsonMediaTypeFormatter jsonMediaTypeFormatter)
        {
            var mediaTypes = new[]
            {
                "application/vnd.countingks.food.v1+json",
                "application/vnd.countingks.measure.v1+json",
                "application/vnd.countingks.measure.v2+json",
                "application/vnd.countingks.diary.v1+json",
                "application/vnd.countingks.diaryEntry.v1+json",
            };

            jsonMediaTypeFormatter.SupportedMediaTypes.AddRange(mediaTypes.Select(mt => new MediaTypeHeaderValue(mt)));
        }
    }
}