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
    public class VehicleController : Controller
    {
        VehicleRepository vehicleRepo = new VehicleRepository();

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

            IQueryable<Vehicle> lstVehicle = vehicleRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstVehicle = lstVehicle.Where(s => s.Make.Contains(searchString)
                                       || s.Year.ToString().Contains(searchString)
                                       || s.Model.ToString().Contains(searchString)
                                       || s.CnNo.Contains(searchString)
                                       || s.LicenseNo.ToString().Contains(searchString)
                                       || s.EngineNo.ToString().Contains(searchString)
                                       || s.ChasisNo.ToString().Contains(searchString)
                                       || s.Color.ToString().Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstVehicle = lstVehicle.OrderByDescending(p => p.Id);
            }
            else
            {
                lstVehicle = lstVehicle.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstVehicle.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View("Edit", new Vehicle());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Vehicle vehicle = vehicleRepo.GetById(id);

            return View(vehicle);
        }

        [HttpPost]
        public ActionResult Edit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                vehicleRepo.Save(vehicle);

                TempData["message"] = "Vehicle has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(vehicle);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            vehicleRepo.Delete(id);

            return RedirectToAction("Index");
        }
    }
}