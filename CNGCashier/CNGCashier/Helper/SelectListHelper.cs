using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNGCashier.Models
{
    public class SelectListHelper
    {
        public static SelectList UserTypes()
        {
            UserTypeRepo userTypeRepo = new UserTypeRepo();
            SelectList userSelectList = new SelectList(userTypeRepo.List(), "Id", "Description", "");

            return userSelectList;
        }

        public static SelectList GeneralManagers()
        {
            UserRepo userRepo = new UserRepo();
            SelectList selectList = new SelectList(userRepo.GetByUserTypeId((int)EUserType.GeneralManager), "Id", "FullName", "");

            return selectList;
        }
    }
}