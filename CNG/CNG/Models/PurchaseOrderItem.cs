using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderItem
    {
        private ReceivingRepository receivingRepo = new ReceivingRepository();
        private  PurchaseOrderRepository poRepo = new PurchaseOrderRepository();

        public PurchaseOrderItem()
        {
            Receivings = new List<Receiving>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal UnitCost { get; set; }

        public string GetUnitCost
        { get { return UnitCost.ToString("#,##0.00"); } }

        public decimal DisplayUnitCost
        { get
            {
                decimal Unit;
                if (UnitCost > 1.00m)
                {
                    Unit = UnitCost;
                }
                else
                {
                    Unit = Item.UnitCost;
                }

                return Unit;
            }

        }

        [Required]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(200)]
        [DisplayName("Serial No")]
        public string SerialNo { get; set; }

        [DisplayName("Received Quantity")]
        public int ReceivedQuantity {
            get {
                int total = receivingRepo.ListByPurchaseOrderItemId(Id).Sum(p => p.Quantity);

                return total;
            }
        }

        public DateTime? ReceivedDate
        {
            get
            {
                DateTime? receiveddate = receivingRepo.ListByPurchaseOrderItemId(Id).OrderByDescending(p=>p.DateReceived).First().DateReceived;

                return receiveddate;
            }
        }

        public string VendorName
        {
            get
            {
                string vendor = "";
                try
                {
                     vendor = poRepo.GetById(PurchaseOrderId).Vendor.Name;
                }
                catch {  vendor = ""; }

                return vendor;
            }
        }

        public string DrNo { get; set; }

        public DateTime Date { get; set; }

        public DateTime? RemainingBalanceDate { get; set; }

        public int? TransactionLogId { get; set; }

        public virtual Item Item { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual List<Receiving> Receivings { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal Amount {
            get {
                return UnitCost * Quantity;
            }
        }

        public string GetAmount
        { get { return Amount.ToString("#,##0.00"); } }

        public int Balance {
            get {
                return Quantity - ReceivedQuantity;
            }
        }

 
    }
}