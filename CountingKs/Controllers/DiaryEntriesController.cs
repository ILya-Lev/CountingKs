using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, HttpRequestMessage request, ICountingKsIdentityService identityService)
            : base(repo, request)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {
            return Repository
                .GetDiaryEntries(_identityService.CurrentUser, diaryId)
                .ToList()
                .Select(ModelFactory.Create);
        }
    }
}
