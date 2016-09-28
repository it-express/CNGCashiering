using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNG.Controllers
{
    public class ExcessPartsSetController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        ExcessPartsSetRepository epsRepo = new ExcessPartsSetRepository();
        ExcessPartsSetItemRepository epsItemRepo;

        public ExcessPartsSetController()
        {
            epsItemRepo = new ExcessPartsSetItemRepository(context);
        }

        // GET: RequisitionPurchase
        public ActionResult Index()
        {
            List<ExcessPartsSet> lst = epsRepo.List().ToList();

            return View(lst);
        }

        public ActionResult Create()
        {
            ViewBag.EpsNo = epsRepo.GenerateEpsNo();
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");

            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            ExcessPartsSet eps = epsRepo.GetById(id);

            return View(eps);
        }

        public void Save(ExcessPartsSetDTO epsDTO)
        {
            ExcessPartsSet eps = new ExcessPartsSet();
            eps.No = epsRepo.GenerateEpsNo();
            eps.Date = DateTime.Now;
            eps.PreparedBy = 0; //Get from session
            eps.ApprovedBy = 0; //Get from session

            context.ExcessPartsSets.Add(eps);
            context.SaveChanges();

            foreach (ExcessPartsSetDTO.Item dtoItem in epsDTO.Items)
            {
                ExcessPartsSetItem epsItem = new ExcessPartsSetItem();

                //public decimal UnitCost { get; set; }
                //public int Quantity { get; set; }
                //public string Remarks { get; set; }

                epsItem.ExcessPartsSetId = eps.Id;
                epsItem.ItemId = dtoItem.ItemId;
                epsItem.UnitCost = dtoItem.UnitCost;
                epsItem.Quantity = dtoItem.Quantity;
                epsItem.Remarks = dtoItem.Remarks;

                context.ExcessPartsSetItems.Add(epsItem);
                context.SaveChanges();

                InsertLogs(epsItem.ItemId, epsItem.Quantity);
            }
        }

        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.ExcessPartsFromSet
            };

            transactionLogRepo.Add(transactionLog);
        }

        public ActionResult Remove(string epsNo) {
            epsRepo.Delete(epsNo);

            return RedirectToAction("Index");
        }
    }
}