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
    public class ReceivingController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        CompanyRepository companyRepo = new CompanyRepository();

        public ReceivingController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page, int? companyId)
        {
            if (companyId.HasValue) {
                Sessions.CompanyId = companyId;
            }

            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = companyRepo.GetById(Sessions.CompanyId.Value).Name;
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

                int quantity = 0;
                if (poItem.TransactionLogId.HasValue == false)
                {
                    quantity = poItem.ReceivedQuantity;
                }
                else {
                    TransactionLog transLog = poItem.TransactionLog;

                    quantity = poItem.ReceivedQuantity - transLog.CumulativeQuantity;
                }

                poItem.TransactionLogId = InsertLogs(poItem.ItemId, quantity, poItem.ReceivedQuantity);

                context.SaveChanges();
            }

            poRepo.ChangeStatus(receivingDTO.PoNo, receivingDTO.Status);
        }

        public int InsertLogs(int itemId, int quantiy, int cumulativeQuantity) {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                CumulativeQuantity = cumulativeQuantity,
                TransactionMethodId = (int)ETransactionMethod.Receiving
            };

            transactionLogRepo.Add(transactionLog);

            return transactionLog.Id;
        }

        private void InitViewBags()
        {
            ViewBag.CompanyId = Request.QueryString["companyId"];
        }
    }
}