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

        public ActionResult Create()
        {
            ViewBag.PurchaseOrders = new SelectList(poRepo.List(), "No", "No");

            return View();
        }
        
        public JsonResult ListItemByPoNo(string poNo)
        {
            List<PurchaseOrderItem> lstItem = poItemRepo.ListByPoNo(poNo);

            return Json(lstItem);
        }
    }
}