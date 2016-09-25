using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class RequisitionPurchaseController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        RequisitionPurchaseRepository rpRepo = new RequisitionPurchaseRepository();
        RequisitionPurchaseItemRepository rpItemRepo;

        public RequisitionPurchaseController() {
            rpItemRepo = new RequisitionPurchaseItemRepository(context);
        }

        // GET: RequisitionPurchase
        public ActionResult Index()
        {
            List<RequisitionPurchase> lst = rpRepo.List().ToList();

            return View(lst);
        }

        public ActionResult Create()
        {
            ViewBag.RpNo = rpRepo.GenerateRpNo();
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View();
        }

        public ActionResult Details(string rpNo) {
            RequisitionPurchase rp = rpRepo.GetByRpNo(rpNo);

            return View(rp);
        }

        public void Save(RequisitionPurchaseDTO rpDTO)
        {
            RequisitionPurchase rp = new RequisitionPurchase();
            rp.No = rpRepo.GenerateRpNo();
            rp.Date = DateTime.Now;
            rp.PreparedBy = 0; //Get from session
            rp.ApprovedBy = 0; //Get from session

            context.RequisitionPurchases.Add(rp);
            context.SaveChanges();

            foreach (RequisitionPurchaseDTO.Item dtoItem in rpDTO.Items)
            {
                RequisitionPurchaseItem rpItem = new RequisitionPurchaseItem();

                //public decimal UnitCost { get; set; }
                //public int Quantity { get; set; }
                //public string Remarks { get; set; }

                rpItem.RequisitionPurchaseId = rp.Id;
                rpItem.ItemId = dtoItem.ItemId;
                rpItem.UnitCost = dtoItem.UnitCost;
                rpItem.Quantity = dtoItem.Quantity;
                rpItem.Remarks = dtoItem.Remarks;

                context.RequisitionPurchaseItems.Add(rpItem);
                context.SaveChanges();

                InsertLogs(rpItem.ItemId, rpItem.Quantity);
            }
        }

        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.RequisitionToPurchase,
                Date = DateTime.Now
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}