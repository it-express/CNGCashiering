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
    public class StockTransferController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        StockTransferRepository stRepo = new StockTransferRepository();
        StockTransferItemRepository stItemRepo = new StockTransferItemRepository();
        VehicleRepository vehicleRepo = new VehicleRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();
        TransactionLogRepository translogRepo = new TransactionLogRepository();
        VehicleItemsRepository veItemRepo = new VehicleItemsRepository();

        // GET: StockTransfer
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page, string dateFrom, string dateTo)
        {
            ViewBag.CompanyName = companyRepo.GetById(Sessions.CompanyId.Value).Name;
            ViewBag.CurrentSort = sortColumn;
            ViewBag.SortOrder = sortOrder == "asc" ? "desc" : "asc";

            DateTime dtDateFrom = DateTime.Now.Date;
            DateTime dtDateTo = DateTime.Now;

            if (!String.IsNullOrEmpty(dateFrom))
            {
                dtDateFrom = Convert.ToDateTime(dateFrom);
            }

            if (!String.IsNullOrEmpty(dateTo))
            {
                dtDateTo = Convert.ToDateTime(dateTo);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<StockTransfer> lstStockTransfer = stRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value && (p.Date >= dtDateFrom && p.Date <= dtDateTo));

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


            ViewBag.DateFrom = dtDateFrom.ToString("MM/dd/yyyy");
            ViewBag.DateTo = dtDateTo.ToString("MM/dd/yyyy");
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

            int? translogId = st.StockTransferItems.Last().TransactionLogId;
            int vehicleId = vehicleRepo.GetIdByPlateNo(st.UnitPlateNo);
            SaveVehicle(vehicleId, translogId);

        }

        public void SaveVehicle(int vehicleId, int? translogId)
        {
            VehicleItems vi = new VehicleItems();
            vi.VehicleId = vehicleId;
            vi.TransactionLogId = translogId;

            veItemRepo.Save(vi);
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
                TransactionMethodId = (int)ETransactionMethod.StockTransfer_Company,
                CompanyId = companyId,
                ItemTypeId = itemRepo.GetItemType(itemId)
            };

            transactionLogRepo.Add(transactionLog);
        }

        public ActionResult Report(string poNo)
        {
            StockTransfer st = stRepo.GetByNo(poNo);
            List<StockTransferItem> lstStockTransferItem = st.StockTransferItems;

            var lstStockTransfer = from s in lstStockTransferItem
                                 select new
                                 {
                                     ApprovedBy = st.ApprovedByObj.FullName,
                                     CheckedBy = st.CheckedBy,
                                     RequestedBy = st.ReportedBy,
                                     ItemCode = s.Item.Code,
                                     Description = s.Item.Description,
                                     Quantity = s.Quantity,
                                     SerialNo = s.SerialNo,
                                     Unit = st.UnitPlateNo
                                     
                                 };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstStockTransfer;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\StockTransfer\Report\rptStockTransfer.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Date", st.Date.ToShortDateString()));
            parameters.Add(new ReportParameter("No", st.No));
            parameters.Add(new ReportParameter("TransferredTo", companyRepo.GetById(st.CompanyTo).Name));
            parameters.Add(new ReportParameter("JobOrderNo", st.JobOrderNo));
            parameters.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            parameters.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        private void InitViewBags()
        {
            ViewBag.Companies = new SelectList(companyRepo.List().Where(p => p.Id != Sessions.CompanyId.Value), "Id", "Name");
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Description");

            ViewBag.ApprovedBy = Common.GetCurrentUser.GeneralManager.FullName;

            ViewBag.CompanyId = Request.QueryString["companyId"];

            var affectedRows1 = context.Database.ExecuteSqlCommand("spUpdate_Items_QuantityOnHand");
        }

        public ActionResult SummaryReport(string dateFrom, string dateTo, int? companyTo)
        {
            DateTime dtDateFrom = DateTime.Now.Date;
            DateTime dtDateTo = DateTime.Now;

            if (companyTo == null)
            {
                companyTo = 0;
            }
           
            if (!String.IsNullOrEmpty(dateFrom))
            {
                dtDateFrom = Convert.ToDateTime(dateFrom);
            }

            if (!String.IsNullOrEmpty(dateTo))
            {
                dtDateTo = Convert.ToDateTime(dateTo);
            }


            int comp = Convert.ToInt32(Sessions.CompanyId.Value);

            var lstSt  = from st in stRepo.List().Where(st => st.CompanyId == comp
                                                                && (System.Data.Entity.DbFunctions.TruncateTime(st.Date) <= dtDateTo
                                                                && System.Data.Entity.DbFunctions.TruncateTime(st.Date) >= dtDateFrom)
                                                                && st.CompanyTo == companyTo).ToList()
                            join stItem in stItemRepo.List() on st.Id equals stItem.StockTransferId
                            into r
                            select new
                            {
                                Date = st.Date.ToString("MM/dd/yyyy"),
                                PlateNo = st.UnitPlateNo,
                                No = st.No,
                                ItemDesc = r.FirstOrDefault().Item.Description,
                                Quantity = r.FirstOrDefault().Quantity,
                                UnitCost = r.FirstOrDefault().Item.UnitCost,
                                Amount = r.FirstOrDefault().Quantity * r.FirstOrDefault().Item.UnitCost


                            };


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstSt;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\StockTransfer\Report\rptSummaryReport.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> _parameter = new List<ReportParameter>();
            _parameter.Add(new ReportParameter("DateRange", dtDateFrom.ToString("MMMM dd, yyyy") + " - " + dtDateTo.ToString("MMMM dd, yyyy")));
            _parameter.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            _parameter.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            if (companyTo == 0)
            {
                _parameter.Add(new ReportParameter("CompanyTo",""));
            }
            else
            {
                _parameter.Add(new ReportParameter("CompanyTo", companyRepo.GetById(Convert.ToInt32(companyTo)).Name));
            }
            reportViewer.LocalReport.SetParameters(_parameter);

            reportViewer.LocalReport.Refresh();


            ViewBag.ReportViewer = reportViewer;

            ViewBag.DateFrom = dtDateFrom.ToString("MM/dd/yyyy");
            ViewBag.DateTo = dtDateTo.ToString("MM/dd/yyyy");

            ViewBag.CompanyName = companyRepo.GetById(Sessions.CompanyId.Value).Name;

            ViewBag.Companies = new SelectList(companyRepo.List().Where(p => p.Id != Sessions.CompanyId.Value), "Id", "Name");

            return View();
        }

        public JsonResult GetById(int CompanyID)
        {
            VehicleAssignmentRepository vehicleAssignmentRepo = new VehicleAssignmentRepository();

            List<VehicleAssignmentVM> lstItemAssignmentVM = (from p in vehicleRepo.List().ToList()
                                                             join q in vehicleAssignmentRepo.List().Where(p => p.CompanyId == CompanyID).ToList()
                                                             on p.Id equals q.VehicleId into pq
                                                             from r in pq.DefaultIfEmpty() where r == null ? false : true
                                                             select new VehicleAssignmentVM
                                                             {
                                                                 VehicleId = p.Id,
                                                                 VehiclePlateNo = p.LicenseNo
                                                               
                                                           }).ToList();      

            return Json(lstItemAssignmentVM);
        }
    }
}