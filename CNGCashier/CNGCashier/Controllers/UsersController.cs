using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNGCashier.Models;
using PagedList;

namespace CNGCashier.Controllers
{
    [AuthorizationFilter]
    public class UsersController : Controller
    {
        private CNGCashierDBContext db = new CNGCashierDBContext();
        UserRepo userRepo = new UserRepo();
        UserTypeRepo usertypeRepo = new UserTypeRepo();

        // GET: Users
        public ActionResult Index(string sortColumn, string sortOrder,string nextpage, string currentFilter, string searchString, int ? page)
        {
            ViewBag.CurrentSort = sortColumn;
            ViewBag.SortOrder = sortOrder;

            if(nextpage == null)
                sortOrder = sortOrder == "asc" ? "desc" : "asc";

            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}
            ViewBag.CurrentFilter = searchString;

            IQueryable<User> users = userRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Username.Contains(searchString)
                                       || u.Password.Contains(searchString)
                                       || u.FirstName.Contains(searchString)
                                       || u.LastName.Contains(searchString)
                                       || u.UserType.Description.Contains(searchString)
                                       || u.GeneralManager.FirstName.Contains(searchString)
                                       || u.GeneralManager.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                users = users.OrderByDescending(u => u.Id);
            }
            else
            {
                users = users.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        //// GET: Users/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        [HttpGet]
        public ActionResult Create()
        {
            //ViewBag.UserType = SelectListHelper.UserTypes();
            //ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
            //return View();
            InitViewBags();

            return View("Edit", new User());
        }

      
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Username,Password,FirstName,LastName,UserTypeId,UserLevel,GeneralManagerId")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(user);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.UserType = SelectListHelper.UserTypes();
        //    ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
        //    return View(user);
        //}

        [HttpGet]
        public ActionResult Edit(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //User user = db.Users.Find(id);
            //if (user == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.UserType = SelectListHelper.UserTypes();
            //ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
            //return View(user);
            InitViewBags();

            User user = userRepo.GetById(id);

            return View(user);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Id,Username,Password,FirstName,LastName,UserTypeId,UserLevel,GeneralManagerId")]*/ User user)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(user).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //ViewBag.UserType = SelectListHelper.UserTypes();
            //ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
            //return View(user);
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

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

        private void InitViewBags()
        {
            ViewBag.UserType = SelectListHelper.UserTypes();
            ViewBag.GeneralManager = SelectListHelper.GeneralManagers();
        }
    }
}
