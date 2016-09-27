using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class ItemController : Controller
    {
        ItemRepository itemRepo = new ItemRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();

        public ActionResult Index()
        {
            List<Item> lstItem = itemRepo.List().ToList();

            return View(lstItem);
        }

        [HttpGet]
        public ViewResult Create()
        {
            InitViewBags();

            return View("Edit", new Item());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            InitViewBags();

            Item item = itemRepo.GetById(id);

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                itemRepo.Save(item);

                TempData["message"] = "Item has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            itemRepo.Delete(id);

            return RedirectToAction("Index");
        }

        public JsonResult GetById(int id)
        {
            Item item = itemRepo.GetById(id);

            return Json(item);
        }

        private void InitViewBags() {
            ViewBag.ItemTypes = new SelectList(itemTypeRepo.List(), "Id", "Description");

        }
    }
}