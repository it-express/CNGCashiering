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
    public class RequisitionPurchaseController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository rpRepo = new PurchaseOrderRepository();
        RequisitionPurchaseItemRepository rpItemRepo;
        CompanyRepository companyRepo = new CompanyRepository();
        ItemRepository itemRepo = new ItemRepository();
        ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();
        UserRepository userRepo = new UserRepository();

        public RequisitionPurchaseController() {
            rpItemRepo = new RequisitionPurchaseItemRepository(context);
        }

        // GET: RequisitionPurchase
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page, int? companyId)
        {
            if (companyId.HasValue)
            {
                Sessions.CompanyId = companyId;
            }

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

            IQueryable<PurchaseOrder> lstRp = rpRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value && p.isRP == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstRp = lstRp.Where(s => s.Date.ToString().Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstRp = lstRp.OrderByDescending(p => p.Id);
            }
            else
            {
                lstRp = lstRp.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstRp.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            IQueryable<Item> lstItem = itemAssignmentRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value).Select(p => p.Item);
            ViewBag.Items = new SelectList(lstItem.Where(p => p.Active).OrderBy(p => p.Description), "Id", "Description");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            PurchaseOrder reqPurchase = new PurchaseOrder
            {
                No = rpRepo.GenerateRpNo(DateTime.Now),
                Date = DateTime.Now
            };

            return View(reqPurchase);
        }

        public ActionResult Remove(string rpNo) {
            rpRepo.Delete(rpNo);

            return RedirectToAction("Index");
        }

        public ActionResult Details(string rpNo) {
            PurchaseOrder rp = rpRepo.GetByNo(rpNo);
            rp.PurchaseOrderItems = new List<PurchaseOrderItem>();

            ViewBag.UserLevel = userRepo.GetByUserLevel(Common.GetCurrentUser.Id);

            return View(rp);
        }

        public ActionResult RenderEditorRow(int itemId)
        {
            PurchaseOrderItem reqPurchaseItem = new PurchaseOrderItem
            {
                Item = itemRepo.GetById(itemId)
            };

            return PartialView("_EditorRow", reqPurchaseItem);
        }

        public void Save(RequisitionPurchaseDTO rpDTO)
        {
            PurchaseOrder rp = new PurchaseOrder();
            int id;
            rp.No = rpRepo.GenerateRpNo(rpDTO.Date);
            rp.Date = rpDTO.Date;
            rp.CompanyId = Sessions.CompanyId.Value;

            rp.PreparedBy = Common.GetCurrentUser.Id;
            rp.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
            rp.CheckedBy = rpDTO.CheckedBy;
            rp.isRP = true;

            context.PurchaseOrders.Add(rp);
            context.SaveChanges();

            id = rp.Id;

            rp.ItemPriceLogs = new List<ItemPriceLogs>();
            foreach (RequisitionPurchaseDTO.Item item in rpDTO.Items)
            {
                ItemPriceLogs itemLogs = new ItemPriceLogs();

                itemLogs.PurchaseOrderId = rp.Id;
                itemLogs.ItemId = item.ItemId;
                itemLogs.UnitCost = Convert.ToDecimal(item.UnitCost);
                itemLogs.Qty = item.Quantity;
                itemLogs.Date = DateTime.Now;

                rp.ItemPriceLogs.Add(itemLogs);

            }

            foreach (RequisitionPurchaseDTO.Item dtoItem in rpDTO.Items)
            {
                PurchaseOrderItem rpItem = new PurchaseOrderItem();

                rpItem.PurchaseOrderId = rp.Id;
                rpItem.ItemId = dtoItem.ItemId;
                rpItem.UnitCost = dtoItem.UnitCost;
                rpItem.Quantity = dtoItem.Quantity;
                rpItem.Remarks = dtoItem.Remarks;
                rpItem.Date = DateTime.Now;

                context.PurchaseOrderItems.Add(rpItem);
              
                context.SaveChanges();

                InsertStockCard(rp.Id, dtoItem.ItemId, dtoItem.UnitCost, dtoItem.Quantity);
            }
        }


        public ActionResult Report(string ReqPoNo)
        {
            PurchaseOrder rp = rpRepo.GetByNo(ReqPoNo);
            List<PurchaseOrderItem> lstrpItem = rp.PurchaseOrderItems;

            var lstReqPurchase = from p in lstrpItem
                                   select new
                                   {
                                       No = rp.No,
                                       Date = rp.Date.ToShortDateString(),
                                       ApprovedBy = rp.ApprovedByObj.FullName,
                                       PreparedBy = rp.PreparedByObj.FullName,
                                       CheckedBy = rp.CheckedBy,
                                       ItemCode = p.Item.Code,
                                       Description = p.Item.Description,
                                       ItemType = p.Item.Type.Description,
                                       Brand = p.Item.Brand,
                                       Quantity = p.Quantity,
                                       UnitCost = p.UnitCost.ToString("N"),
                                       TotalAmount = p.Amount.ToString("N"),
                                       Remarks = p.Remarks
                                   };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstReqPurchase;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\RequisitionPurchase\Report\rptReqPurchase.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Date", rp.Date.ToShortDateString()));
            parameters.Add(new ReportParameter("RPNumber", rp.No));
            parameters.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            parameters.Add(new ReportParameter("CompanyAddress", companyRepo.GetById(Sessions.CompanyId.Value).Address));
            reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
      

        public void InsertStockCard(int ReferenceId, int itemId, decimal unitcost, int quantiy)
        {
            ItemStockCardRepository stockcardRepo = new ItemStockCardRepository();

            StockCard stockCard = new StockCard
            {
                ItemId = itemId,
                ReferenceModule = "Requisition to purchase",
                ReferenceId = ReferenceId,
                Qty = quantiy,
                UnitCost = unitcost,
                CompanyId = Sessions.CompanyId.Value,
                Date = DateTime.Now
            };

            stockcardRepo.Add(stockCard);
        }
    }
}