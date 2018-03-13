using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNGCashier.Models;
using PagedList;

namespace CNGCashier.Controllers
{
    public class UsersController : Controller
    {
        private CNGCashierDBContext db = new CNGCashierDBContext();
        UserRepo userRepo = new UserRepo();

        // GET: Users
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int ? page)
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

            IQueryable<User> users = userRepo.List().Include(u => u.GeneralManager).Include(u => u.UserType);

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

            if (!String.IsNullOrEmpty(sortColumn))
            {
                users = users.OrderByDescending(p => p.Id);
            }
            else
            {
                users = users.OrderBy( p => sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
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

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.GeneralManagerId = new SelectList(db.Users, "Id", "Username");
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Description");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,FirstName,LastName,UserTypeId,UserLevel,GeneralManagerId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GeneralManagerId = new SelectList(db.Users, "Id", "Username", user.GeneralManagerId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Description", user.UserTypeId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.GeneralManagerId = new SelectList(db.Users, "Id", "Username", user.GeneralManagerId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Description", user.UserTypeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,FirstName,LastName,UserTypeId,UserLevel,GeneralManagerId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GeneralManagerId = new SelectList(db.Users, "Id", "Username", user.GeneralManagerId);
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Description", user.UserTypeId);
            return View(user);
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
    }
}
