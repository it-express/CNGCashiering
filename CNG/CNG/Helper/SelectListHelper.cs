using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNG.Models
{
    public class SelectListHelper
    {
        public static SelectList ItemTypes()
        {
            ItemTypeRepository itemTypeRepo = new ItemTypeRepository();
            SelectList userSelectList = new SelectList(itemTypeRepo.List(), "Id", "Description", "");

            return userSelectList;
        }

        public static SelectList UserTypes() {
            UserTypeRepository userTypeRepo = new UserTypeRepository();
            SelectList userSelectList = new SelectList(userTypeRepo.List(), "Id", "Description", "");

            return userSelectList;
        }

        public static SelectList GeneralManagers()
        {
            UserRepository userRepo = new UserRepository();
            SelectList selectList = new SelectList(userRepo.GetByUserTypeId((int) EUserType.GeneralManager), "Id", "FullName", "");

            return selectList;
        }
    }
}