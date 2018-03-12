using CNGCashier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNGCashier.Controllers
{
    [AuthorizationFilter]
    public class CompanyController : Controller
    {
        CompanyRepo companyRepo = new CompanyRepo();
        // GET: Company
        public ActionResult Index()
        {
            return View();
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