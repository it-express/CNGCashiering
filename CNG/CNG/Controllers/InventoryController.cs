﻿using System;
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
    public class InventoryController : Controller
    {
        ItemRepository itemRepo = new ItemRepository();
        TransactionLogRepository transactionLogRepo = new TransactionLogRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();

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

            //IQueryable<Item> lstItem = from p in itemRepo.List() select p;
            IQueryable<Item> lstItem = itemAssignmentRepo.List()
                                    .Where(p => p.CompanyId == Sessions.CompanyId)
                                    .Select(p => p.Item);

            if (!String.IsNullOrEmpty(searchString))
            {
                lstItem = lstItem.Where(s => s.Code.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Brand.Contains(searchString)
                                       || s.UnitCost.ToString().Contains(searchString)
                                       || s.Type.Description.ToString().Contains(searchString));
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

            IQueryable<TransactionLog> lstTransactionLog = transactionLogRepo.List().Where(p => p.ItemId == id);
            lstTransactionLog = from p in lstTransactionLog
                            where DbFunctions.TruncateTime(p.Date) >= DbFunctions.TruncateTime(dtDateFrom) &&
                                                          DbFunctions.TruncateTime(p.Date) <= DbFunctions.TruncateTime(dtDateTo)
                                                          && p.CompanyId == Sessions.CompanyId.Value && p.TransactionMethodId != 7
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
                lstTransactionLog = lstTransactionLog.OrderByDescending(p => p.Id);
            }
            else
            {
                lstTransactionLog = lstTransactionLog.OrderBy(sortColumn + " " + sortOrder);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var transHistoryVM = new TransactionHistoryVM {
                Item = itemRepo.GetById(id),
                Company = companyRepo.GetById(Sessions.CompanyId.Value),
                TransactionLogs = lstTransactionLog.ToPagedList(pageNumber, pageSize)
            };

            return View(transHistoryVM);
        }

        public ActionResult InventoryReport(string dateFrom, string dateTo) {
            DateTime dtDateFrom = DateTime.Now.Date;
            DateTime dtDateTo = DateTime.Now;

            if (!String.IsNullOrEmpty(dateFrom)) {
                dtDateFrom = Convert.ToDateTime(dateFrom);
            }

            if (!String.IsNullOrEmpty(dateTo)) {
                dtDateTo = Convert.ToDateTime(dateTo);
            }

            var lstInventory2 = (from p in transactionLogRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value ).ToList()                           
                                group p by p.ItemId into g
                                select new
                                {
                                    ItemId = g.Key,
                                    EndingQuantity = g.Where(p => p.Date.Date <= dtDateTo ).Sum(p => p.Quantity),
                                    In = g.Where(p => p.Quantity > 0 && p.Date.Date >= dtDateFrom && p.Date.Date <= dtDateTo && p.TransactionMethodId != 7).Sum(p => p.Quantity),
                                    Out = g.Where(p => p.Quantity < 0 && p.Date.Date >= dtDateFrom && p.Date.Date <= dtDateTo && p.TransactionMethodId != 7).Sum(p => p.Quantity),
                                    StartingQuantity = g.Where(p => p.Date.Date <= dtDateTo).Sum(p => p.Quantity)
                                }).ToList();



            var lstItemAssignment = from itemAssign in itemAssignmentRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value).ToList()
                                    join item in itemRepo.List()
                                    on itemAssign.ItemId equals item.Id into lst
                                    from l in lst
                                    select new
                                    {
                                        ItemId = l.Id,
                                        Code = l.Code,
                                        Description = l.Description,
                                        UnitCost = l.UnitCost
                                    };

            var lstInventory = from inv in lstInventory2
                               join item in lstItemAssignment
                               on inv.ItemId equals item.ItemId into itemInv
                               from i in itemInv
                               select new
                               {
                                   Code = i.Code,
                                   Description = i.Description,
                                   UnitCost = string.Format("{0:#,##0.00}", i.UnitCost), //item.UnitCost.ToString("F"),
                                   StartingQuantity = i != null ? inv.StartingQuantity - (inv.In + inv.Out) : 0,
                                   EndingQuantity = i != null ? inv.EndingQuantity : 0, 
                                   In = i != null ? inv.In : 0,
                                   Out = i != null ? inv.Out : 0
                               };

            int currCompanyId = Sessions.CompanyId.Value;
            int totalMaterials = 0; //itemRepo.List().Where(p => p.ClassificationId == (int)EItemClassification.Materials).ToList().Sum(p => p.QuantityOnHand(currCompanyId));
            int totalTires = 0; //itemRepo.List().Where(p => p.ClassificationId == (int)EItemClassification.Tires).ToList().Sum(p => p.QuantityOnHand(currCompanyId));
            int totalBatteries = 0; //itemRepo.List().Where(p => p.ClassificationId == (int)EItemClassification.Batteries).ToList().Sum(p => p.QuantityOnHand(currCompanyId));

             ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            ReportDataSource _rds = new ReportDataSource();
            _rds.Name = "DataSet1";
            _rds.Value = lstInventory.OrderBy(p => p.Code);

            reportViewer.KeepSessionAlive = false;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Views\Inventory\Report\rptInventory.rdlc";

            List<ReportParameter> _parameter = new List<ReportParameter>();
            _parameter.Add(new ReportParameter("DateRange", dtDateFrom.ToString("MMMM dd, yyyy") + " - " + dtDateTo.ToString("MMMM dd, yyyy")));
            _parameter.Add(new ReportParameter("CompanyName", companyRepo.GetById(Sessions.CompanyId.Value).Name));
            _parameter.Add(new ReportParameter("TotalMaterials", totalMaterials.ToString()));
            _parameter.Add(new ReportParameter("TotalTires", totalTires.ToString()));
            _parameter.Add(new ReportParameter("TotalBatteries", totalBatteries.ToString()));

            reportViewer.LocalReport.DataSources.Add(_rds);
            reportViewer.LocalReport.Refresh();
            reportViewer.LocalReport.SetParameters(_parameter);

            ViewBag.ReportViewer = reportViewer;

            ViewBag.DateFrom = dtDateFrom.ToString("MM/dd/yyyy");
            ViewBag.DateTo = dtDateTo.ToString("MM/dd/yyyy");

            ViewBag.CompanyName = companyRepo.GetById(Sessions.CompanyId.Value).Name;

            return View();
        }
    }
}