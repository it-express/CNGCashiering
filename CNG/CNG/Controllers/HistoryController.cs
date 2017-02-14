using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;
using PagedList;
using System.Linq.Dynamic;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Core.Objects;

namespace CNG.Controllers
{
    [AuthorizationFilter]
    public class HistoryController : Controller
    {
        VehicleRepository vehicleRepo = new VehicleRepository();
        VehicleItemsRepository vehicleItemRepo = new VehicleItemsRepository();
        VehicleAssignmentRepository vehicleAssignmentRepo = new VehicleAssignmentRepository();
        ItemRepository itemRepo = new ItemRepository();
        TransactionLogRepository transactionLogRepo = new TransactionLogRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        CNGDBContext context = new CNGDBContext();

        // GET: Inventory
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

            IQueryable<Vehicle> lstVehicle = vehicleAssignmentRepo.List()
                                            .Where(p => p.CompanyId == Sessions.CompanyId.Value)
                                            .Select(q => q.Vehicle);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstVehicle = lstVehicle.Where(s => s.LicenseNo.Contains(searchString)
                                       || s.EngineNo.Contains(searchString)
                                       || s.ChasisNo.Contains(searchString)
                                       || s.Make.Contains(searchString)
                                       || s.Year.ToString().Contains(searchString));
            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstVehicle = lstVehicle.OrderBy(p => p.LicenseNo);
            }
            else
            {
                lstVehicle = lstVehicle.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstVehicle.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TransactionHistory(int id, string sortColumn, string sortOrder, string currentFilter, string searchString, string dateFrom, string dateTo, int? page)
        {
           
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
            ViewBag.DateTo = dtDateTo.ToShortDateString();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            List<int?> lstVehicle = vehicleItemRepo.List().Where(v => v.VehicleId == id).Select(v => v.TransactionLogId).ToList();

            //IQueryable<TransactionLog> lstTransactionLog = transactionLogRepo.List().Where(p => lstVehicle.Contains(p.Id));

            //lstTransactionLog = from p in lstTransactionLog
            //                    where DbFunctions.TruncateTime(p.Date) >= DbFunctions.TruncateTime(dtDateFrom) &&
            //                                                  DbFunctions.TruncateTime(p.Date) <= DbFunctions.TruncateTime(dtDateTo)
            //                                                  && p.CompanyId == Sessions.CompanyId.Value
            //                    select p;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    lstTransactionLog = lstTransactionLog.Where(s => s.Item.Description.Contains(searchString)
            //                           || s.Date.ToString().Contains(searchString));        

            //}

            //if (String.IsNullOrEmpty(sortColumn))
            //{
            //    lstTransactionLog = lstTransactionLog.OrderByDescending(p => p.Id);
            //}
            //else
            //{
            //    lstTransactionLog = lstTransactionLog.OrderBy(sortColumn + " " + sortOrder);
            //}

            // IQueryable<TransactionLogVM> lstTransactionLog =  transactionLogRepo.List().Where(p => lstVehicle.Contains(p.Id));

            var lstVehicleStockTransferItem = from p in context.VehicleStockTransferItems
                                              select new TransactionLogVM
                                              {
                                                  TransactionLog = p.TransactionLog,
                                                  VehicleFrom = p.VehicleFrom.LicenseNo
                                              };

            List<TransactionLogVM> lstTransactionLog = (from p in transactionLogRepo.List().Where(p => DbFunctions.TruncateTime(p.Date) >= DbFunctions.TruncateTime(dtDateFrom) &&
                                                               DbFunctions.TruncateTime(p.Date) <= DbFunctions.TruncateTime(dtDateTo)
                                                              && p.CompanyId == Sessions.CompanyId.Value).ToList()
                                                        join q in lstVehicleStockTransferItem.ToList()
                                                        on p.Id equals q.TransactionLog.Id into pq from sub in pq.DefaultIfEmpty()
                                                        where lstVehicle.Contains(p.Id)

                                                        select new TransactionLogVM
                                                        {
                                                            TransactionLog = p,
                                                            VehicleFrom = sub == null ? null: "from vehicle:" + sub.VehicleFrom
                                                        }).ToList();

            //lstTransactionLog = (from p in lstTransactionLog
            //                     where DbFunctions.TruncateTime(p.TransactionLog.Date) >= DbFunctions.TruncateTime(dtDateFrom) &&
            //                                                   DbFunctions.TruncateTime(p.TransactionLog.Date) <= DbFunctions.TruncateTime(dtDateTo)
            //                                                  && p.TransactionLog.CompanyId == Sessions.CompanyId.Value).ToList();
                              
                                        



            if (!String.IsNullOrEmpty(searchString))
            {
                lstTransactionLog = (lstTransactionLog.Where(s => s.TransactionLog.Item.Description.Contains(searchString)
                                       || s.TransactionLog.Date.ToString().Contains(searchString)
                                       || s.VehicleFrom.Contains(searchString))).ToList();

            }

            if (String.IsNullOrEmpty(sortColumn))
            {
                lstTransactionLog = (lstTransactionLog.OrderByDescending(p => p.TransactionLog.Id)).ToList();
            }
            else
            {
                lstTransactionLog = (lstTransactionLog.OrderBy(sortColumn + " " + sortOrder)).ToList();
            }



            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var transHistoryVM = new ItemHistoryVM
            {
                Vehicle = vehicleRepo.GetById(id),
                Company = companyRepo.GetById(Sessions.CompanyId.Value),
                TransactionLogs = lstTransactionLog.ToPagedList(pageNumber, pageSize),
            };

            return View(transHistoryVM);
        }


    
    }
}