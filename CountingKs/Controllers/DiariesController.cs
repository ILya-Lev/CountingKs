using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiariesController : BaseApiController
    {
        public DiariesController(ICountingKsRepository repo, HttpRequestMessage request) : base(repo, request)
        {
        }

        public IEnumerable<Diary> Get()
        {
            return Repository.GetDiaries(User.Identity.Name);
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
            return Ok(diary);
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

            var entryDescriptions = diary.Entries
                .Select(e => $"{e.FoodItem.Description} {e.Measure.Description} {e.Quantity}");

            var aSummary = string.Join("\r\n", entryDescriptions);
            return Ok(aSummary);
        }

        private Diary GetDiaryForDate(DateTime date)
        {
            return Repository.GetDiary(User.Identity.Name, date);
        }
    }
}
