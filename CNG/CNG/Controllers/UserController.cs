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
    public class UserController : Controller
    {
        UserRepository userRepo = new UserRepository();
        UserTypeRepository userTypeRepo = new UserTypeRepository();

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

            IQueryable<User> lstUser = userRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstUser = lstUser.Where(s => s.Username.Contains(searchString)
                                       || s.Password.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                       || s.LastName.Contains(searchString)
                                       || s.UserType.Description.Contains(searchString)
                                       || s.GeneralManager.FirstName.Contains(searchString)
                                       || s.GeneralManager.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstUser = lstUser.OrderByDescending(p => p.Id);
            }
            else
            {
                lstUser = lstUser.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstUser.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ViewResult Create()
        {
            InitViewBags();

            return View("Edit", new User());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            InitViewBags();

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
                InitViewBags();

                return View(user);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            userRepo.Delete(id);

            return RedirectToAction("Index");
        }

        private void InitViewBags()
        {
            ViewBag.UserType = SelectListHelper.UserTypes();
            ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
        }
    }
}