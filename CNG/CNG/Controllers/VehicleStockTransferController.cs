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
    public class VehicleStockTransferController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        VehicleStockTransferRepository vstRepo = new VehicleStockTransferRepository();
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

            IQueryable<VehicleStockTransfer> lstSt = vstRepo.List();

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

        public ActionResult RenderEditorRow(int itemId, int vehicleFromId, int vehicleToId, string remarks)
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
                Remarks = remarks
            };

            return PartialView("_EditorRow", vm);
        }

        public void Save(VehicleStockTransferDTO entry) {
            VehicleStockTransfer vst = new VehicleStockTransfer();

            vst.No = entry.No;

            vst.Date = Convert.ToDateTime(entry.Date);

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
                vstItem.Remarks = item.Remarks;

                vst.VehicleStockTransferItems.Add(vstItem);
            }

            vstRepo.Save(vst);
        }

        public ActionResult Details(string vstNo)
        {
            VehicleStockTransfer vst = vstRepo.GetByNo(vstNo);

            return View(vst);
        }

    }
}