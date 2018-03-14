using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNGCashier.Models;
using PagedList;

namespace CNGCashier.Controllers
{
    public class VehiclesController : Controller
    {
        private CNGCashierDBContext db = new CNGCashierDBContext();

        // GET: Vehicles
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.MakeSortParm = String.IsNullOrEmpty(sortOrder) ? "make_desc" : "";
            ViewBag.YearSortParm = String.IsNullOrEmpty(sortOrder) ? "year_desc" : "";
            ViewBag.ModelSortParm = String.IsNullOrEmpty(sortOrder) ? "model_desc" : "";
            ViewBag.CnNoSortParm = String.IsNullOrEmpty(sortOrder) ? "cnno_desc" : "";
            ViewBag.LicenseNoSortParm = String.IsNullOrEmpty(sortOrder) ? "licenseNo_desc" : "";
            ViewBag.EngineNoSortParm = String.IsNullOrEmpty(sortOrder) ? "engineno_desc" : "";
            ViewBag.ChasisNoSortParm = String.IsNullOrEmpty(sortOrder) ? "chasisno_desc" : "";
            ViewBag.ColorSortParm = String.IsNullOrEmpty(sortOrder) ? "color_desc" : "";
            ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) ? "active_desc" : "";
            var vehicles = from v in db.Vehicles select v;

            //Paging
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            //Search
            if (!String.IsNullOrEmpty(searchString))
            {
                vehicles = vehicles.Where(c => c.Make.Contains(searchString));
            }

            //Sorting the table for companies
            switch (sortOrder)
            {
                case "make_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Make);
                    break;
                case "year_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Year);
                    break;
                case "model_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Model);
                    break;
                case "cnno_desc":
                    vehicles = vehicles.OrderByDescending(v => v.CnNo);
                    break;
                case "licenseNo_desc":
                    vehicles = vehicles.OrderByDescending(v => v.LicenseNo);
                    break;
                case "engineno_desc":
                    vehicles = vehicles.OrderByDescending(v => v.EngineNo);
                    break;
                case "chasisno_desc":
                    vehicles = vehicles.OrderByDescending(v => v.ChasisNo);
                    break;
                case "color_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Color);
                    break;
                case "active_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Active);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Make);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(vehicles.ToPagedList(pageNumber, pageSize));
            //return View(db.Vehicles.ToList());
        }


        // GET: Vehicles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Make,Year,Model,CnNo,LicenseNo,EngineNo,ChasisNo,Color,Active")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Make,Year,Model,CnNo,LicenseNo,EngineNo,ChasisNo,Color,Active")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            db.Vehicles.Remove(vehicle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
