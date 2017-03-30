using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard(int? companyId)
        {
            if (companyId.HasValue) {
                Sessions.CompanyId = companyId;
            }


            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login() {

            return View();
        }

        public ActionResult Logout() {
            Session.Abandon();
            Session.Clear();

            return RedirectToAction("Login");
        }

        public ActionResult Login(string username, string password) {
            UserRepository userRepo = new UserRepository();

            User user = userRepo.GetByCredentials(username, password);
            if (user != null)
            {
                Session["uid"] = user.Id;
                Session["uname"] = user.Username;
                Session["utype"] = user.UserTypeId;

                return RedirectToAction("Index");
            }
            else {
                ViewBag.ErrorMessage = "Inivalid username and/or password.";

                return View();
            }
        }
    }
}