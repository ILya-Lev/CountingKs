﻿using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize(false)]
    [RoutePrefix("api/nutrition/foods")]
    public class FoodsController : BaseApiController
    {
        private const int PageSize = 10;

        public FoodsController(ICountingKsRepository repo, IModelFactory modelFactory) : base(repo, modelFactory)
        {
        }

        [Route("{foodid:int}", Name = "Food")]
        public IHttpActionResult Get(int foodid)
        {
            Debug.WriteLine($"Thread start in get {Thread.CurrentThread.ManagedThreadId}");
            //StartAsync();
            var aFood = Repository.GetFood(foodid);
            Debug.WriteLine($"Thread  end in get {Thread.CurrentThread.ManagedThreadId}");
            var foodModel = ModelFactory.Create(aFood);
            return Versioned(foodModel);
        }

        [Route("", Name = "FoodPage")]
        public HttpResponseMessage Get(bool includeMeasures = true, int page = 0)
        {
            try
            {
                var foodByDescription = GetFoods(includeMeasures).OrderBy(f => f.Description);

                var totalFoodAmount = foodByDescription.Count();
                var totalPages = totalFoodAmount / PageSize;

                var foodsOnThePage = foodByDescription
                    .Skip(page * PageSize)
                    .Take(PageSize)
                    .ToList()
                    .Select(ModelFactory.Create);

                var pagingData = new
                {
                    TotalCount = totalFoodAmount,
                    TotalPages = totalPages,
                    Links = new[]
                    {
                        LinkForPage("prevPage", page - 1, totalPages),
                        LinkForPage("nextPage", page + 1, totalPages)
                    }.Where(lm => !string.IsNullOrWhiteSpace(lm.Href)).ToList(),
                    FoodsOnThePage = foodsOnThePage
                };

                return Request.CreateResponse(HttpStatusCode.OK, pagingData);
            }
            catch (Exception exc)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc);
            }
        }

        private LinkModel LinkForPage(string pageName, int pageNumber, int totalPages)
        {
            var linkForPage = pageNumber < 0 || pageNumber > totalPages
                ? ""
                : new UrlHelper(Request).Link("FoodPage", new { page = pageNumber });

            return ModelFactory.CreateLink(linkForPage, pageName);
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
