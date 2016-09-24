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
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        
        public ReceivingController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

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

        public void Save(ReceivingDTO receivingDTO) {
            foreach (ReceivingDTO.Item item in receivingDTO.Items) {
                PurchaseOrderItem poItem = poItemRepo.Find(item.PoItemId);

                poItem.SerialNo = item.SerialNo;
                poItem.ReceivedQuantity = item.ReceivedQuantity;
                poItem.DrNo = item.DrNo;
                poItem.Date = item.Date;

                context.SaveChanges();

                if (receivingDTO.IsCompleted == true) {
                    InsertLogs(poItem.ItemId, poItem.ReceivedQuantity);
                }
            }
        }

        public void InsertLogs(int itemId, int quantiy) {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.Receiving,
                Date = DateTime.Now
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}