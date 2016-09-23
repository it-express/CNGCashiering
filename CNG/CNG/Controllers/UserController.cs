using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class UserController : Controller
    {
        UserRepository userRepo = new UserRepository();
        UserTypeRepository userTypeRepo = new UserTypeRepository();

        public ActionResult Index()
        {
            List<User> lstUser = userRepo.List().ToList();

            return View(lstUser);
        }

        [HttpGet]
        public ViewResult Create()
        {
            ViewBag.UserType = SelectListHelper.UserTypes();
            ViewBag.GeneralManager = SelectListHelper.GeneralManagers();

            return View("Edit", new User());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.UserType = SelectListHelper.UserTypes();
            ViewBag.GeneralManager = SelectListHelper.GeneralManagers();

            User user = userRepo.GetById(id);

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                userRepo.Save(user);

                TempData["message"] = "User has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            userRepo.Delete(id);

            return RedirectToAction("Index");
        }
    }
}