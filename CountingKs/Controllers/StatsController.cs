using CountingKs.Data;
using CountingKs.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    [RoutePrefix("api/stats")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = Repository.GetAllFoods().Count(),
                NumUsers = Repository.GetApiUsers().Count()
            };
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        [Route("{id:int}")]       // ~ disables route prefix; e.g. ~/api/{id}
        public HttpResponseMessage Get(int id)
        {
            if (id == 1)
                return Request.CreateResponse(HttpStatusCode.OK, new { NumFoods = Repository.GetAllFoods().Count() });
            if (id == 2)
                return Request.CreateResponse(HttpStatusCode.OK, new { NumUsers = Repository.GetApiUsers().Count() });

            return Request.CreateResponse(HttpStatusCode.NotFound, $"Unknown operation for id={id}");
        }

        [Route("{name:alpha}")]     // alpha means string ?!?
        public HttpResponseMessage Get(string name)
        {
            if (name == "foods")
                return Request.CreateResponse(HttpStatusCode.OK, new { NumFoods = Repository.GetAllFoods().Count() });
            if (name == "users")
                return Request.CreateResponse(HttpStatusCode.OK, new { NumUsers = Repository.GetApiUsers().Count() });

            return Request.CreateResponse(HttpStatusCode.NotFound, $"Unknown operation for name={name}");
        }
    }
}
