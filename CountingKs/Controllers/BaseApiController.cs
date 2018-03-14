using CountingKs.Data;
using CountingKs.Models;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly ICountingKsRepository _repo;
        private readonly ModelFactory _modelFactory;

        protected BaseApiController(ICountingKsRepository repo, HttpRequestMessage request)
        {
            _repo = repo;
            _modelFactory = new ModelFactory(request);
        }

        protected ICountingKsRepository Repository => _repo;
        protected ModelFactory ModelFactory => _modelFactory;// ?? (_modelFactory = new ModelFactory(this.Request));
    }
}