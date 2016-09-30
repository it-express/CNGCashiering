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

        public ActionResult Details(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            return View(po);
        }

        public ActionResult Create()
        {
            ViewBag.PurchaseOrders = new SelectList(poRepo.ListForReceiving(), "No", "No");

            return View(new PurchaseOrder());
        }

        public ActionResult Edit(string poNo)
        {
            ViewBag.PurchaseOrders = new SelectList(poRepo.List(), "No", "No");

            PurchaseOrder po = poRepo.GetByNo(poNo);

            return View("Create", po);
        }
        
        public JsonResult ListItemByPoNo(string poNo)
        {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            List<PurchaseOrderItem> lstItem = po.PurchaseOrderItems;

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

                poRepo.ChangeStatus(receivingDTO.PoNo, receivingDTO.Status);

                if (receivingDTO.Status == (int)EPurchaseOrderStatus.Submitted)
                {
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
                TransactionMethodId = (int)ETransactionMethod.Receiving
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}