using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNG.Models
{
    public class RequisitionItemVM
    {
        ItemTypeRepository itemTypeRepo = new ItemTypeRepository();

        public RequisitionItem RequisitionItem { get; set; }
        public int CompanyId { get; set; }
        public IEnumerable<SelectListItem> ItemTypes {
            get {
                List<SelectListItem> lstSelItem = new List<SelectListItem>();

                SelectListItem selList1 = new SelectListItem
                {
                    Text = "Scrap",
                    Value = "1"
                };

                SelectListItem selList2 = new SelectListItem
                {
                    Text = "Junk",
                    Value = "2"
                };

                lstSelItem.Add(selList1);
                lstSelItem.Add(selList2);

                return lstSelItem;
            }
        }
    }
}