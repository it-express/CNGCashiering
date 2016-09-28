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

        // GET: Inventory
        public ActionResult Index()
        {
            List<Item> lstItem = itemRepo.List().ToList();

            return View(lstItem);
        }
    }
}