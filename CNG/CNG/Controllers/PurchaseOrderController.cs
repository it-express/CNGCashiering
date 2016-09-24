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
        VendorRepository vendorRepo = new VendorRepository();
        ItemRepository itemRepo = new ItemRepository();

        // GET: PurchaseOrder
        public ActionResult Index()
        {
            List<PurchaseOrder> lstPo = poRepo.List().ToList();

            return View(lstPo);
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

        public string GeneratePoNumber() {
            string poNumber = poRepo.GeneratePoNumber();

            return poNumber;
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
                poItem.PurchaseOrderNo = po.No;
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