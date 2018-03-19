using CountingKs.Data.Entities;

namespace CountingKs.Models
{
    public interface IModelFactory
    {
        FoodModel Create(Food food);
        MeasureModel Create(Measure measure);
        DiaryEntryModel Create(DiaryEntry entry);
        DiaryModel Create(Diary diary);
        DiaryEntry Parse(DiaryEntryModel entry);
        DiarySummaryModel CreateSummary(Diary diary);
        AuthTokenModel Create(AuthToken authToken);
    }
}