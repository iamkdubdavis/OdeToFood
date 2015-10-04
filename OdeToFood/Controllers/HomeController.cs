using OdeToFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;

namespace OdeToFood.Controllers
{
    //[Authorize(Users = "kdavis@test.com")]
    public class HomeController : Controller
    {
        OdeToFoodDb _db = new OdeToFoodDb();

        public ActionResult Autocomplete(string term)
        {
            var model = _db.Restaurants
                .Where(r => r.Name.StartsWith(term))
                .Take(10)
                .ToList()
                .Select(r => new
                {
                    label = r.Name
                });

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        [OutputCache(CacheProfile = "Long", VaryByHeader = "X-Requested-With;Accept-Language", Location = OutputCacheLocation.Server)]
        public ActionResult Index(string searchTerm = null, int page = 1)
        {
            var greeting = OdeToFood.Views.Home.Resources.Greeting;

            var model =
                _db.Restaurants
                    .OrderByDescending(r => r.Reviews.Average(review => review.Rating))
                    .Where(r => searchTerm == null || r.Name.StartsWith(searchTerm))
                    //Fixed "System.NotSupportedException" error via StackOverflow
                    .ToList()
                    .Select(r => new RestaurantListViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        City = r.City,
                        Country = r.Country,
                        CountOfReviews = r.Reviews.Count()
                    }).ToPagedList(page, 10);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Restaurants", model);
            }

            return View(model);
        }

        public ActionResult About()
        {
            var model = new AboutModel();

            model.Name = "Kim";
            model.Location = "Georgia, USA";

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}