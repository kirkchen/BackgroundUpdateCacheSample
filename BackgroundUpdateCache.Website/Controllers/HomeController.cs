using BackgroundUpdateCache.Website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackgroundUpdateCache.Website.Controllers
{
    public class HomeController : Controller
    {
        public ILongRunningService LongRunningService { get; set; }

        public HomeController(ILongRunningService longRunningService)
        {
            this.LongRunningService = longRunningService;
        }

        public ActionResult Index()
        {
            var data = this.LongRunningService.GetData();

            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }        
    }
}