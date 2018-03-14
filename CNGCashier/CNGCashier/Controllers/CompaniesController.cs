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
using System.Linq.Dynamic;

namespace CNGCashier.Controllers
{
    [AuthorizationFilter]
    public class CompaniesController : Controller
    {
        private CNGCashierDBContext db = new CNGCashierDBContext();
        CompanyRepo companyRepo = new CompanyRepo();

        // GET: Companies
        public ActionResult Index(string sortColumn, string sortOrder,string nextpage, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortColumn;
            ViewBag.SortOrder = sortOrder;

            if(nextpage == null)
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.AddressSortParm = String.IsNullOrEmpty(sortOrder) ? "address_desc" : "";
            //ViewBag.ContactPSortParm = String.IsNullOrEmpty(sortOrder) ? "contactp_desc" : "";
            //ViewBag.ContactNSortParm = String.IsNullOrEmpty(sortOrder) ? "contactn_desc" : "";
            //ViewBag.ActiveSortParm = String.IsNullOrEmpty(sortOrder) ? "active_desc" : "";
            //ViewBag.PrefixSortParm = String.IsNullOrEmpty(sortOrder) ? "prefix_desc" : "";

            //Paging
            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}
            ViewBag.CurrentFilter = searchString;

            IQueryable<Company> companies = companyRepo.List();

            //Search
            if (!String.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(s => s.Name.Contains(searchString)
                                       || s.Address.Contains(searchString)
                                       || s.ContactPerson.ToString().Contains(searchString)
                                       || s.ContactNo.Contains(searchString));
            }

            ////Sorting the table for companies
            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        companies = companies.OrderByDescending(c => c.Name);
            //        break;
            //    case "address_desc":
            //        companies = companies.OrderByDescending(c => c.Address);
            //        break;
            //    case "contactp_desc":
            //        companies = companies.OrderByDescending(c => c.ContactPerson);
            //        break;
            //    case "contactn_desc":
            //        companies = companies.OrderByDescending(c => c.ContactNo);
            //        break;
            //    case "active_desc":
            //        companies = companies.OrderByDescending(c => c.Active);
            //        break;
            //    case "prefix_desc":
            //        companies = companies.OrderByDescending(c => c.Prefix);
            //        break;
            //    default:
            //        companies = companies.OrderBy(c => c.Name);
            //        break;
            //}
            if (String.IsNullOrEmpty(sortColumn))
            {
                companies = companies.OrderByDescending(c => c.Id);
            }
            else
            {
                companies = companies.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(companies.ToPagedList(pageNumber, pageSize));
            //return View(db.Companies.ToList());
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,ContactPerson,ContactNo,Active,Prefix")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,ContactPerson,ContactNo,Active,Prefix")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
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

        public JsonResult GetById(int id)
        {
            Company company = companyRepo.GetById(id);

            return Json(company);
        }

        public PartialViewResult MenuList()
        {
            List<Company> lstCompany = companyRepo.List().ToList();

            return PartialView(lstCompany);
        }
    }
}
