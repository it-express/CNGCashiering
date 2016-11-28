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
    public class StockTransferController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        StockTransferRepository stRepo = new StockTransferRepository();
        VehicleRepository vehicleRepo = new VehicleRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();

        // GET: StockTransfer
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            IQueryable<StockTransfer> lstStockTransfer = stRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstStockTransfer = lstStockTransfer.Where(s => s.No.Contains(searchString)
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
                lstStockTransfer = lstStockTransfer.OrderByDescending(p => p.Id);
            }
            else
            {
                lstStockTransfer = lstStockTransfer.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstStockTransfer.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            InitViewBags();
            var lstPlateNos = from p in vehicleRepo.List()
                              select new { No = p.LicenseNo };
            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No");

            StockTransferVM stVM = new StockTransferVM
            {
                StockTransfer = new StockTransfer
                {
                    No = stRepo.GenerateStockTransferNo(),
                    Date = DateTime.Now,
                    JobOrderDate = DateTime.Now
                },
                ItemTypes = new SelectList(itemTypeRepo.List().ToList(), "Id", "Description")
            };

            return View(stVM);
        }

        public ActionResult Edit(string stNo)
        {
            InitViewBags();
            ViewBag.PurchaseOrders = new SelectList(stRepo.List(), "No", "No");

            StockTransferVM stVM = new StockTransferVM
            {
                StockTransfer = stRepo.GetByNo(stNo),
                ItemTypes = new SelectList(itemTypeRepo.List().ToList(), "Id", "Description")
            };

            var lstPlateNos = from p in vehicleRepo.List()
                              select new { No = p.LicenseNo };
            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No", stVM.StockTransfer.UnitPlateNo);

            return View("Create", stVM);
        }

        public ActionResult Details(string stNo)
        {
            StockTransfer st = stRepo.GetByNo(stNo);

            ViewBag.CompanyId = Request.QueryString["companyId"];

            return View(st);
        }

        public ActionResult RenderEditorRow(int itemId)
        {
            StockTransferItem stItem = new StockTransferItem
            {
                Item = itemRepo.GetById(itemId),
                ItemId = itemId
            };

            StockTransferItemVM vm = new StockTransferItemVM
            {
                StockTransferItem = stItem,
                CompanyId = Sessions.CompanyId.Value
            };

            return PartialView("_EditorRow", vm);
        }

        public void Save(StockTransferDTO entry)
        {
            StockTransfer st = new StockTransfer();

            st.No = entry.No;
            st.Date = entry.JobOrderDate;
            st.CompanyTo = entry.CompanyTo;
            st.JobOrderNo = entry.JobOrderNo;
            st.UnitPlateNo = entry.UnitPlateNo;
            st.JobOrderDate = entry.JobOrderDate;
            st.OdometerReading = entry.OdometerReading; //Get from session
            st.DriverName = entry.DriverName; //Get from session
            st.ReportedBy = entry.ReportedBy; //Get from session
            st.CheckedBy = entry.CheckedBy; //Get from session
            st.ApprovedBy = Common.GetCurrentUser.GeneralManagerId; //Get from session

            st.CompanyId = Sessions.CompanyId.Value;

            List<StockTransferItem> lstStItem = new List<StockTransferItem>();
            foreach (StockTransferDTO.Item item in entry.Items)
            {
                StockTransferItem stItem = new StockTransferItem
                {
                    StockTransferId = st.Id,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    SerialNo = item.SerialNo,
                    Type = item.Type,
                    QuantityReturn = item.QuantityReturn,
                    SerialNoReturn = item.SerialNoReturn
                };

                lstStItem.Add(stItem);
            }

            st.StockTransferItems = lstStItem;

            stRepo.Save(st);
        }

        public ActionResult Delete(string stNo)
        {
            stRepo.Delete(stNo);

            return RedirectToAction("Index");
        }

        public void InsertLogs(int itemId, int quantiy, int companyId)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.StockTransfer,
                CompanyId = companyId
            };

            transactionLogRepo.Add(transactionLog);
        }

        private void InitViewBags()
        {
            ViewBag.Companies = new SelectList(companyRepo.List().Where(p => p.Id != Sessions.CompanyId.Value), "Id", "Name");
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Description");

            ViewBag.ApprovedBy = Common.GetCurrentUser.GeneralManager.FullName;

            ViewBag.CompanyId = Request.QueryString["companyId"];
        }
    }
}