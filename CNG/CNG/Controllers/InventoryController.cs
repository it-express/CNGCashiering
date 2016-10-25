using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;
using PagedList;
using System.Linq.Dynamic;
using System.Data.Entity;

namespace CNG.Controllers
{
    [AuthorizationFilter]
    public class InventoryController : Controller
    {
        ItemRepository itemRepo = new ItemRepository();
        TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

        // GET: Inventory
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

            IQueryable<Item> lstItem = itemRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstItem = lstItem.Where(s => s.Code.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Brand.Contains(searchString)
                                       || s.UnitCost.ToString().Contains(searchString)
                                       || s.Type.Description.ToString().Contains(searchString)
                                       || s.QuantityOnHand.ToString().Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstItem = lstItem.OrderBy(p => p.Code);
            }
            else
            {
                lstItem = lstItem.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstItem.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TransactionHistory(int id, string sortColumn, string sortOrder, string currentFilter, string searchString, string dateFrom, string dateTo, int? page) {
            ViewBag.CurrentSort = sortColumn;
            ViewBag.SortOrder = sortOrder == "asc" ? "desc" : "asc";

            DateTime dtDateFrom = DateTime.Now;
            DateTime dtDateTo = DateTime.Now;

            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                dtDateFrom = Convert.ToDateTime(dateFrom);
                dtDateTo = Convert.ToDateTime(dateTo);
            }

            ViewBag.DateFrom = dtDateFrom.ToShortDateString();
            ViewBag.DateTo = dtDateFrom.ToShortDateString();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<TransactionLog> lstTransactionLog = transactionLogRepo.List().Where(p => p.ItemId == id);
            lstTransactionLog = from p in lstTransactionLog
                            where DbFunctions.TruncateTime(p.Date) >= DbFunctions.TruncateTime(dtDateFrom) &&
                                                          DbFunctions.TruncateTime(p.Date) <= DbFunctions.TruncateTime(dtDateTo)
                                                          select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstTransactionLog = lstTransactionLog.Where(s => s.TransactionMethod.Description.Contains(searchString)
                                       || s.Quantity.ToString().Contains(searchString)
                                       || s.Date.ToString().Contains(searchString)
                                       || s.User.Username.ToString().Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstTransactionLog = lstTransactionLog.OrderBy(p => p.Id);
            }
            else
            {
                lstTransactionLog = lstTransactionLog.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstTransactionLog.ToPagedList(pageNumber, pageSize));
        }
    }
}