using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, IModelFactory modelFactory, ICountingKsIdentityService identityService)
            : base(repo, modelFactory)
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

        public HttpResponseMessage Post(DateTime diaryId, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entry = ModelFactory.Parse(model);

                if (entry == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Could not read diary entry in body");

                var diary = Repository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find diary entry for {diaryId}");

                if (diary.Entries.Any(e => e.Measure.Id == entry.Measure.Id))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Duplicate Measure is not allowed!");

                entry.Diary = diary;
                diary.Entries.Add(entry);
                return Repository.SaveAll()
                    ? Request.CreateResponse(HttpStatusCode.Created, ModelFactory.Create(entry))
                    : Request.CreateResponse(HttpStatusCode.BadRequest, $"Could not save into database new entry");
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"{exc.Message} {exc.InnerException?.Message}");
            }
        }

        public HttpResponseMessage Delete(DateTime diaryId, int id)
        {
            try
            {
                var entry = Repository.GetDiaryEntry(_identityService.CurrentUser, diaryId, id);
                if (entry == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"There is no diary entry with id {id}");

                return Repository.DeleteDiaryEntry(id) && Repository.SaveAll()
                     ? Request.CreateResponse(HttpStatusCode.OK, "Diary entry was deleted")
                     : Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot delete diary entry from database");
            }
            catch (Exception exc)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc);
            }
        }
    }
}
