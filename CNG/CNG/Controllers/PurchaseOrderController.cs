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
    public class PurchaseOrderController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        PurchaseOrderRepository poRepo = new PurchaseOrderRepository();
        PurchaseOrderItemRepository poItemRepo;
        VendorRepository vendorRepo = new VendorRepository();
        ItemRepository itemRepo = new ItemRepository();
        CompanyRepository companyRepo = new CompanyRepository();

        public PurchaseOrderController() {
            poItemRepo = new PurchaseOrderItemRepository(context);
        }

        // GET: PurchaseOrder
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

            IQueryable<PurchaseOrder> lstPo = poRepo.List().Where(p => p.ShipTo == companyId);

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
            ViewBag.PoNumber = poRepo.GeneratePoNumber();
            ViewBag.Vendors = new SelectList(context.Vendors.Where(p => p.Active), "Id", "Name");
            InitViewBags();

            PurchaseOrder po = new PurchaseOrder();
            po.Vendor = new Vendor();
            po.ShipToCompany = new Company();

            PurchaseOrderVM poVM = new PurchaseOrderVM();
            poVM.PurchaseOrder = po;
            int companyId = Convert.ToInt32(Request.QueryString["companyId"]);
            poVM.SelectedCompany = companyRepo.GetById(companyId);

            return View(poVM);
        }

        public ActionResult Edit(string poNo) {
            PurchaseOrderVM poVM = new PurchaseOrderVM();
            poVM.PurchaseOrder = poRepo.GetByNo(poNo);
            int companyId = Convert.ToInt32(Request.QueryString["companyId"]);
            poVM.SelectedCompany = companyRepo.GetById(companyId);

            ViewBag.PoNumber = poNo;
            ViewBag.Vendors = new SelectList(context.Vendors.Where(p => p.Active), "Id", "Name", poVM.PurchaseOrder.VendorId.ToString());
            InitViewBags();

            return View("Create", poVM);
        }

        public ActionResult Details(string poNo) {
            InitViewBags();
            PurchaseOrder po = poRepo.GetByNo(poNo);

            return View(po);
        }

        public ActionResult Delete(string poNo, int companyId) {
            poRepo.Delete(poNo);

            return RedirectToAction("Index", new { companyId = companyId });
        }

        public string GeneratePoNumber() {
            string poNumber = poRepo.GeneratePoNumber();

            return poNumber;
        }

        [HttpPost]
        public JsonResult ListItemByPoNo(string poNo) {
            PurchaseOrder po = poRepo.GetByNo(poNo);

            List<PurchaseOrderItem> lstPoItem = new List<PurchaseOrderItem>();
            if (po != null) {
                lstPoItem = po.PurchaseOrderItems;
            }

            return Json(lstPoItem);
        }

        public void Save(PurchaseOrderDTO entry) {
            PurchaseOrder po = new PurchaseOrder();

            //po.No = poRepo.GeneratePoNumber();
            po.No = entry.No;

            po.Date = DateTime.Now;
            po.VendorId = entry.VendorId;
            po.ShipTo = entry.ShipTo;
            po.Terms = vendorRepo.GetById(entry.VendorId).Terms;

            po.PreparedBy = Common.GetCurrentUser.Id;
            po.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
            po.CheckedBy = entry.CheckedBy;

            po.PurchaseOrderItems = new List<PurchaseOrderItem>();
            foreach (PurchaseOrderDTO.Item item in entry.Items)
            {
                PurchaseOrderItem poItem = new PurchaseOrderItem();
                poItem.PurchaseOrderId = po.Id;
                poItem.ItemId = item.Id;
                Item _item = itemRepo.GetById(item.Id);

                poItem.UnitCost = _item.UnitCost;
                poItem.Quantity = item.Quantity;
                poItem.Remarks = item.Remarks;
                poItem.Date = DateTime.Now;
                poItem.RemainingBalanceDate = DateTime.Now;

                po.PurchaseOrderItems.Add(poItem);
            }

            poRepo.Save(po);
        }

        private void InitViewBags()
        {
            
            
            ViewBag.Items = new SelectList(context.Items.Where(p => p.Active), "Id", "Description");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            int companyId = Convert.ToInt32(Request.QueryString["companyId"]);
            ViewBag.Companies = new SelectList(context.Companies.Where(p => p.Active), "Id", "Name", companyId);

            ViewBag.CompanyId = companyId.ToString();
        }
    }
}