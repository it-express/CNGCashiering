using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class ReceivingController : Controller
    {
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo = new PurchaseOrderItemRepository();
        CNGDBContext context = new CNGDBContext();

        public ActionResult Index()
        {
            List<PurchaseOrder> lstReceivedPo = poRepo.ListReceived();

            return View(lstReceivedPo);
        }

        public ActionResult Create() {
            ViewBag.PurchaseOrders = new SelectList(poRepo.List(), "No", "No");

            return View();
        }
        
        public JsonResult ListItemByPoNo(string poNo)
        {
            List<PurchaseOrderItem> lstItem = poItemRepo.ListByPoNo(poNo);

            return Json(lstItem);
        }

        public ActionResult RequisitionToPurchase() {
            Session["selectedItems"] = null;

            ViewBag.rpNo = poRepo.GeneratePoNumber();
            ViewBag.Vendors = new SelectList(context.Vendors, "Id", "Name");
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View();
        }

        public ActionResult ExcessParts() {
            Session["selectedItems"] = null;

            ViewBag.rpNo = poRepo.GeneratePoNumber();
            ViewBag.Vendors = new SelectList(context.Vendors, "Id", "Name");
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View();
        }
    }
}