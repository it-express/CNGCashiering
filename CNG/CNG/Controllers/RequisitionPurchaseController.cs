﻿using System;
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
    public class RequisitionPurchaseController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        RequisitionPurchaseRepository rpRepo = new RequisitionPurchaseRepository();
        RequisitionPurchaseItemRepository rpItemRepo;
        CompanyRepository companyRepo = new CompanyRepository();
        ItemRepository itemRepo = new ItemRepository();

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

            IQueryable<RequisitionPurchase> lstRp = rpRepo.List().Where(p => p.CompanyId == Sessions.CompanyId.Value);

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
            ViewBag.Items = new SelectList(context.Items, "Id", "Description");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;

            RequisitionPurchase reqPurchase = new RequisitionPurchase
            {
                No = rpRepo.GenerateRpNo(),
                Date = DateTime.Now
            };

            return View(reqPurchase);
        }

        public ActionResult Remove(string rpNo) {
            rpRepo.Delete(rpNo);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int rpNo) {
            RequisitionPurchase rp = rpRepo.GetById(rpNo);
            rp.RequisitionPurchaseItems = new List<RequisitionPurchaseItem>();

            return View(rp);
        }

        public ActionResult RenderEditorRow(int itemId)
        {
            RequisitionPurchaseItem reqPurchaseItem = new RequisitionPurchaseItem
            {
                Item = itemRepo.GetById(itemId)
            };

            return PartialView("_EditorRow", reqPurchaseItem);
        }

        public void Save(RequisitionPurchaseDTO rpDTO)
        {
            RequisitionPurchase rp = new RequisitionPurchase();
            rp.No = rpRepo.GenerateRpNo();
            rp.Date = DateTime.Now;
            rp.CompanyId = Sessions.CompanyId.Value;

            rp.PreparedBy = Common.GetCurrentUser.Id;
            rp.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;

            context.RequisitionPurchases.Add(rp);
            context.SaveChanges();

            foreach (RequisitionPurchaseDTO.Item dtoItem in rpDTO.Items)
            {
                RequisitionPurchaseItem rpItem = new RequisitionPurchaseItem();

                rpItem.RequisitionPurchaseId = rp.Id;
                rpItem.ItemId = dtoItem.ItemId;
                rpItem.UnitCost = dtoItem.UnitCost;
                rpItem.Quantity = dtoItem.Quantity;
                rpItem.Remarks = dtoItem.Remarks;

                context.RequisitionPurchaseItems.Add(rpItem);
                context.SaveChanges();

                InsertLogs(rpItem.ItemId, rpItem.Quantity);
            }
        }

        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = quantiy,
                TransactionMethodId = (int)ETransactionMethod.RequisitionToPurchase,
                CompanyId = Sessions.CompanyId.Value
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}