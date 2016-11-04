using System;
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
    public class RequisitionPurchaseController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        RequisitionPurchaseRepository rpRepo = new RequisitionPurchaseRepository();
        RequisitionPurchaseItemRepository rpItemRepo;
        CompanyRepository companyRepo = new CompanyRepository();

        public RequisitionPurchaseController() {
            rpItemRepo = new RequisitionPurchaseItemRepository(context);
        }

        // GET: RequisitionPurchase
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

            IQueryable<RequisitionPurchase> lstRp = rpRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstRp = lstRp.Where(s => s.Date.ToString().Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstRp = lstRp.OrderByDescending(p => p.Id);
            }
            else
            {
                lstRp = lstRp.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstRp.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.RpNo = rpRepo.GenerateRpNo();
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Items = new SelectList(context.Items, "Id", "Code");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            return View();
        }

        public ActionResult Remove(string rpNo) {
            rpRepo.Delete(rpNo);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int rpNo) {
            RequisitionPurchase rp = rpRepo.GetById(rpNo);
            rp.RequisitionPurchaseItems = new List<RequisitionPurchaseItem>();

            return View(rp);
        }

        public void Save(RequisitionPurchaseDTO rpDTO)
        {
            RequisitionPurchase rp = new RequisitionPurchase();
            rp.No = rpRepo.GenerateRpNo();
            rp.Date = DateTime.Now;

            rp.PreparedBy = Common.GetCurrentUser.Id;
            rp.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;

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
                CompanyId = Sessions.CompanyId.Value
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}