using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Net;
using System.Net.Http;

namespace CountingKs.Controllers
{
    public class DiarySummaryController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repo, IModelFactory modelFactory, ICountingKsIdentityService identityService)
            : base(repo, modelFactory)
        {
            _identityService = identityService;
        }

        public HttpResponseMessage Get(DateTime diaryId)
        {
            try
            {
                var diary = Repository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"There is no diary for '{diaryId}'");

                return Request.CreateResponse(HttpStatusCode.OK, ModelFactory.CreateSummary(diary));
            }
            catch (Exception exc)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc);
            }

        }
    }
}
