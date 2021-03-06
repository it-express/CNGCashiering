﻿using System;
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
    public class ItemController : Controller
    {
        ItemRepository itemRepo = new ItemRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();
        ItemClassificationRepository itemClassificationRepo = new ItemClassificationRepository();
        ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();

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

            //IQueryable<Item> lstItem = itemRepo.List();

            IQueryable<ItemAssignment> lstItem = itemAssignmentRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstItem = lstItem.Where(s => s.Item.Code.Contains(searchString)
                                       || s.Item.Description.Contains(searchString)
                                       || s.UnitCost.ToString().Contains(searchString)
                                       || s.Item.Type.Description.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstItem = lstItem.OrderBy(p => p.Item.Description);
            }
            else {
                lstItem = lstItem.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstItem.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ViewResult Create()
        {
            ItemCode(0);
            InitViewBags();
            ViewBag.Action = "Create";

            return View("Edit", new Item());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ItemCode(id);
            InitViewBags();
            ViewBag.Action = "Update";
            Item item = itemRepo.GetById(id);

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {

                    string msg = itemRepo.SaveItem(item);

                if (msg == "save")
                {
                    TempData["message"] = "Item has been saved";
                }
                else if (msg == "updated")
                {
                    TempData["message"] = "Item has been updated";
                }
                else
                {
                    TempData["message"] = "Item already exist";
                }
                    return RedirectToAction("Index");
            }
            else
            {
                ItemCode(item.Id);
                InitViewBags();

                return View(item);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            string msg = itemRepo.Delete(id);
            if (msg == "save")
            {
                TempData["message"] = "Item has been deleted";
            }
            else
            {
                TempData["message"] = "Cannot delete item is used in another company";
            }

            return RedirectToAction("Index");
        }

        public ActionResult Assignment() {
            ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();

            List<ItemAssignmentVM> lstItemAssignmentVM = (from p in itemRepo.List().ToList().OrderBy(p => p.Description)
                                                          join q in itemAssignmentRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value).ToList() 
                                                               on p.Id equals q.ItemId into pq
                                                               from r in pq.DefaultIfEmpty()
                                                               select new ItemAssignmentVM
                                                         {
                                                             ItemId = p.Id,
                                                             Item = p,
                                                             IsAssigned = r == null ? false : true
                                                         }).ToList();

            return View(lstItemAssignmentVM);
        }

        [HttpPost]
        public ActionResult AssignmentSave(int[] ItemId) {
            ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();

            List<ItemAssignment> lstItemAssignment = new List<ItemAssignment>();
            foreach (int itemId in ItemId) {
                ItemAssignment itemAssign = new ItemAssignment
                {
                    ItemId = itemId,
                    CompanyId = Sessions.CompanyId.Value
                };

                lstItemAssignment.Add(itemAssign);
            }

            itemAssignmentRepo.Save(lstItemAssignment);

            return RedirectToAction("Index");
        }

        public JsonResult GetById(int id)
        {
            Item item = itemRepo.GetById(id);

            return Json(item);
        }

        private void InitViewBags() {

            Item item = new Item();
            ViewBag.ItemTypes = new SelectList(itemTypeRepo.List(), "Id", "Description");
            ViewBag.ItemClassifications = new SelectList(itemClassificationRepo.List(), "Id", "Description");

        }

        private void ItemCode(int itemid)
        {

            Item item = new Item();
            ViewBag.ItemCode = item.GetCode(itemid);
        }
    }
}