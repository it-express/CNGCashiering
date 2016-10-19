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
    public class RequisitionController : Controller
    {
        private CNGDBContext context = new CNGDBContext();
        RequisitionRepository reqRepo = new RequisitionRepository();
        RequisitionItemRepository reqItemRepo;
        VehicleRepository vehicleRepo = new VehicleRepository();
        ItemRepository itemRepo = new ItemRepository();

        public RequisitionController() {
            reqItemRepo = new RequisitionItemRepository(context);
        }

        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page, int companyId = 1)
        {
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

            IQueryable<Requisition> lstReq = reqRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstReq = lstReq.Where(s => s.No.Contains(searchString)
                                       || s.Date.ToString().Contains(searchString)
                                       || s.JobOrderNo.Contains(searchString)
                                       || s.UnitPlateNo.Contains(searchString)
                                       || s.OdometerReading.ToString().Contains(searchString)
                                       || s.DriverName.Contains(searchString)
                                       || s.ReportedBy.Contains(searchString)
                                       || s.CheckedBy.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstReq = lstReq.OrderByDescending(p => p.Id);
            }
            else
            {
                lstReq = lstReq.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstReq.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create() {
            ViewBag.reqNumber = reqRepo.GenerateReqNo();

            var lstPlateNos = from  p in vehicleRepo.List()
                              select new { No = p.LicenseNo };

            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No");
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Code");

            ViewBag.ApprovedBy = Common.GetCurrentUser.FullName;

            ViewBag.CompanyId = Request.QueryString["companyId"];

            return View(new Requisition());
        }

        public ActionResult Delete(string reqNo) {
            reqRepo.Delete(reqNo);

            return RedirectToAction("Index");
        }

        public ActionResult Details(string reqNo) {
            Requisition req = reqRepo.GetByNo(reqNo);
            req.RequisitionItems = new List<RequisitionItem>();

            ViewBag.CompanyId = Request.QueryString["companyId"];

            return View(req);
        }

        public void Save(RequisitionDTO entry)
        {
            Requisition req = new Requisition();

            req.No = reqRepo.GenerateReqNo();
            req.Date = entry.JobOrderDate;
            req.JobOrderNo = entry.JobOrderNo;
            req.UnitPlateNo = entry.UnitPlateNo;
            req.JobOrderDate = entry.JobOrderDate;
            req.OdometerReading = entry.OdometerReading; //Get from session
            req.DriverName = entry.DriverName; //Get from session
            req.ReportedBy = entry.ReportedBy; //Get from session
            req.CheckedBy = entry.CheckedBy; //Get from session
            req.ApprovedBy = Common.GetCurrentUser.Id; //Get from session

            context.Requisitions.Add(req);
            context.SaveChanges();

            foreach (RequisitionDTO.Item item in entry.Items)
            {
                RequisitionItem reqItem = new RequisitionItem();
                reqItem.RequisitionId = req.Id;

                Item _item = itemRepo.GetById(item.ItemId);

                reqItem.ItemId = item.ItemId;
                reqItem.Quantity = item.Quantity;
                reqItem.SerialNo = item.SerialNo;
                reqItem.Type = item.Type;

                reqItemRepo.Save(reqItem);

                InsertLogs(item.ItemId, reqItem.Quantity);
            }
        }
        
        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                TransactionMethodId = (int)ETransactionMethod.Requisition
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}