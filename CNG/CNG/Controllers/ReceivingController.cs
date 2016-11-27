using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;
using PagedList;
using System.Linq.Dynamic;
using Microsoft.Reporting.WebForms;

namespace CNG.Controllers
{
    [AuthorizationFilter]
    public class ReceivingController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        CompanyRepository companyRepo = new CompanyRepository();
        ReceivingRepository receivingRepo = new ReceivingRepository();
        TransactionLogRepository transLogRepo = new TransactionLogRepository();

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

        public ActionResult RenderEditorRow(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            return PartialView("_EditorRow", po.PurchaseOrderItems.ToList());
        }

        public ActionResult RenderReceivingLogEditor(int poItemId)
        {
            PurchaseOrderItem poItem = poItemRepo.Find(poItemId);

            ViewBag.POItemId = poItemId;
            ViewBag.ItemDescription = poItem.Item.Description;
            IEnumerable<Receiving> lstReceiving = receivingRepo.ListByPurchaseOrderItemId(poItemId);

            return PartialView("_ReceivingLogEditor", lstReceiving);
        }

        public ActionResult RenderReceivingLogEditorRow(int receivingId) {
            Receiving receiving = new Receiving();
            if (receivingId != 0) {
                receivingRepo.GetById(receivingId);
            }

            return PartialView("_ReceivingLogEditorRow", receiving);
        }

        public void Save(ReceivingDTO receivingDTO) {
            poRepo.ChangeStatus(receivingDTO.PoNo, receivingDTO.Status);

            //foreach (ReceivingDTO.Item item in receivingDTO.Items) {
            //    PurchaseOrderItem poItem = poItemRepo.Find(item.PoItemId);

            //    poItem.SerialNo = item.SerialNo;
            //    //poItem.ReceivedQuantity = item.ReceivedQuantity;
            //    poItem.DrNo = item.DrNo;
            //    poItem.Date = item.Date;
            //    poItem.RemainingBalanceDate = item.RemainingBalanceDate;

            //    int quantity = 0;
            //    if (poItem.TransactionLogId.HasValue == false)
            //    {
            //        quantity = poItem.ReceivedQuantity;
            //    }
            //    else {
            //        TransactionLog transLog = poItem.TransactionLog;

            //        quantity = poItem.ReceivedQuantity - transLog.CumulativeQuantity;
            //    }

            //    if (quantity != 0) {
            //        poItem.TransactionLogId = InsertLogs(poItem.ItemId, quantity, poItem.ReceivedQuantity);
            //    }

            //    context.SaveChanges();
            //}
        }

        public void ReceivingLogsSave(ReceivingLogsDTO receivingLogsDTO) {
            PurchaseOrderItem poItem = poItemRepo.Find(receivingLogsDTO.PurchaseOrderItemId);
            PurchaseOrder po = context.PurchaseOrders.Find(poItem.PurchaseOrderId);

            po.Status = (int) EPurchaseOrderStatus.Saved;

            context.SaveChanges();

            foreach (ReceivingLogsDTO.Item item in receivingLogsDTO.Items)
            {
                Receiving receiving = new Receiving();

                if (item.Id != 0)
                {
                    receiving.Id = item.Id;
                }
                else {
                    receiving.TransactionLogId = InsertLogs(poItem.ItemId, item.Quantity, item.DateReceived);
                }

                receiving.PurchaseOrderItemId = receivingLogsDTO.PurchaseOrderItemId;
                receiving.Quantity = item.Quantity;
                receiving.SerialNo = item.SerialNo;
                receiving.DrNo = item.DrNo;
                receiving.DateReceived = item.DateReceived;

                receivingRepo.Save(receiving);
            }
        }

        public ActionResult Report(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            List<PurchaseOrderItem> lstReceiving = po.PurchaseOrderItems;

            var lstPurchaseOrder = from p in lstReceiving
                                   select new
                                   {
                                       No = po.No,
                                       CompanyName = po.ShipToCompany.Name,
                                       ItemCode = p.Item.Code,
                                       Description = p.Item.Description,
                                       Quantity = p.Quantity,
                                       UnitCost = p.UnitCost.ToString("N"),
                                       TotalAmount = p.Amount.ToString("N"),
                                       Balance = p.Balance,
                                       DateReceived = p.Date.ToShortDateString()
                                   };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstPurchaseOrder;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\Receiving\Report\rptReceiving.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public int InsertLogs(int itemId, int quantiy, DateTime dateReceived) {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.Receiving,
                CompanyId = Sessions.CompanyId.Value,
                Date = dateReceived
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