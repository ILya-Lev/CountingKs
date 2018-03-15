using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CountingKs.Controllers
{
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        public IEnumerable<FoodModel> Get(bool includeMeasures = true)
        {
            var result =
                GetFoods(includeMeasures)
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                .Select(ModelFactory.Create);

            return result;
        }

        public FoodModel Get(int foodid)
        {
            Debug.WriteLine($"Thread start in get {Thread.CurrentThread.ManagedThreadId}");
            //StartAsync();
            var aFood = Repository.GetFood(foodid);
            Debug.WriteLine($"Thread  end in get {Thread.CurrentThread.ManagedThreadId}");
            return ModelFactory.Create(aFood);
        }

        private IQueryable<Food> GetFoods(bool includeMeasures)
        {
            return includeMeasures
                ? Repository.GetAllFoodsWithMeasures()
                : Repository.GetAllFoods();
        }
        #region async check staff
        private async void StartAsync()
        {
            Debug.WriteLine($"Thread top start {Thread.CurrentThread.ManagedThreadId}");
            Task<int> getLengthTask = AccessTheWebAsync();

            Debug.WriteLine($"Thread before await in start {Thread.CurrentThread.ManagedThreadId}");
            int contentLength = await getLengthTask;

            Debug.WriteLine($"Thread  after await in start {Thread.CurrentThread.ManagedThreadId}");
            var text = contentLength;
        }

        async Task<int> AccessTheWebAsync()
        {
            Debug.WriteLine($"Thread top access {Thread.CurrentThread.ManagedThreadId}");
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");

            Debug.WriteLine($"Thread before await in access {Thread.CurrentThread.ManagedThreadId}");
            string urlContents = await getStringTask;

            Debug.WriteLine($"Thread  after await in access {Thread.CurrentThread.ManagedThreadId}");
            return urlContents.Length;
        }
        #endregion async check staff
    }
}
