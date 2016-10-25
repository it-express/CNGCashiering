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
    public class CompanyController : Controller
    {
        CompanyRepository companyRepo = new CompanyRepository();

        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page)
        {
            //List<Company> lstCompany = companyRepo.List().ToList();

            //return View(lstCompany);

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

            IQueryable<Company> lstCompany = companyRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstCompany = lstCompany.Where(s => s.Name.Contains(searchString)
                                       || s.Address.Contains(searchString)
                                       || s.ContactPerson.ToString().Contains(searchString)
                                       || s.ContactNo.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstCompany = lstCompany.OrderByDescending(p => p.Id);
            }
            else
            {
                lstCompany = lstCompany.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstCompany.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View("Edit", new Company());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Company company = companyRepo.GetById(id);

            return View(company);
        }

        [HttpPost]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                companyRepo.Save(company);

                TempData["message"] = "Company has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            companyRepo.Delete(id);

            return RedirectToAction("Index");
        }

        public JsonResult GetById(int id) {
            Company company = companyRepo.GetById(id);

            return Json(company);
        }

        public PartialViewResult MenuList(string controllerName)
        {
            List<Company> lstCompany = companyRepo.List().ToList();

            ViewBag.ControllerName = controllerName; 

            return PartialView(lstCompany);
        }
    }
}