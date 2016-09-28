using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class InventoryController : Controller
    {
        ItemRepository itemRepo = new ItemRepository();
        TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

        // GET: Inventory
        public ActionResult Index()
        {
            List<Item> lstItem = itemRepo.List().ToList();

            return View(lstItem);
        }

        public ActionResult TransactionHistory(int id) {
            List<TransactionLog> lstTransactionLog = transactionLogRepo.List()
                .Where(p => p.ItemId == id)
                .OrderByDescending(p => p.Date).ToList();

            return View(lstTransactionLog);
        }
    }
}