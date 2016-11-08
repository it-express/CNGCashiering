using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class DevelopmentController : Controller
    {
        CNGDBContext context = new CNGDBContext();

        // GET: Development
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TruncateDatabase() {
            var affectedRows = context.Database.ExecuteSqlCommand("spTruncateDatabase");

            ViewBag.Annotation = "Database was successfully truncated";

            return RedirectToAction("Index");
        }
    }
}