using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNGCashier.Controllers
{
    public class DriverController : Controller
    {
        // GET: Drivers
        public ActionResult Index()
        {
            return View();
        }
    }
}