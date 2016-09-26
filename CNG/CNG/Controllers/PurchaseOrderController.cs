﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class PurchaseOrderController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        VendorRepository vendorRepo = new VendorRepository();
        ItemRepository itemRepo = new ItemRepository();

        public PurchaseOrderController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

        // GET: PurchaseOrder
        public ActionResult Index()
        {
            List<PurchaseOrder> lstPo = poRepo.List().ToList();

            return View(lstPo);
        }

        public ActionResult Create()
        {
            ViewBag.PoNumber = poRepo.GeneratePoNumber();
            ViewBag.Vendors = new SelectList(context.Vendors, "Id", "Name");
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            PurchaseOrder po = new PurchaseOrder();
            po.PurchaseOrderItems = new List<PurchaseOrderItem>();

            return View(po);
        }

        public ActionResult Edit(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            ViewBag.PoNumber = po.No;
            ViewBag.Vendors = new SelectList(context.Vendors, "Id", "Name");
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View("Create", po);
        }

        public string GeneratePoNumber() {
            string poNumber = poRepo.GeneratePoNumber();

            return poNumber;
        }

        [HttpPost]
        public JsonResult ListItemByPoNo(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            List<PurchaseOrderItem> lstPoItem = new List<PurchaseOrderItem>();
            if (po != null) {
                lstPoItem = po.PurchaseOrderItems;
            }

            return Json(lstPoItem);
        }

        public void Save(PurchaseOrderDTO entry) {
            PurchaseOrder po = new PurchaseOrder();

            po.No = poRepo.GeneratePoNumber();
            po.Date = DateTime.Now;
            po.VendorId = entry.VendorId;
            po.ShipTo = entry.ShipTo;
            po.Terms = vendorRepo.GetById(entry.VendorId).Terms;
            po.PreparedBy = 0; //Get from session
            po.ApprovedBy = 0; //Get from session

            context.PurchaseOrders.Add(po);
            context.SaveChanges();

            foreach (PurchaseOrderDTO.Item item in entry.Items) {
                PurchaseOrderItem poItem = new PurchaseOrderItem();
                poItem.PurchaseOrderId = po.Id;
                poItem.ItemId = item.Id;
                Item _item = itemRepo.GetById(item.Id);

                poItem.UnitCost = _item.UnitCost;
                poItem.Quantity = item.Quantity;
                poItem.Remarks = item.Remarks;
                poItem.Date = DateTime.Now;

                poItemRepo.Save(poItem);
            }
        }
    }
}