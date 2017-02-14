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
    public class RequisitionController : Controller
    {
        private CNGDBContext context = new CNGDBContext();
        RequisitionRepository reqRepo = new RequisitionRepository();
        RequisitionItemRepository reqItemRepo = new RequisitionItemRepository();
        VehicleRepository vehicleRepo = new VehicleRepository();
        VehicleItemsRepository veItemRepo = new VehicleItemsRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();
       

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

            IQueryable<Requisition> lstReq = reqRepo.List().Where(p => p.CompanyId == Sessions.CompanyId);

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
            InitViewBags();
            var lstPlateNos = from p in vehicleRepo.List()
                              select new { No = p.LicenseNo };
            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No");

            RequisitionVM reqVM = new RequisitionVM
            {
                Requisition = new Requisition {
                    No = reqRepo.GenerateReqNo(),
                    Date = DateTime.Now,
                    JobOrderDate = DateTime.Now
                },
                ItemTypes = new SelectList(itemTypeRepo.List().ToList(), "Id", "Description")
            };

            return View(reqVM);
        }
        
        public ActionResult Edit(string reqNo)
        {
            InitViewBags();
            ViewBag.PurchaseOrders = new SelectList(reqRepo.List(), "No", "No");
            
            RequisitionVM reqVM = new RequisitionVM
            {
                Requisition = reqRepo.GetByNo(reqNo),
                ItemTypes = new SelectList(itemTypeRepo.List().ToList(), "Id", "Description")
            };

            var lstPlateNos = from p in vehicleRepo.List()
                              select new { No = p.LicenseNo };
            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No", reqVM.Requisition.UnitPlateNo);

            return View("Create", reqVM);
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

        public ActionResult RenderEditorRow(int itemId) {
            RequisitionItem reqItem = new RequisitionItem
            {
                Item = itemRepo.GetById(itemId),
                ItemId = itemId
            };

            RequisitionItemVM vm = new RequisitionItemVM
            {
                RequisitionItem = reqItem,
                CompanyId = Sessions.CompanyId.Value
            };

            return PartialView("_EditorRow", vm);
        }

        public void Save(RequisitionDTO entry)
        {
            try
            {
                Requisition req = new Requisition();

                req.No = entry.No;
                req.Date = entry.JobOrderDate;
                req.JobOrderNo = entry.JobOrderNo;
                req.UnitPlateNo = entry.UnitPlateNo;
                req.JobOrderDate = entry.JobOrderDate;
                req.OdometerReading = entry.OdometerReading; //Get from session
                req.DriverName = entry.DriverName; //Get from session
                req.ReportedBy = entry.ReportedBy; //Get from session
                req.CheckedBy = entry.CheckedBy; //Get from session
                req.ApprovedBy = Common.GetCurrentUser.GeneralManagerId; //Get from session

                req.CompanyId = Sessions.CompanyId.Value;

                int vehicleId = vehicleRepo.GetIdByPlateNo(entry.UnitPlateNo);

                List<RequisitionItem> lstReqItem = new List<RequisitionItem>();
                foreach (RequisitionDTO.Item item in entry.Items)
                {
                    RequisitionItem reqItem = new RequisitionItem
                    {
                        RequisitionId = req.Id,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        SerialNo = item.SerialNo,
                        Type = item.Type,
                        QuantityReturn = item.QuantityReturn,
                        SerialNoReturn = item.SerialNoReturn
                    };

                    lstReqItem.Add(reqItem);
                }

                req.RequisitionItems = lstReqItem;


                reqRepo.Save(req);
                int? translogId = req.RequisitionItems.Last().TransactionLogId;
                SaveVehicle(vehicleId, translogId);
            }
            catch { }
        }

        public void SaveVehicle(int vehicleId, int? translogId)
        {
            VehicleItems vi = new VehicleItems();
            vi.VehicleId = vehicleId;
            vi.TransactionLogId = translogId;

            veItemRepo.Save(vi);
        }

        public ActionResult Report(string poNo)
        {
            Requisition req = reqRepo.GetByNo(poNo);
            List<RequisitionItem> lstreqItem = req.RequisitionItems;

            var lstRequisition = from r in lstreqItem
                                 select new
                                 {
                                     RequestedBy = req.ReportedBy,
                                     ApprovedBy = req.ApprovedByObj.FullName,
                                     CheckedBy = req.CheckedBy,
                                     ItemCode = r.Item.Code,
                                     Description = r.Item.Description,
                                     Quantity = r.Quantity,
                                     Unit = req.UnitPlateNo,
                                     SerialNo = r.SerialNo

                                 };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstRequisition;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\Requisition\Report\rptRequisition.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Date", req.Date.ToShortDateString()));
            parameters.Add(new ReportParameter("ReqNumber", req.No));
            parameters.Add(new ReportParameter("TaxiUnit", req.UnitPlateNo));
            parameters.Add(new ReportParameter("Odometer", req.OdometerReading));
            parameters.Add(new ReportParameter("JONumber", req.JobOrderNo));
            parameters.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            parameters.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
        private void InitViewBags()
        {
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Description");

            ViewBag.ApprovedBy = Common.GetCurrentUser.GeneralManager.FullName;

            ViewBag.CompanyId = Request.QueryString["companyId"];
        }

        public ActionResult RequisitionSummaryReport(string dateFrom, string dateTo)
        {
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


            int comp = Convert.ToInt32(Sessions.CompanyId.Value);

            var lstReqSum = from req in reqRepo.List().Where(req => req.CompanyId == comp
                                                                 && (System.Data.Entity.DbFunctions.TruncateTime(req.Date) <= dtDateTo
                                                                && System.Data.Entity.DbFunctions.TruncateTime(req.Date) >= dtDateFrom)).ToList()
                                join reqItem in reqItemRepo.List() on req.Id equals reqItem.RequisitionId 
                                join V in vehicleRepo.List() on req.UnitPlateNo equals V.LicenseNo
                                into r
                                select new
                                {
                                    Date = req.Date.ToShortDateString(),
                                    PlateNo = req.UnitPlateNo,
                                    RNo = req.No,
                                    ItemDesc = reqItem.Item.Description,
                                    Quantity = reqItem.Quantity,
                                    VehicleUnit = r.FirstOrDefault().Model,
                                    CompanyName =req.Company.Prefix

                                };


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstReqSum;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\Requisition\Report\rptRequisitionSummary.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> _parameter = new List<ReportParameter>();
            _parameter.Add(new ReportParameter("DateRange", dtDateFrom.ToString("MMMM dd, yyyy") + " - " + dtDateTo.ToString("MMMM dd, yyyy")));
            _parameter.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            _parameter.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            reportViewer.LocalReport.SetParameters(_parameter);

            reportViewer.LocalReport.Refresh();
           

            ViewBag.ReportViewer = reportViewer;

            ViewBag.DateFrom = dtDateFrom.ToString("MM/dd/yyyy");
            ViewBag.DateTo = dtDateTo.ToString("MM/dd/yyyy");

            ViewBag.CompanyName = companyRepo.GetById(Sessions.CompanyId.Value).Name;

            return View();
        }
    }
}