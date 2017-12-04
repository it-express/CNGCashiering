using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;
using PagedList;
using System.Linq.Dynamic;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;

namespace CNG.Controllers
{
    [AuthorizationFilter]

    public class PurchaseOrderController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        VendorRepository vendorRepo = new VendorRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();
        UserRepository userRepo = new UserRepository();
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();

        public PurchaseOrderController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

        // GET: PurchaseOrder
        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CompanyId = Sessions.CompanyId;
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

            IQueryable<PurchaseOrder> lstPo = poRepo.List().Where(p => p.ShipTo == Sessions.CompanyId && p.isRP == false);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstPo = lstPo.Where(s => s.No.Contains(searchString)
                                       || s.Date.ToString().Contains(searchString)
                                       || s.Vendor.Name.ToString().Contains(searchString)
                                       || s.ShipToCompany.Name.Contains(searchString)
                                       || s.Terms.ToString().Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstPo = lstPo.OrderByDescending(p => p.Id);
            }
            else
            {
                lstPo = lstPo.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstPo.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            
            //ViewBag.PoNumber = poRepo.GeneratePoNumber(DateTime.Now);
            
            ViewBag.Vendors = new SelectList(context.Vendors.Where(p => p.Active), "Id", "Name");
            ViewBag.User = Common.GetCurrentUser.FullName;
            InitViewBags();

            PurchaseOrder po = new PurchaseOrder();
            po.Vendor = new Vendor();
            po.ShipToCompany = new Company();

            PurchaseOrderVM poVM = new PurchaseOrderVM();
            poVM.PurchaseOrder = po;
            int companyId = Convert.ToInt32(Sessions.CompanyId);
            poVM.SelectedCompany = companyRepo.GetById(companyId);
            ViewBag.Update = "0";

            return View(poVM);
        }

        public ActionResult Edit(string poNo) {
            PurchaseOrderVM poVM = new PurchaseOrderVM();
            poVM.PurchaseOrder = poRepo.GetByNo(poNo);
            int companyId = poVM.PurchaseOrder.ShipTo;
            poVM.SelectedCompany = companyRepo.GetById(companyId);

            ViewBag.PoNumber = poNo;
            ViewBag.Vendors = new SelectList(context.Vendors.Where(p => p.Active), "Id", "Name", poVM.PurchaseOrder.VendorId.ToString());
            ViewBag.User = poVM.PurchaseOrder.PreparedByObj.FullName;
            ViewBag.Update = "1";
            InitViewBags();

            return View("Create", poVM);
        }

        public ActionResult Details(string poNo) {
            InitViewBags();
            PurchaseOrder po = poRepo.GetByNo(poNo);

            return View(po);
        }

        public ActionResult Delete(string poNo) {
            poRepo.Delete(poNo);

            return RedirectToAction("Index");
        }

        public ActionResult RenderEditorRow(int itemId) {

            PurchaseOrderItem poItem = new PurchaseOrderItem();
            poItem.Item = itemRepo.GetById(itemId);

            return PartialView("_EditorRow", poItem);
        }

        //public string GeneratePoNumber() {
        //    string poNumber = poRepo.GeneratePoNumber();

        //    return poNumber;
        //}

        [HttpPost]
        public JsonResult ListItemByPoNo(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            List<PurchaseOrderItem> lstPoItem = new List<PurchaseOrderItem>();
            if (po != null) {
                lstPoItem = po.PurchaseOrderItems;
            }

            return Json(lstPoItem);
        }

        public void Save(PurchaseOrderDTO entry)
        {
            PurchaseOrder po = new PurchaseOrder();

            //po.No = poRepo.GeneratePoNumber();
            po.No = entry.No;

            po.Date = Convert.ToDateTime(entry.Date);
            po.VendorId = entry.VendorId;
            po.ShipTo = entry.ShipTo;
            po.Terms = vendorRepo.GetById(entry.VendorId).Terms;

            po.PreparedBy = Common.GetCurrentUser.Id;
            po.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
            po.CheckedBy = entry.CheckedBy;
            po.isRP = false;
            po.CompanyId = Sessions.CompanyId.Value;

            po.PurchaseOrderItems = new List<PurchaseOrderItem>();
            foreach (PurchaseOrderDTO.Item item in entry.Items)
            {
                PurchaseOrderItem poItem = new PurchaseOrderItem();
                poItem.PurchaseOrderId = po.Id;
                poItem.ItemId = item.Id;
                Item _item = itemRepo.GetById(item.Id);

                poItem.UnitCost = Convert.ToDecimal(item.UnitCost);
                poItem.Quantity = item.Quantity;
                poItem.Remarks = item.Remarks;
                poItem.Date = Convert.ToDateTime(entry.Date);
                poItem.RemainingBalanceDate = null;

                po.PurchaseOrderItems.Add(poItem);

            }

            // for FIFO price
            po.ItemPriceLogs = new List<ItemPriceLogs>();
            foreach (PurchaseOrderDTO.Item item in entry.Items)
            {
                ItemPriceLogs itemLogs = new ItemPriceLogs();
                Item _item = itemRepo.GetById(item.Id);

                itemLogs.PurchaseOrderId = po.Id;
                itemLogs.ItemId = item.Id;
                itemLogs.UnitCost = Convert.ToDecimal(item.UnitCost);
                itemLogs.Qty = item.Quantity;
                itemLogs.Date = DateTime.Now;
                itemLogs.CompanyId = Sessions.CompanyId.Value;

                po.ItemPriceLogs.Add(itemLogs);
                UpdateItemType(item.Id, _item.TypeId, item.TypeId);

            }


            poRepo.Save(po);

            foreach (PurchaseOrderDTO.Item item in entry.Items)
            {
                Item _item = context.Items.Find(item.Id);
                if(_item.TypeId != item.TypeId)
                {
                    _item.TypeId = item.TypeId;
                    context.SaveChanges();
                }

            }

        }

        public void UpdateItemType(int itemid, int oldtypeid, int newtypeid)
        {
            ItemHistory _item = new ItemHistory();

            _item.ItemId = itemid;
            _item.OldItemTypeId = oldtypeid;
            _item.NewItemTypeId = newtypeid;
            _item.CompanyId = Sessions.CompanyId.Value;
            _item.Date = DateTime.Now;
            
        }

        public void Checked(PurchaseOrderDTO entry)
        {
            PurchaseOrder po = new PurchaseOrder();

            po.No = entry.No;
            po.Checked = entry.Checked;

            poRepo.Checked(po);

        }

        public void Approved(PurchaseOrderDTO entry)
        {
            PurchaseOrder po = new PurchaseOrder();

            po.No = entry.No;
            po.Approved = entry.Approved;

            poRepo.Approved(po);

        }




        private void InitViewBags()
        {
            IQueryable<Item> lstItem = itemAssignmentRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value).Select(p => p.Item);
            ViewBag.Items = new SelectList(lstItem.Where(p => p.Active).OrderBy(p=> p.Description), "Id", "Description");
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            int companyId = Convert.ToInt32(Sessions.CompanyId);
            ViewBag.Companies = new SelectList(context.Companies.Where(p => p.Active), "Id", "Name", companyId);

            ViewBag.CompanyId = companyId.ToString();

            ViewBag.UserLevel = userRepo.GetByUserLevel(Common.GetCurrentUser.Id);
            ViewBag.ItemTypes = new SelectList(itemTypeRepo.List(), "Id", "Description");

            SqlParameter parameter1 = new SqlParameter("@CompanyID",Sessions.CompanyId);
            var affectedRows = context.Database.ExecuteSqlCommand("sp_Update_Item_UnitCost @CompanyID", parameter1);
            var affectedRows1 = context.Database.ExecuteSqlCommand("spUpdate_Items_QuantityOnHand");
          
        }

        public ActionResult Report(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);
            List<PurchaseOrderItem> lstPoItem = po.PurchaseOrderItems;

            var lstPurchaseOrder = from p in lstPoItem
                   select new
                       {
                           No = po.No,
                           Vendor = po.Vendor.Name,
                           ShipTo = po.ShipToCompany.Name,
                           Date = po.Date.ToShortDateString(),
                           Terms = po.Terms,
                           Status = po.StatusDescription,
                           ApprovedBy = po.ApprovedByObj.FullName,
                           PreparedBy = po.PreparedByObj.FullName,
                           CheckedBy = po.CheckedBy,
                           ItemCode = p.Item.Code,
                           Description = p.Item.Description,
                           ItemType = p.Item.Type.Description,
                           Brand = p.Item.Brand,
                           Quantity = p.Quantity,
                           UnitCost = p.UnitCost.ToString("N"),
                           TotalAmount = p.Amount.ToString("N"),
                           Remarks = p.Remarks,
                           DueDate = po.DueDate,
                           CompanyAddress = po.ShipToCompany.Address,
                           VendorAddress = po.Vendor.Address
                       };

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstPurchaseOrder;

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\PurchaseOrder\Report\rptPurchaseOrder.rdlc";

            reportViewer.LocalReport.DataSources.Add(_rds);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Date", po.Date.ToShortDateString()));
            parameters.Add(new ReportParameter("PONumber", po.No));
            parameters.Add(new ReportParameter("Status", po.StatusDescription));
            parameters.Add(new ReportParameter("Terms", po.Terms.ToString()));
            parameters.Add(new ReportParameter("DueDate", po.DueDate));
            parameters.Add(new ReportParameter("VendorName", po.Vendor.Name));
            parameters.Add(new ReportParameter("VendorAddress", po.Vendor.Address));
            parameters.Add(new ReportParameter("ShipTo", po.ShipToCompany.Name));
            parameters.Add(new ReportParameter("CompanyAddress", po.ShipToCompany.Address));
            parameters.Add(new ReportParameter("ShipToContact", po.ShipToCompany.ContactNo));
            parameters.Add(new ReportParameter("VendorContact", po.Vendor.ContactNo));
            reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public JsonResult GetPONo(string Date)
        {

            string ponumber = poRepo.GeneratePoNumber(Convert.ToDateTime(Date));

            return Json(ponumber);
        }



        public JsonResult GetItemTypes()
        {
           
            List<ItemType> lstItemTypes = (from p in itemTypeRepo.List().ToList()
                                                             select new ItemType
                                                             {
                                                                 Id = p.Id,
                                                                 Description = p.Description

                                                             }).ToList();

            return Json(lstItemTypes);
        }
    }
}