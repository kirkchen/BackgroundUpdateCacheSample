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
            var data = this.LongRunningService.GetData(0, 100);

            return View(data);
        }
    }
}