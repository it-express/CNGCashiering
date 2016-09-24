using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class RequisitionController : Controller
    {
        private CNGDBContext context = new CNGDBContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}