using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class RequisitionPurchaseController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        RequisitionPurchaseRepository requisitionPurchaseRepo = new RequisitionPurchaseRepository(); 

        // GET: RequisitionPurchase
        public ActionResult Index()
        {
            List<RequisitionPurchase> lst = requisitionPurchaseRepo.List().ToList();

            return View(lst);
        }

        public ActionResult Create()
        {
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View();
        }
    }
}