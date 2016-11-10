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

                List<ItemType> itemTypes = itemTypeRepo.List().ToList();
                foreach (ItemType type in itemTypes) {
                    SelectListItem selList = new SelectListItem
                    {
                        Text = type.Description,
                        Value = type.Id.ToString()
                    };

                    lstSelItem.Add(selList);
                }

                return lstSelItem;
            }
        }
    }
}