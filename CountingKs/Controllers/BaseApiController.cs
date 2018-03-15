using CountingKs.Data;
using CountingKs.Models;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly ICountingKsRepository _repo;
        private readonly IModelFactory _modelFactory;

        protected BaseApiController(ICountingKsRepository repo, IModelFactory modelFactory)
        {
            _repo = repo;
            _modelFactory = modelFactory;
        }

        protected ICountingKsRepository Repository => _repo;
        protected IModelFactory ModelFactory => _modelFactory;// ?? (_modelFactory = new ModelFactory(this.Request));
    }
}