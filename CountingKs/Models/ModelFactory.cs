using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace CountingKs.Models
{
    public class ModelFactory : IModelFactory
    {
        private readonly ICountingKsRepository _repo;
        private readonly UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repo)
        {
            _repo = repo;
            _urlHelper = new UrlHelper(request);
        }

        public FoodModel Create(Food food)
        {
            if (food == null)
                return null;

            return new FoodModel
            {
                Url = _urlHelper.Link("Food", new { foodid = food.Id }),
                Description = food.Description,
                Measures = food.Measures.Select(Create)
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories),
            };
        }

        public DiaryModel Create(Diary diary)
        {
            return new DiaryModel
            {
                Url = _urlHelper.Link("Diary", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),
                UserName = diary.UserName,
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(Create)
            };
        }

        public DiaryEntryModel Create(DiaryEntry entry)
        {
            return new DiaryEntryModel
            {
                Url = _urlHelper.Link("Diary Entry", new { id = entry.Id, diaryid = entry.Diary.CurrentDate.ToString("yyyy-MM-dd") }),
                Quantity = entry.Quantity,
                FoodDescription = entry.FoodItem?.Description,
                Measure = Create(entry.Measure)
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var measureUrl = new Uri(model.Measure.Url);
                var measureId = int.Parse(measureUrl.Segments.Last());

                var entry = new DiaryEntry
                {
                    Quantity = model.Quantity,
                    Measure = _repo.GetMeasure(measureId),
                };

                entry.FoodItem = entry.Measure.Food;
                //todo: what about diary?
                return entry;
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"{exc.Message} {exc.InnerException?.Message}");
                return null;
            }
        }

        public DiarySummaryModel CreateSummary(Diary diary)
        {
            return new DiarySummaryModel
            {
                DiaryDate = diary.CurrentDate,
                TotalCalories = Math.Ceiling(diary.Entries.Sum(e => e.Measure.Calories * e.Quantity))
            };
        }
    }
}