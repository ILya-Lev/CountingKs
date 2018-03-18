using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace CountingKs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var jsonMediaTypeFormatter = config.Formatters.JsonFormatter;
            jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

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

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            ////config.Filters.Add(new EnableCorsAttribute);

#if !DEBUG
            config.Filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}