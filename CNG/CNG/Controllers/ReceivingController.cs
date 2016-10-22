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
    public class ReceivingController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        
        public ReceivingController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page, int? companyId)
        {
            if (companyId.HasValue) {
                Sessions.CompanyId = companyId;
            }

            ViewBag.CompanyId = companyId;
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

            IQueryable<PurchaseOrder> lstReceivedPo = poRepo.ListReceived().Where(p => p.ShipTo == Sessions.CompanyId);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstReceivedPo = lstReceivedPo.Where(s => s.No.Contains(searchString)
                                       || s.Date.ToString().Contains(searchString)
                                       || s.Vendor.Name.ToString().Contains(searchString)
                                       || s.ShipToCompany.Name.Contains(searchString)
                                       || s.Terms.ToString().Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstReceivedPo = lstReceivedPo.OrderByDescending(p => p.Id);
            }
            else
            {
                lstReceivedPo = lstReceivedPo.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstReceivedPo.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            return View(po);
        }

        public ActionResult Create()
        {
            InitViewBags();

            int companyId = Convert.ToInt32(Session["companyId"]);

            ViewBag.PurchaseOrders = new SelectList(poRepo.ListForReceiving().Where(p => p.ShipTo == companyId), "No", "No");

            return View(new PurchaseOrder());
        }

        public ActionResult Edit(string poNo)
        {
            InitViewBags();
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
                poItem.RemainingBalanceDate = item.RemainingBalanceDate;

                context.SaveChanges();

                poRepo.ChangeStatus(receivingDTO.PoNo, receivingDTO.Status);

                if (poItem.ReceivedQuantity > 0)
                {
                    //delete last log first
                    DeleteLastLog(poItem.ItemId);

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

        public void DeleteLastLog(int itemId) {
            TransactionLog log = context.TransactionLogs.FirstOrDefault(p => 
            p.ItemId == itemId &&
            p.TransactionMethodId == (int)ETransactionMethod.Receiving);

            if (log != null) {
                context.TransactionLogs.Remove(log);

                context.SaveChanges();

                ItemRepository itemRepo = new ItemRepository();
                itemRepo.AdjustQuantity(log.ItemId, -1 * log.Quantity);
            }
        }

        private void InitViewBags()
        {
            ViewBag.CompanyId = Request.QueryString["companyId"];
        }
    }
}