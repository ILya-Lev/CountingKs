using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiariesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repo, IModelFactory modelFactory, ICountingKsIdentityService identityService)
            : base(repo, modelFactory)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryModel> Get()
        {
            //return Repository.GetDiaries(User.Identity.Name);
            return Repository
                .GetDiaries(_identityService.CurrentUser)
                .OrderByDescending(d => d.CurrentDate)
                .Take(10)
                .ToList()
                .Select(ModelFactory.Create);
        }

        public void Post()
        {
            var diary = new Diary
            {
                CurrentDate = DateTime.Now,
                UserName = User.Identity.Name,
            };
        }

        public IHttpActionResult Get(DateTime diaryId)
        {
            var diary = GetDiaryForDate(diaryId);
            if (diary == null)
                return NotFound();
            var diaryModel = ModelFactory.Create(diary);
            return Ok(diaryModel);
        }

        public IHttpActionResult Delete(DateTime date)
        {
            var diary = GetDiaryForDate(date);
            if (diary == null)
            {
                return NotFound();
            }
            Repository.DeleteDiary(diary.Id);
            return Ok();
        }

        public IHttpActionResult Get(DateTime date, bool summary)
        {
            var diary = GetDiaryForDate(date);
            if (diary == null)
                return NotFound();

            var diaryModel = ModelFactory.Create(diary);
            var entryDescriptions = diaryModel.Entries
                .Select(e => $"{e.FoodDescription} {e.Measure.Description} {e.Quantity}");

            var aSummary = string.Join("\r\n", entryDescriptions);
            return Ok(aSummary);
        }

        private Diary GetDiaryForDate(DateTime date)
        {
            //return Repository.GetDiary(User.Identity.Name, date);
            return Repository.GetDiary(_identityService.CurrentUser, date);
        }
    }
}
