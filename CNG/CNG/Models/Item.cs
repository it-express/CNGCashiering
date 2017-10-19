using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class Item
    {
        TransactionLogRepository transLogRepo = new TransactionLogRepository();
        ItemAssignmentRepository itemAssignRepo = new ItemAssignmentRepository();
        ItemRepository itemRepo = new ItemRepository();

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Code { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(250)]
        public string Brand { get; set; }

        [Required]
        [DisplayName("Unit Cost")]
        [DataType(DataType.Text)]
        public decimal UnitCost { get; set; }

        [Required]
        [DisplayName("Type")]
        public int TypeId { get; set; }

        [Required]
        [DisplayName("Classification")]
        public int ClassificationId { get; set; }

        [Required]
        public bool Active { get; set; }

        [DisplayName("Quantity on Hand")]
        public int QuantityOnHand(int companyId)
        {
            return transLogRepo.SumByItemId(Id, companyId);
        }

        public string GetCode(int itemid)
        {
            string code = "";

            var item = itemRepo.GetById(itemid);

            try
            {
                if (item.Code != null)
                {
                    code = item.Code;
                }
            }
            catch
            {
                code = itemRepo.GeneratedItemCode();
            }          
            

            return code;
        }

        public decimal GetUnitCostByCompany
        {
            get
            {
                try

                {
                    return itemAssignRepo.GetUnitCostByCompany(Id, Sessions.CompanyId);
                }
                catch
                {
                    return 0;
                }
            }
           
        }

        public decimal GetLatestUnitCostByCompany
        {
            get
            {
                try

                {
                    return itemAssignRepo.GetLatestUnitCostByCompany(Id, Sessions.CompanyId);
                }
                catch
                {
                    return 0;
                }
            }

        }



        public virtual ItemType Type { get; set; }

        public virtual ItemClassification Classification { get; set; }
    }
}