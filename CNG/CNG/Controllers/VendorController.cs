using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class VendorController : Controller
    {
        VendorRepository vendorRepo = new VendorRepository();

        public ActionResult Index()
        {
            List<Vendor> lstVendor = vendorRepo.List().ToList();

            return View(lstVendor);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View("Edit", new Vendor());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Vendor vendor = vendorRepo.GetById(id);

            return View(vendor);
        }

        [HttpPost]
        public ActionResult Edit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                vendorRepo.Save(vendor);

                TempData["message"] = "Vendor has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(vendor);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            vendorRepo.Delete(id);

            return RedirectToAction("Index");
        }

        public JsonResult GetById(int id) {
            Vendor vendor = vendorRepo.GetById(id);

            return Json(vendor);
        }
    }
}