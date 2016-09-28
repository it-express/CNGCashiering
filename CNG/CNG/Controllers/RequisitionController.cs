using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNG.Models;

namespace CNG.Controllers
{
    public class RequisitionController : Controller
    {
        private CNGDBContext context = new CNGDBContext();
        RequisitionRepository reqRepo = new RequisitionRepository();
        RequisitionItemRepository reqItemRepo;
        VehicleRepository vehicleRepo = new VehicleRepository();
        ItemRepository itemRepo = new ItemRepository();

        public RequisitionController() {
            reqItemRepo = new RequisitionItemRepository(context);
        }

        public ActionResult Index()
        {
            List<Requisition> lstReq = reqRepo.List().ToList();

            return View(lstReq);
        }

        public ActionResult Create() {
            ViewBag.reqNumber = reqRepo.GenerateReqNo();

            var lstPlateNos = from  p in vehicleRepo.List()
                              select new { No = p.LicenseNo };

            ViewBag.PlateNos = new SelectList(lstPlateNos, "No", "No");
            ViewBag.Items = new SelectList(itemRepo.List(), "Id", "Code");

            return View(new Requisition());
        }

        public ActionResult Delete(string reqNo) {
            reqRepo.Delete(reqNo);

            return RedirectToAction("Index");
        }

        public ActionResult Details(string reqNo) {
            Requisition req = reqRepo.GetByNo(reqNo);
            req.RequisitionItems = new List<RequisitionItem>();

            return View(req);
        }

        public void Save(RequisitionDTO entry)
        {
            Requisition req = new Requisition();

            req.No = reqRepo.GenerateReqNo();
            req.Date = entry.JobOrderDate;
            req.JobOrderNo = entry.JobOrderNo;
            req.UnitPlateNo = entry.UnitPlateNo;
            req.JobOrderDate = entry.JobOrderDate;
            req.OdometerReading = entry.OdometerReading; //Get from session
            req.DriverName = entry.DriverName; //Get from session
            req.ReportedBy = 0; //Get from session
            req.CheckedBy = 0; //Get from session
            req.ApprovedBy = 0; //Get from session

            context.Requisitions.Add(req);
            context.SaveChanges();

            foreach (RequisitionDTO.Item item in entry.Items)
            {
                RequisitionItem reqItem = new RequisitionItem();
                reqItem.RequisitionId = req.Id;

                Item _item = itemRepo.GetById(item.ItemId);

                reqItem.Quantity = item.Quantity;
                reqItem.SerialNo = item.SerialNo;
                reqItem.Type = item.Type;

                reqItemRepo.Save(reqItem);

                InsertLogs(item.ItemId, reqItem.Quantity);
            }
        }
        
        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                TransactionMethodId = (int)ETransactionMethod.Requisition,
                Date = DateTime.Now
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}