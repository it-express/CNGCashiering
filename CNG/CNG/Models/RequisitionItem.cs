using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionItem
    {
        ItemRepository itemRepo = new ItemRepository();

        [Key]
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string SerialNo { get; set; }
        public int Type { get; set; }

        public int QuantityReturn { get; set; }
        public string SerialNoReturn { get; set; }

        public int? TransactionLogId { get; set; }

        [ForeignKey("ItemId")]

        public decimal GetItemUnitCost
        { get { return itemRepo.GetByItemId(ItemId,Sessions.CompanyId).UnitCost; } }
        public virtual Item Item { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public string TypeDescription {
            get {
                if (Type == 1)
                {
                    return "scrap";
                }
                else {
                    return "junk";
                }
            }
        }
    }
}