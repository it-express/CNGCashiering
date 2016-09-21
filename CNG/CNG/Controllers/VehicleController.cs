using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class VehicleController : Controller
    {
        VehicleRepository vehicleRepo = new VehicleRepository();

        public ActionResult Index()
        {
            List<Vehicle> lstVehicle = vehicleRepo.List().ToList();

            return View(lstVehicle);
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