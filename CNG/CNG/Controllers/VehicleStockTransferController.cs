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
    public class VehicleStockTransferController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        VehicleStockTransferRepository vstRepo = new VehicleStockTransferRepository();
        VehicleItemsRepository veItemRepo = new VehicleItemsRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemRepository itemRepo = new ItemRepository();
        VehicleRepository vehicleRepo = new VehicleRepository();

        // GET: VehicleStockTransfer
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

            IQueryable<VehicleStockTransfer> lstSt = vstRepo.List().Where(p => p.CompanyId == Sessions.CompanyId);

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    lstSt = lstSt.Where(s => s.Date.ToString().Contains(searchString)
            //                           || s.PreparedByObj.FirstName.Contains(searchString)
            //                           || s.PreparedByObj.LastName.Contains(searchString)
            //                           || s.ApprovedByObj.FirstName.Contains(searchString)
            //                           || s.ApprovedByObj.LastName.Contains(searchString));
            //}

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstSt = lstSt.OrderByDescending(p => p.Id);
            }
            else
            {
                lstSt = lstSt.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(lstSt.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.Items = new SelectList(context.Items, "Id", "Description");
            ViewBag.PlateNos = new SelectList(context.Vehicles, "Id", "LicenseNo");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            VehicleStockTransfer vehicleStockTransfer = new VehicleStockTransfer
            {
                No = vstRepo.GenerateVehicleStockTransferNo(),
                Date = DateTime.Now
            };

            return View(vehicleStockTransfer);
        }

        public ActionResult RenderEditorRow(int itemId, int vehicleFromId, int vehicleToId, int quantity, string remarks)
        {
            Item item = itemRepo.GetById(itemId);
            Vehicle vehicleFrom = vehicleRepo.GetById(vehicleFromId);
            Vehicle vehicleTo = vehicleRepo.GetById(vehicleToId);

            VehicleStockTransferItemVM vm = new VehicleStockTransferItemVM
            {
                ItemId = itemId,
                ItemCode = item.Code,
                ItemDescription = item.Description,
                VehicleFromId = vehicleFromId,
                VehicleToId = vehicleToId,
                VehicleFromPlateNo = vehicleFrom.LicenseNo,
                VehicleToPlateNo = vehicleTo.LicenseNo, 
                Quantity = quantity,
                Remarks = remarks
            };

            return PartialView("_EditorRow", vm);
        }

        public void Save(VehicleStockTransferDTO entry) {
            VehicleStockTransfer vst = new VehicleStockTransfer();

            vst.No = entry.No;

            vst.Date = Convert.ToDateTime(entry.Date);
            vst.CompanyId = Sessions.CompanyId.Value;

            vst.RequestedBy = entry.RequestedBy;
            vst.CheckedBy = Common.GetCurrentUser.Id;
            vst.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;

            vst.VehicleStockTransferItems = new List<VehicleStockTransferItem>();
            foreach (VehicleStockTransferDTO.Item item in entry.Items)
            {
                VehicleStockTransferItem vstItem = new VehicleStockTransferItem();
                vstItem.VehicleStockTransferId = vst.Id;
                vstItem.ItemId = item.Id;
                vstItem.VehicleFromId = item.VehicleFromId;
                vstItem.VehicleToId = item.VehicleToId;
                vstItem.Quantity = item.Quantity;
                vstItem.Remarks = item.Remarks;

                vst.VehicleStockTransferItems.Add(vstItem);
            }

            vstRepo.Save(vst);

            int? translogId = vst.VehicleStockTransferItems.Last().TransactionLogId;
            int vehicleId = vst.VehicleStockTransferItems.Last().VehicleToId;
            SaveVehicle(vehicleId, translogId);
        }

        public void SaveVehicle(int vehicleId, int? translogId)
        {
            VehicleItems vi = new VehicleItems();
            vi.VehicleId = vehicleId;
            vi.TransactionLogId = translogId;

            veItemRepo.Save(vi);
        }


        public ActionResult Details(string vstNo)
        {
            VehicleStockTransfer vst = vstRepo.GetByNo(vstNo);

            return View(vst);
        }

        public ActionResult Report(string poNo)
        {
            VehicleStockTransfer st = vstRepo.GetByNo(poNo);
            List<VehicleStockTransferItem> lstVSTransferItem = st.VehicleStockTransferItems;

            var lstStockTransfer = from s in lstVSTransferItem
                                   select new
                                   {
                                       RequestedBy = st.RequestedBy,
                                       ApprovedBy = st.ApprovedByObj.FullName,
                                       CheckedBy = st.CheckedBy,
                                       ItemCode = s.Item.Code,
                                       Description = s.Item.Description,                  
                                       Quantity = s.Quantity,
                                       PlateNoFrom = s.VehicleFrom.LicenseNo,
                                       PlateNoTo = s.VehicleTo.LicenseNo,
                                       Remarks = s.Remarks
                                   };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstStockTransfer;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\VehicleStockTransfer\Report\rptStockTransfer.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Date", st.Date.ToShortDateString()));
            parameters.Add(new ReportParameter("No", st.No));
            parameters.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            parameters.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult SummaryReport(string dateFrom, string dateTo)
        {
            CNGDBContext context = new CNGDBContext();
            VehicleStockTransferItemRepository vstItemRepo = new VehicleStockTransferItemRepository(context);

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

            var lstSt = from st in vstRepo.List().Where(st => st.CompanyId == comp
                                                                && (System.Data.Entity.DbFunctions.TruncateTime(st.Date) <= dtDateTo
                                                               && System.Data.Entity.DbFunctions.TruncateTime(st.Date) >= dtDateFrom)).ToList()
                        join stItem in vstItemRepo.List() on st.Id equals stItem.VehicleStockTransferId
                        into r
                        select new
                        {
                            Date = st.Date.ToShortDateString(),
                            TransferNo = st.No,
                            ItemDesc = r.FirstOrDefault().Item.Description,
                            Quantity = r.FirstOrDefault().Quantity,
                            VehicleFrom = r.FirstOrDefault().VehicleFrom.LicenseNo,           
                            VehicleTo = r.FirstOrDefault().VehicleTo.LicenseNo,
                            Mechanic = st.RequestedBy,
                            Remarks = r.FirstOrDefault().Remarks,

                        };


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstSt;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\VehicleStockTransfer\Report\rptSummaryReport.rdlc";

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