using CountingKs.Data;
using CountingKs.Models;
using System.Linq;
using System.Web.Http;

namespace CountingKs.Controllers
{
    [RoutePrefix("api/stats")]
    //[EnableCors(origins: "*", headers: "*", methods: "GET")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        [Route("")]
        //[DisableCors]
        public IHttpActionResult Get()
        {
            var results = new
            {
                NumFoods = Repository.GetAllFoods().Count(),
                NumUsers = Repository.GetApiUsers().Count()
            };
            return Ok(results);
        }

        [Route("{id:int}")]       // ~ disables route prefix; e.g. ~/api/{id}
        public IHttpActionResult Get(int id)
        {
            if (id == 1)
                return Ok(new { NumFoods = Repository.GetAllFoods().Count() });
            if (id == 2)
                return Ok(new { NumUsers = Repository.GetApiUsers().Count() });

            return NotFound();//HttpStatusCode.NotFound, $"Unknown operation for id={id}";
        }

        [Route("{name:alpha}")]     // alpha means string ?!?
        public IHttpActionResult Get(string name)
        {
            if (name == "foods")
                return Ok(new { NumFoods = Repository.GetAllFoods().Count() });
            if (name == "users")
                return Ok(new { NumUsers = Repository.GetApiUsers().Count() });

            return NotFound();//HttpStatusCode.NotFound, $"Unknown operation for name={name}";
        }
    }
}
