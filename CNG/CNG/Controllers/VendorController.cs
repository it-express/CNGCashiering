using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;
using PagedList;
using System.Linq.Dynamic;

namespace CNG.Controllers
{
    [AuthorizationFilter]
    public class VendorController : Controller
    {
        VendorRepository vendorRepo = new VendorRepository();
        
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortColumn;
            ViewBag.SortOrder = sortOrder == "asc" ? "desc" : "asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<Vendor> lstVendor = vendorRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstVendor = lstVendor.Where(s => s.Name.Contains(searchString)
                                       || s.Address.Contains(searchString)
                                       || s.ContactPerson.ToString().Contains(searchString)
                                       || s.ContactNo.Contains(searchString)
                                       || s.Terms.ToString().Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstVendor = lstVendor.OrderByDescending(p => p.Id);
            }
            else
            {
                lstVendor = lstVendor.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstVendor.ToPagedList(pageNumber, pageSize));
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