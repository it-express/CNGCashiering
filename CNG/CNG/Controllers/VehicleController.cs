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
    public class VehicleController : Controller
    {
        VehicleRepository vehicleRepo = new VehicleRepository();
        VehicleAssignmentRepository vehicleAssignmentRepo = new VehicleAssignmentRepository();

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

            //IQueryable<Vehicle> lstVehicle = vehicleRepo.List();
            IQueryable<Vehicle> lstVehicle = vehicleAssignmentRepo.List()
                                            .Where(p => p.CompanyId == Sessions.CompanyId.Value)
                                            .Select(q => q.Vehicle);

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

        public ActionResult Assignment()
        {
            VehicleAssignmentRepository vehicleAssignmentRepo = new VehicleAssignmentRepository();

            List<VehicleAssignmentVM> lstItemAssignmentVM = (from p in vehicleRepo.List().ToList()
                                                          join q in vehicleAssignmentRepo.List().ToList()
                                                          on p.Id equals q.VehicleId into pq
                                                          from r in pq.DefaultIfEmpty()
                                                          select new VehicleAssignmentVM
                                                          {
                                                              VehicleId = p.Id,
                                                              Vehicle = p,
                                                              IsAssigned = r == null ? false : true
                                                          }).ToList();

            return View(lstItemAssignmentVM);
        }

        [HttpPost]
        public ActionResult AssignmentSave(int[] VehicleId)
        {
            List<VehicleAssignment> lstVechicleAssignment = new List<VehicleAssignment>();
            foreach (int vehicleId in VehicleId)
            {
                VehicleAssignment vehicleAssign = new VehicleAssignment
                {
                    VehicleId = vehicleId,
                    CompanyId = Sessions.CompanyId.Value
                };

                lstVechicleAssignment.Add(vehicleAssign);
            }

            vehicleAssignmentRepo.Save(lstVechicleAssignment);

            return RedirectToAction("Index");
        }
    }
}