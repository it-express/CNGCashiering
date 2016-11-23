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
    public class StockTransferController : Controller
    {
        CNGDBContext context = new CNGDBContext();
        StockTransferRepository stRepo = new StockTransferRepository();
        CompanyRepository companyRepo = new CompanyRepository();
        ItemRepository itemRepo = new ItemRepository();

        // GET: StockTransfer
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

            IQueryable<StockTransfer> lstSt = stRepo.List();

            if (!String.IsNullOrEmpty(searchString))
            {
                lstSt = lstSt.Where(s => s.Date.ToString().Contains(searchString)
                                       || s.PreparedByObj.FirstName.Contains(searchString)
                                       || s.PreparedByObj.LastName.Contains(searchString)
                                       || s.ApprovedByObj.FirstName.Contains(searchString)
                                       || s.ApprovedByObj.LastName.Contains(searchString));
            }

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
            ViewBag.Companies = new SelectList(context.Companies, "Id", "Name");
            ViewBag.User = Common.GetCurrentUser.FullName;
            ViewBag.GeneralManager = Common.GetCurrentUser.GeneralManager.FullName;
            

            StockTransfer stockTransfer = new StockTransfer
            {
                No = stRepo.GenerateStockTransferNo(),
                Date = DateTime.Now
            };

            return View(stockTransfer);
        }

        public ActionResult RenderEditorRow(int itemId, int transferFrom)
        {
            StockTransferItem stockTransferItem = new StockTransferItem
            {
                Item = itemRepo.GetById(itemId)
            };

            ViewBag.TransferFrom = transferFrom;
            return PartialView("_EditorRow", stockTransferItem);
        }

        public void Save(StockTransferDTO entry)
        {
            StockTransfer st = new StockTransfer();

            st.No = entry.No;
            st.Date = Convert.ToDateTime(entry.Date);
            st.TransferFrom = entry.TransferFrom;

            st.PreparedBy = Common.GetCurrentUser.Id;
            st.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
            //st.CheckedBy = entry.CheckedBy;

            st.StockTransferItems = new List<StockTransferItem>();
            foreach (StockTransferDTO.Item item in entry.Items)
            {
                StockTransferItem stItem = new StockTransferItem();

                stItem.StockTransferId = st.Id;
                stItem.ItemId = item.Id;
                stItem.Quantity = item.Quantity;
                stItem.Remarks = item.Remarks;

                st.StockTransferItems.Add(stItem);
            }

            stRepo.Save(st);
        }
    }
}