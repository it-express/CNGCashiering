using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Linq.Dynamic;

namespace CNG.Controllers
{
    [AuthorizationFilter]
    public class ExcessPartsSetController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        ExcessPartsSetRepository epsRepo = new ExcessPartsSetRepository();
        ExcessPartsSetItemRepository epsItemRepo;
        ItemRepository itemRepo = new ItemRepository();

        public ExcessPartsSetController()
        {
            epsItemRepo = new ExcessPartsSetItemRepository(context);
        }

        public ActionResult Index(string sortColumn, string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            IQueryable<ExcessPartsSet> lstEps = epsRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstEps = lstEps.Where(s => s.No.Contains(searchString)
                                       || s.Date.ToString().Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstEps = lstEps.OrderByDescending(p => p.Id);
            }
            else
            {
                lstEps = lstEps.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstEps.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.Items = new SelectList(context.Items, "Id", "Description");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            ExcessPartsSet eps = new ExcessPartsSet
            {
                No = epsRepo.GenerateEpsNo(),
                Date = DateTime.Now
            };

            return View(eps);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            ExcessPartsSet eps = epsRepo.GetById(id);

            return View(eps);
        }

        public ActionResult RenderEditorRow(int itemId)
        {
            ExcessPartsSetItem epsItem = new ExcessPartsSetItem
            {
                Item = itemRepo.GetById(itemId)
            };

            return PartialView("_EditorRow", epsItem);
        }

        public void Save(ExcessPartsSetDTO epsDTO)
        {
            ExcessPartsSet eps = new ExcessPartsSet();
            eps.No = epsRepo.GenerateEpsNo();
            eps.Date = DateTime.Now;

            eps.PreparedBy = Common.GetCurrentUser.Id;
            eps.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
            eps.CheckedBy = epsDTO.CheckedBy;

            context.ExcessPartsSets.Add(eps);
            context.SaveChanges();

            foreach (ExcessPartsSetDTO.Item dtoItem in epsDTO.Items)
            {
                ExcessPartsSetItem epsItem = new ExcessPartsSetItem();

                epsItem.ExcessPartsSetId = eps.Id;
                epsItem.ItemId = dtoItem.ItemId;
                epsItem.UnitCost = dtoItem.UnitCost;
                epsItem.Quantity = dtoItem.Quantity;
                epsItem.Remarks = dtoItem.Remarks;

                context.ExcessPartsSetItems.Add(epsItem);
                context.SaveChanges();

                InsertLogs(epsItem.ItemId, epsItem.Quantity);
            }
        }

        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.ExcessPartsFromSet,
                CompanyId = Sessions.CompanyId.Value
            };

            transactionLogRepo.Add(transactionLog);
        }

        public ActionResult Remove(string epsNo) {
            epsRepo.Delete(epsNo);

            return RedirectToAction("Index");
        }
    }
}