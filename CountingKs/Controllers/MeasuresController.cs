using CountingKs.Data;
using CountingKs.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        public MeasuresController(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        public IEnumerable<MeasureModel> Get(int foodId)
        {
            return Repository
                .GetMeasuresForFood(foodId)
                .ToList()
                .Select(ModelFactory.Create);
        }

        public IHttpActionResult Get(int foodId, int id)
        {
            var measure = Repository.GetMeasure(id);
            if (measure.Food.Id != foodId)
                return NotFound();

            return Ok(ModelFactory.Create(measure));
        }
    }
}
