using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo = new PurchaseOrderItemRepository();

        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            Session["selectedItems"] = null;

            ViewBag.PoNumber = poRepo.GeneratePoNumber();
            ViewBag.Vendors = new SelectList(context.Vendors, "Id", "Name");
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View(new PurchaseOrder());
        }

        public void AddItem(int itemId)
        {
            List<Item> lstItem = GetSelectedItems();

            Item item = context.Items.FirstOrDefault(p => p.Id == itemId);

            lstItem.Add(item);

            Session["selectedItems"] = lstItem;
        }

        public JsonResult ListItems() {
            List<Item> lstItem = GetSelectedItems();

            return Json(lstItem);
        }

        private List<Item> GetSelectedItems() {
            List<Item> lstItem;

            if (Session["selectedItems"] == null)
            {
                lstItem = new List<Item>();
            }
            else
            {
                lstItem = (List<Item>)Session["selectedItems"];
            }

            return lstItem;
        }

        public string GeneratePoNumber() {
            string poNumber = poRepo.GeneratePoNumber();

            return poNumber;
        }

        public void Save(PurchaseOrder entry) {
            PurchaseOrder po = new PurchaseOrder();

            po.No = poRepo.GeneratePoNumber();
            po.Date = DateTime.Now;
            po.VendorId = entry.VendorId;
            po.ShipTo = entry.ShipTo;
            po.Terms = entry.Terms;
            po.PreparedBy = 0;
            po.ApprovedBy = 0;

            context.PurchaseOrders.Add(po);
            context.SaveChanges();

            //save selected items to purchase order items
            List<Item> lstItem = GetSelectedItems();
            foreach (Item item in lstItem) {
                PurchaseOrderItem poItem = new PurchaseOrderItem();
                poItem.PurchaseOrderNo = po.No;
                poItem.ItemId = item.Id;
                poItem.UnitCost = item.UnitCost;
                poItem.Remarks = "";
                poItem.Date = DateTime.Now;

                poItemRepo.Save(poItem);
            }
        }
    }
}