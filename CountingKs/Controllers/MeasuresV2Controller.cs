using CountingKs.Data;
using CountingKs.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace CountingKs.Controllers
{
    public class MeasuresV2Controller : BaseApiController
    {
        public MeasuresV2Controller(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        public IEnumerable<MeasureV2Model> Get(int foodId)
        {
            return Repository
                .GetMeasuresForFood(foodId)
                .ToList()
                .Select(ModelFactory.Create2);
        }

        public HttpResponseMessage Get(int foodId, int id)
        {
            var measure = Repository.GetMeasure(id);

            return measure.Food.Id != foodId
                ? Request.CreateResponse(HttpStatusCode.NotFound, $"No Measure with {id} for food id {foodId} was found")
                : Request.CreateResponse(HttpStatusCode.OK, ModelFactory.Create2(measure));
        }
    }
}
