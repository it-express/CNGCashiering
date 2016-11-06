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
    public class RequisitionController : Controller
    {
        private CNGDBContext context = new CNGDBContext();
        RequisitionRepository reqRepo = new RequisitionRepository();
        RequisitionItemRepository reqItemRepo;
        VehicleRepository vehicleRepo = new VehicleRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();

        public RequisitionController() {
            reqItemRepo = new RequisitionItemRepository(context);
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
                RequisitionItem = reqItem
            };

            return PartialView("_EditorRow", vm);
        }

        public void Save(RequisitionDTO entry)
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
            req.ApprovedBy = Common.GetCurrentUser.Id; //Get from session

            req.CompanyId = Sessions.CompanyId.Value;

            List<RequisitionItem> lstReqItem = new List<RequisitionItem>();
            foreach (RequisitionDTO.Item item in entry.Items) {
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
        }
        
        private void InitViewBags()
        {
            
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Description");

            ViewBag.ApprovedBy = Common.GetCurrentUser.FullName;

            ViewBag.CompanyId = Request.QueryString["companyId"];
        }
    }
}