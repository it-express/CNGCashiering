using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class CompanyController : Controller
    {
        CompanyRepository companyRepo = new CompanyRepository();

        public ActionResult Index()
        {
            List<Company> lstCompany = companyRepo.List().ToList();

            return View(lstCompany);
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
    }
}